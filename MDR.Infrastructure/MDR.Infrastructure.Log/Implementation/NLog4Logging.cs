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

        public NLog4Logging() { }

        readonly Lazy<ILogger> error = new(() => LogManager.GetLogger("error"), true);
        readonly Lazy<ILogger> trace = new(() => LogManager.GetLogger("trace"), true);
        public void Error(object msg, Exception ex)
        {
            ILogger logger = error.Value;
            MethodBase? method = new StackTrace(true).GetFrame(1)?.GetMethod();

            if (method == null)
            {
                logger.Error(ex, msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString());
                return;
            }

            logger = LogManager.GetLogger($"{method.DeclaringType?.FullName?.Trim() ?? ""}.{method.Name}");
            logger.Error(ex, msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString());
        }

        public void Trace(object msg)
        {
            ILogger logger = trace.Value;
            MethodBase? method = new StackTrace(true).GetFrame(1)?.GetMethod();

            if (method == null)
            {
                logger.Trace(msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString());
                return;
            }
            logger = LogManager.GetLogger($"{method.DeclaringType?.FullName?.Trim() ?? ""}.{method.Name}");
            logger.Trace(msg.GetType().IsClass && (msg is not string) ? $"\n{msg.ToJson()}" : msg.ToString());
        }
    }
}