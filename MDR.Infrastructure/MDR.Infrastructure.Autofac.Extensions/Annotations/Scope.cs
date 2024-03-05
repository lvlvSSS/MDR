namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

public class Scope(string scopeName = "singleton")
{
    public override string ToString() => scopeName;

    public static Scope Singleton = new Scope();

    public static Scope Prototype = new Scope("prototype");

    public static Scope Lifetime = new Scope("lifetime");
}