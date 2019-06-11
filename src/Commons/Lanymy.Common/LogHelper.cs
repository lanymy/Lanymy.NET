using Lanymy.Common.Instruments.Logger;
using Lanymy.Common.Interfaces;

namespace Lanymy.Common
{
    /// <summary>
    /// 日志辅助类
    /// </summary>
    public class LogHelper
    {


        private static readonly object _Locker = new object();


        #region log4net 内核

        private static ILogger _FileLogger = null;


        /// <summary>
        /// 文件模式 日志
        /// </summary>
        public static ILogger FileLoggerInstance()
        {

            if (null == _FileLogger)
            {
                lock (_Locker)
                {
                    if (null == _FileLogger)
                    {
                        _FileLogger = new Log4NetLogger(GlobalSettings.Log4netConfigFileFullPath, LoggerTypeEnum.FileLogger.ToString());
                    }
                }
            }

            return _FileLogger;

        }



        #endregion



        #region NLog 内核


        private static ILogger _NLogFileLogger = null;


        /// <summary>
        /// NLog 文件模式 日志
        /// </summary>
        public static ILogger NLogFileLoggerInstance()
        {

            if (null == _NLogFileLogger)
            {
                lock (_Locker)
                {
                    if (null == _NLogFileLogger)
                    {
                        _NLogFileLogger = new NLogLogger(GlobalSettings.NLogConfigFileFullPath, LoggerTypeEnum.FileLogger.ToString());
                    }
                }
            }

            return _NLogFileLogger;

        }

        private static ILogger _NLogDataBaseLogger = null;


        /// <summary>
        /// NLog 文件模式 日志
        /// </summary>
        public static ILogger NLogDataBaseLoggerInstance()
        {

            if (null == _NLogDataBaseLogger)
            {
                lock (_Locker)
                {
                    if (null == _NLogDataBaseLogger)
                    {
                        _NLogDataBaseLogger = new NLogLogger(GlobalSettings.NLogConfigFileFullPath, LoggerTypeEnum.DataBaseLogger.ToString());
                    }
                }
            }

            return _NLogDataBaseLogger;

        }


        #endregion



    }

}
