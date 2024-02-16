namespace MDR.Device.Api;

public abstract class DriverManager
{
    protected DriverManager() { }

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
}

