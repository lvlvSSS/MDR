namespace MDR.Infrastructure.Log;

public interface ILogging
{
    void Trace(object msg);
    void Info(object msg);
    void Error(object msg, Exception? exception = null);
    void Api(object msg, LoggingLevel level = LoggingLevel.INFO);
    void Signalr(object msg, LoggingLevel level = LoggingLevel.INFO);
    void Sql(object msg, LoggingLevel level = LoggingLevel.INFO);
    void Device(object msg, string deviceName, LoggingLevel level = LoggingLevel.INFO);
    void Task(object msg, TaskLogStatus status, LoggingLevel level = LoggingLevel.INFO);
}

public enum TaskLogStatus
{
    /// <summary>
    /// 任务创建
    /// </summary>
    CREATE,
    /// <summary>
    /// 任务流转
    /// </summary>
    FLOW
}

public enum LoggingLevel
{
    TRACE,
    DEBUG,
    INFO,
    Error,
    WARN,
    FATAL
}
