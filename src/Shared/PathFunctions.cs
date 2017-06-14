/********************************************************************

时间: 2015年10月20日, PM 03:10:52

作者: lanyanmiyu@qq.com

描述: 路径辅助类

其它:     

********************************************************************/



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lanymy.General.Extension.ExtensionFunctions;
using Lanymy.General.Extension.Models;
using System.Text.RegularExpressions;



namespace Lanymy.General.Extension
{


    /// <summary>
    /// 路径辅助类
    /// </summary>
    public class PathFunctions
    {

        /// <summary>
        /// 获取 DLL 全路径
        /// </summary>
        /// <returns></returns>
        public static string GetCallDllFullPath()
        {
            return Path.Combine(GetCallDomainPath(), Path.GetFileName(Assembly.GetCallingAssembly().Location));
        }

        /// <summary>
        /// 获取 DLL 全名称
        /// </summary>
        /// <returns></returns>
        public static string GetCallDllFullName()
        {
            return Path.GetFileName(Assembly.GetCallingAssembly().Location);
        }



        /// <summary>
        /// 获取 DLL 全路径
        /// </summary>
        /// <typeparam name="T">要获取 DLL 全路径 内的随意 对象</typeparam>
        /// <returns></returns>
        public static string GetDllFullPath<T>()
        {
            return Path.Combine(GetCallDomainPath(), Path.GetFileName(Assembly.GetAssembly(typeof(T)).Location));
        }


        /// <summary>
        /// 获取 DLL 全名称
        /// </summary>
        /// <typeparam name="T">要获取 DLL 全路径 内的随意 对象</typeparam>
        /// <returns></returns>
        public static string GetDllFullName<T>()
        {
            return Path.GetFileName(Assembly.GetAssembly(typeof(T)).Location);
        }


        /// <summary>
        /// 获取程序域 根目录
        /// </summary>
        /// <returns></returns>
        public static string GetCallDomainPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 初始化目标路径文件夹 , 严格以路径是否有 文件后缀名 来区分 是文件路径 还是文件夹路径 , 可以是文件全路径 , 也可以是文件夹全路径
        /// </summary>
        /// <param name="path">可以是文件全路径 , 也可以是文件夹全路径</param>
        ///// <param name="isConfirmDirectoryPath">是否 自动 闭合 文件夹</param>
        public static void InitDirectoryPath(string path)
        {
            if (path.IfIsNullOrEmpty()) return;

            //if (isConfirmDirectoryPath)
            //{
            //    if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            //        path += Path.DirectorySeparatorChar;
            //}
            //else
            //{
            //    path = GetFolderPath(path);
            //}

            path = GetFolderPath(path);

            if (!path.IfIsNullOrEmpty() && !Directory.Exists(path) && path != Path.GetPathRoot(path))
                Directory.CreateDirectory(path);

        }

        /// <summary>
        /// 正确 获取 路径 文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFolderPath(string path)
        {

            if (path.IfIsNullOrEmpty()) return string.Empty;

            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                path += Path.DirectorySeparatorChar;

            return path;

        }


        /// <summary>
        /// 根据路径(绝对路径/相对路径)是否有文件扩展名 获取路径的类别 如 文件夹 路径 还是 文件路径 只是单纯解析路径  不效验 路径描述的文件夹 / 文件 是否真实存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PathTypeEnum GetPathType(string path)
        {

            PathTypeEnum result = PathTypeEnum.UnKnow;

            if (IsAbsolutePath(path) || IsRelativePath(path))
            {
                result = Path.HasExtension(path) ? PathTypeEnum.File : PathTypeEnum.Directory;
            }

            return result;

        }


        /// <summary>
        /// 效验绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAbsolutePath(string path)
        {
            return RegexFunctions.IsAbsolutePath(path);
        }

        /// <summary>
        /// 效验相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsRelativePath(string path)
        {
            return RegexFunctions.IsRelativePath(path);
        }



        /// <summary>
        /// 相对路径组装
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CombineRelativePath(params string[] path)
        {

            List<string> pathList = path.Select(s => s.Replace("\\", "/")).ToList();

            if (pathList.IfIsNullOrEmpty())
            {
                return "";
            }

            var paths = pathList[0].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            pathList.RemoveAt(0);

            foreach (string pathItem in pathList)
            {
                var pathItemTemp = pathItem;
                var tempPaths = pathItemTemp.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                while (true)
                {
                    if (pathItemTemp.StartsWith("../"))
                    {
                        tempPaths.RemoveAt(0);
                        pathItemTemp = pathItemTemp.Remove(0, 3);

                        if (paths.Count > 1)
                        {
                            paths.RemoveAt(paths.Count - 1);
                        }
                    }
                    else
                    {
                        break;
                    }
                }



                paths.AddRange(tempPaths);
            }

            string result = string.Join("/", paths);

            if (path[0].Replace("\\", "/").StartsWith("/"))
            {
                result = "/" + result;
            }

            return result;

        }

