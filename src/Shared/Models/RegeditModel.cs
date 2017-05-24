/********************************************************************

时间: 2015年04月02日, AM 10:10:01

作者: lanyanmiyu@qq.com

描述: 注册表 实体类

其它:     

********************************************************************/



using System;

namespace Lanymy.General.Extension.Models
{
    /// <summary>
    /// 注册表 实体类
    /// </summary>
    [Serializable]
    public class RegeditModel
    {
        /// <summary>
        /// 注册表根键枚举
        /// </summary>
        public RegeditRootEnum RegeditRootEnum { get; set; }
        /// <summary>
        /// 注册表项 路径
        /// </summary>
        public string SubKey { get; set; }

        /// <summary>
        /// 键值Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 键值Value
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// 构造方法
        /// </summary>
        public RegeditModel() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="regeditRootEnum">注册表根键枚举</param>
        /// <param name="subKey">注册表项 路径</param>
        /// <param name="key">键值Key</param>
        public RegeditModel(RegeditRootEnum regeditRootEnum, string subKey, string key)
        {


            RegeditRootEnum = regeditRootEnum;
            SubKey = subKey;
            Key = key;


        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="regeditRootEnum">注册表根键枚举</param>
        /// <param name="subKey">注册表项 路径</param>
        /// <param name="key">键值Key</param>
        /// <param name="value">键值Value</param>
        public RegeditModel(RegeditRootEnum regeditRootEnum,
                            string subKey,
                            string key,
                            string value)
        {


            RegeditRootEnum = regeditRootEnum;
            SubKey = subKey;
            Key = key;
            Value = value;

        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="regeditRootEnum">注册表根键枚举</param>
        /// <param name="subKey">注册表项 路径</param>
        public RegeditModel(RegeditRootEnum regeditRootEnum, string subKey)
        {
            RegeditRootEnum = regeditRootEnum;
            SubKey = subKey;
        }


    }
}
