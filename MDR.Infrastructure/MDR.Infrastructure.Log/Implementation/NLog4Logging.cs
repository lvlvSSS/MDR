using MDR.Infrastructure.Extensions;
using NLog;

namespace MDR.Infrastructure.Log.Implementation
{
    public class NLog4Logging : ILogging
    {
        public NLog4Logging()
        {
            LogManager.Setup(manager => manager.LoadConfigurationFromFile());
        }

        readonly Lazy<ILogger> error = new(() => LogManager.GetLogger("error"), true);
        readonly Lazy<ILogger> trace = new(() => LogManager.GetLogger("trace"), true);
        public void Error(object msg, Exception ex)
        {
            if (msg is string)
            {
                error.Value.Error(ex, msg as string);
                return;
            }
            error.Value.Error(ex, $"\n{msg.ToJson()}\n");
        }

        public void Trace(object msg)
        {
            if (msg is string)
            {
                trace.Value.Trace(msg);
                return;
            }
            trace.Value.Trace($"\n{msg.ToJson()}\n");
        }
    }
}