        /// <summary>
        /// 相对路径组装
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string CombineRelativePath(string path1, string path2)
        {
            return CombineRelativePath(new[] { path1, path2 });
        }

        /// <summary>
        /// 相对路径组装
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <returns></returns>
        public static string CombineRelativePath(string path1, string path2, string path3)
        {
            return CombineRelativePath(new[] { path1, path2, path3 });
        }


        /// <summary>
        /// 相对路径组装
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <param name="path4"></param>
        /// <returns></returns>
        public static string CombineRelativePath(string path1, string path2, string path3, string path4)
        {
            return CombineRelativePath(new[] { path1, path2, path3, path4 });
        }




        /// <summary>
        /// 从文件夹中获取文件信息 支持通配符过滤
        /// </summary>
        /// <param name="folderFullPath">文件夹全路径</param>
        /// <param name="dirSearchPatterns">通配符列表 默认值 null 为 提取所有文件信息 文件过滤通配符  如 : *.txt 只提取文件夹下所有txt文件 包括子目录</param>
        /// <returns></returns>
        public static List<string> GetFilesFromFolder(string folderFullPath, params string[] dirSearchPatterns)
        {

            List<string> files = new List<string>();


            if (!Directory.Exists(folderFullPath))
            {
                return files;
            }


            if (dirSearchPatterns.IfIsNullOrEmpty())
            {
                files.AddRange(Directory.GetFiles(folderFullPath, "*", SearchOption.AllDirectories));
            }
            else
            {
                foreach (var dirSearchPattern in dirSearchPatterns)
                {
                    files.AddRange(Directory.GetFiles(folderFullPath, dirSearchPattern, SearchOption.AllDirectories));
                }
            }


            return files.OrderBy(o => o).ToList();

        }


        /// <summary>
        /// 使用通配符 过滤匹配  路径信息
        /// </summary>
        /// <param name="paths">路径信息列表 相对路径 或 绝对路径</param>
        /// <param name="searchPatterns">通配符列表</param>
        /// <returns></returns>
        public static List<string> MatchFilesWithWildcard(List<string> paths, params string[] searchPatterns)
        {

            if (searchPatterns.IfIsNullOrEmpty())
            {
                return paths;
            }

            List<string> result = new List<string>();

            foreach (var path in paths)
            {

                if (CheckFileWithWildcard(path, searchPatterns))
                {
                    result.Add(path);
                }
            }

            return result;
        }

        /// <summary>
        /// 使用通配符 效验 路径 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPatterns"></param>
        /// <returns></returns>
        public static bool CheckFileWithWildcard(string path, params string[] searchPatterns)
        {
            if (path.IfIsNullOrEmpty() || searchPatterns.IfIsNullOrEmpty())
            {
                return true;
            }

            string fileName = GetFileName(path);

            if (fileName.IfIsNullOrEmpty())
            {
                return true;
            }

            foreach (var dirSearchPattern in searchPatterns)
            {
                if (RegexFunctions.CheckWithWildcard(fileName, dirSearchPattern))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取路径中 包含扩展名的 文件名  如果 文件名 无扩展名信息 返回空
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {

            if (path.IfIsNullOrEmpty() || !Path.HasExtension(path))
            {
                return "";
            }


            return Path.GetFileName(path);

        }


        /// <summary>
        /// 获取文件名非法字符 过滤后的 合法文件名
        /// </summary>
        /// <param name="fileFullName"></param>
        /// <returns></returns>
        public static string GetFileNameWithFilterInvalidFileNameChars(string fileFullName)
        {
            return new Regex("\\" + string.Join("|\\", Path.GetInvalidFileNameChars())).Replace(fileFullName, "");
        }


        /// <summary>
        /// 获取父级文件夹信息实体类
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static DirectoryInfo GetDirectoryParent(string path)
        {
            return Directory.GetParent(GetFolderPath(path));
        }


        /// <summary>
        /// 获取父级文件夹名称
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string GetDirectoryParentName(string path)
        {
            return GetDirectoryParent(path).Name;
        }


    }



}
