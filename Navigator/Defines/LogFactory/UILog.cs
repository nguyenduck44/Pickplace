using System;
using System.Collections.Generic;
using log4net;
using log4net.Appender;
using System.Windows;

namespace NavigatorMachine.Defines
{
    public enum LogLevel
    {
        Debug = 0,
        Error = 1,
        Fatal = 2,
        Info = 3,
        Warning = 4
    }

    public static class UILog
    {
        #region Members
        private static readonly ILog _logger = LogManager.GetLogger("UILog");
        private static readonly Dictionary<LogLevel, Action<string>> _actions;
        #endregion

        static UILog()
        {
            LogFactory.Configure();
            _actions = new Dictionary<LogLevel, Action<string>>
            {
                { LogLevel.Debug, Debug },
                { LogLevel.Error, Error },
                { LogLevel.Fatal, Fatal },
                { LogLevel.Info, Info },
                { LogLevel.Warning, Warning }
            };
        }

        public static NotifyAppender Appender
        {
            get
            {
                try
                {
                    foreach (ILog log in LogManager.GetCurrentLoggers())
                    {
                        foreach (IAppender appender in log.Logger.Repository.GetAppenders())
                        {
                            if (appender is NotifyAppender notifyAppender)
                            {
                                return notifyAppender;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.Error("Error retrieving NotifyAppender", ex);
                    return null;
                }
            }
        }

        public static void Write(LogLevel level, string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            if (level < LogLevel.Debug || level > LogLevel.Warning)
                throw new ArgumentOutOfRangeException(nameof(level));

            _actions[level](message);
        }

        #region Action methods
        public static void Debug(string message)
        {
            if (_logger.IsDebugEnabled)
                _logger.Debug(message);
        }

        public static void Error(string message)
        {
            if (_logger.IsErrorEnabled)
                _logger.Error(message);
        }


        public static void Fatal(string message)
        {
            if (_logger.IsFatalEnabled)
                _logger.Fatal(message);
        }

        public static void Info(string message)
        {
            if (_logger.IsInfoEnabled)
                _logger.Info(message);
        }

        public static void Warning(string message)
        {
            if (_logger.IsWarnEnabled)
                _logger.Warn(message);
        }
        #endregion
    }
}
