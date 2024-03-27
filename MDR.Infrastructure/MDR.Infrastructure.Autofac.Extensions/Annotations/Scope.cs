namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

public class Scope(string scopeName = "singleton")
{
    public override string ToString() => scopeName;

    public static Scope Singleton = new("singleton");

    public static Scope Prototype = new("prototype");

    public static Scope Session = new("session");

    public static Scope Request = new("request");
}