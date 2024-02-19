using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using MDR.Infrastructure.Extensions;
using NLog;
using NLog.Config;

namespace MDR.Infrastructure.Log.Implementation
{
    public class NLog4Logging : ILogging
    {
        public NLog4Logging(string configFilePath)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(configFilePath);
        }
        public NLog4Logging()
        { }


        readonly Lazy<ILogger> error = new(() => LogManager.GetLogger("Error"), true);
        readonly Lazy<ILogger> trace = new(() => LogManager.GetLogger("Trace"), true);
        readonly Lazy<ILogger> info = new(() => LogManager.GetLogger("Info"), true);
        readonly Lazy<ILogger> api = new(() => LogManager.GetLogger("Api"), true);
        readonly Lazy<ILogger> signalr = new(() => LogManager.GetLogger("Signalr"), true);
        readonly Lazy<ILogger> sql = new(() => LogManager.GetLogger("Sql"), true);

        private ConcurrentDictionary<string, ILogger> devices = new();
        private ConcurrentDictionary<string, ILogger> tasks = new();

        public void Error(object msg, Exception? ex = null)
        {
            string body = msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "";
            if (ex == null)
                error.Value.Error($"{body}");
            else
                error.Value.Error(ex, $"{body}");
        }

        public void Trace(object msg)
        {
            trace.Value.Trace(msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Info(object msg)
        {
            info.Value.Info(msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Api(object msg, LoggingLevel level = LoggingLevel.INFO)
        {
            api.Value.Log(mapToLogLevel(level), msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Signalr(object msg, LoggingLevel level = LoggingLevel.INFO)
        {
            signalr.Value.Log(mapToLogLevel(level), msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Sql(object msg, LoggingLevel level = LoggingLevel.INFO)
        {
            sql.Value.Log(mapToLogLevel(level), msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Device(object msg, string? deviceName, LoggingLevel level = LoggingLevel.INFO)
        {
            string loggerName = $"MDR.Device.{deviceName ?? "Api"}";
            var logger = devices.GetOrAdd(loggerName, LogManager.GetLogger);
            logger.Log(mapToLogLevel(level), msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }

        public void Task(object msg, TaskLogStatus status, LoggingLevel level = LoggingLevel.INFO)
        {
            string loggerName = $"MDR.Task.{status}";
            var logger = tasks.GetOrAdd(loggerName, LogManager.GetLogger);
            logger.Log(mapToLogLevel(level), msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString() ?? "");
        }


        /// <summary>
        /// 获取调用日志接口的方法全路径
        /// </summary>
        /// <returns></returns>
        private string? getCallPath()
        {
            MethodBase? method = new StackTrace(true).GetFrame(2)?.GetMethod();
            if (method != null)
                return $"{method.DeclaringType?.FullName?.Trim() ?? ""}.{method.Name}";
            return null;
        }

        private LogLevel mapToLogLevel(LoggingLevel level)
        {
            LogLevel target = level switch
            {
                LoggingLevel.TRACE => LogLevel.Trace,
                LoggingLevel.DEBUG => LogLevel.Debug,
                LoggingLevel.INFO => LogLevel.Info,
                LoggingLevel.WARN => LogLevel.Warn,
                LoggingLevel.Error => LogLevel.Error,
                LoggingLevel.FATAL => LogLevel.Fatal,
                _ => LogLevel.Off,
            };
            return target;
        }
    }
}