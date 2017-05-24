///********************************************************************

//时间: 2015年10月22日, PM 01:23:50

//作者: lanyanmiyu@qq.com

//描述: 日志辅助类

//其它:     

//********************************************************************/



//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Lanymy.General.Extension.ExtensionFunctions;
//using Lanymy.General.Extension.Models;


//namespace Lanymy.General.Extension
//{

//    /// <summary>
//    /// 日志辅助类
//    /// </summary>
//    public class LogHelper : FileReadWriteHelper
//    {




//        /// <summary>
//        /// 日志文件全路径
//        /// </summary>
//        /// <param name="logFileFullPath"></param>
//        public LogHelper(string logFileFullPath)
//            : base(logFileFullPath)
//        {

//        }


//        /// <summary>
//        /// 写日志
//        /// </summary>
//        /// <param name="logMessageType"></param>
//        /// <param name="logMessage"></param>
//        public void WriteLog(LogMessageTypeEnum logMessageType, string logMessage)
//        {
//            WriteLine(string.Format("[{0}  {1}]  {2}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"), logMessageType, logMessage));
//        }



//    }


//}
