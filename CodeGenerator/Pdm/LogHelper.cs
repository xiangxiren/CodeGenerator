using System;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace CodeGenerator.Pdm
{
    public class LogHelper
    {
       static readonly object Syn = new object();

       private LogHelper() { }

        /// <summary>
        /// 输出Info类信息 PengeSoft.Logging.LogLevel.Info
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void Info(object obj, object message)
        {
            Info(obj, message, null);
        }

        /// <summary>
        /// 输出Info类信息 PengeSoft.Logging.LogLevel.Info， 并接收异常做为参数。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(object obj, object message, Exception exception)
        {
            lock (Syn)
            {
                ILog log = LogManager.GetLogger(obj == null ? typeof(LogHelper) : obj.GetType());
                if (log != null)
                    log.Info(message, exception);
            }
        }
        
        /// <summary>
        /// 输出Debug类信息 PengeSoft.Logging.LogLevel.Debug
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void Debug(object obj, object message)
        {
            Debug(obj, message, null); 
        }

        /// <summary>
        /// 输出Debug类信息 PengeSoft.Logging.LogLevel.Debug， 并接收异常做为参数。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(object obj, object message, Exception exception)
        {
            lock (Syn)
            {
                ILog log = LogManager.GetLogger(obj == null ? typeof(LogHelper) : obj.GetType());
                if (log != null)
                    log.Debug(message, exception);
            }
        }

        /// <summary>
        /// 输出Error类信息 PengeSoft.Logging.LogLevel.Error
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void Error(object obj, object message)
        {
            Error(obj, message, null);
        }

        /// <summary>
        /// 输出Error类信息 PengeSoft.Logging.LogLevel.Error， 并接收异常做为参数。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(object obj, object message, Exception exception)
        {
            lock (Syn)
            {
                ILog log = LogManager.GetLogger(obj == null ? typeof(LogHelper) : obj.GetType());
                if (log != null)
                    log.Error(message, exception);
            }
        }

        /// <summary>
        /// 输出Fatal类信息 PengeSoft.Logging.LogLevel.Fatal
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void Fatal(object obj, object message)
        {
            Fatal(obj, message, null);
        }

        /// <summary>
        /// 输出Fatal类信息 PengeSoft.Logging.LogLevel.Fatal， 并接收异常做为参数。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(object obj, object message, Exception exception)
        {
            lock (Syn)
            {
                ILog log = LogManager.GetLogger(obj == null ? typeof(LogHelper) : obj.GetType());
                if (log != null)
                    log.Fatal(message, exception);
            }
        }

        /// <summary>
        /// 输出Warn类信息 PengeSoft.Logging.LogLevel.Warn
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        public static void Warn(object obj, object message)
        {
            Warn(obj, message, null);
        }

        /// <summary>
        /// 输出Warn类信息 PengeSoft.Logging.LogLevel.Warn， 并接收异常做为参数。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(object obj, object message, Exception exception)
        {
            lock (Syn)
            {
                ILog log = LogManager.GetLogger(obj == null ? typeof(LogHelper) : obj.GetType());
                if (log != null)
                    log.Warn(message, exception);
            }
        }
    }
}