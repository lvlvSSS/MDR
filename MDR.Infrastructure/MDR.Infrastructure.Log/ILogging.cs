namespace MDR.Infrastructure.Log;

public interface ILogging
{
    void Trace(object msg);
    void Error(object msg, Exception exception);
}
