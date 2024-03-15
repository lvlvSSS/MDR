using System.Net;
using System.Net.Sockets;

namespace MDR.Device.Api;

public abstract class DriverManager
{
    protected DriverManager()
    {
    }

    public static T? As<T>() where T : DriverManager
    {
        return Activator.CreateInstance(typeof(T), true) as T;
    }

    public static DriverManager? As(string typeName)
    {
        Type? type = Type.GetType(typeName);
        if (type == null)
        {
            return null;
        }

        return Activator.CreateInstance(type, true) as DriverManager;
    }

    /// <summary>
    /// 获取同设备的连接， 若 MDR 作为服务端，可以将 GetConnection 进行重写
    /// </summary>
    /// <param name="connectionName">连接名</param>
    /// <param name="connectionString">连接字符串</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">默认实现中，connectionString 不能为空值</exception>
    /// <exception cref="ArgumentException">解析地址错误</exception>
    public virtual Connection GetConnection(string connectionName, string? connectionString)
    {
        if (connectionString == null)
        {
            throw new ArgumentNullException($"{nameof(connectionString)} can not be empty or null");
        }

        var addresses = connectionString.Split(":", StringSplitOptions.RemoveEmptyEntries);
        if (addresses.Length is < 1 or > 2)
        {
            throw new ArgumentException("can't resolve connection string");
        }

        if (!IPAddress.TryParse(addresses[0], out var ip))
        {
            throw new ArgumentException("can't resolve ipv4 address");
        }

        // 默认情况下，使用80端口
        return GetDriverConnection(new IPEndPoint(ip, addresses.Length == 2 ? Convert.ToInt32(addresses[1]) : 80),
            connectionName);
    }

    protected abstract Connection GetDriverConnection(IPEndPoint ip, string connectionName);
}