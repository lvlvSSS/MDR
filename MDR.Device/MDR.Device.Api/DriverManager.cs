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

    public Connection GetConnection(string ipv4, int port)
    {
        if (!IPAddress.TryParse(ipv4, out IPAddress? address))
        {
            throw new ArgumentException("can't resolve ipv4 address");
        }

        return new Connection(new IPEndPoint(address, port));
    }
}