namespace MDR.Infrastructure.Autofac.Extensions.Annotations.Conditions;

/// <summary>
/// attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class ConditionalAttribute : Attribute
{
    /// <summary>
    /// ctor
    /// </summary>
    public ConditionalAttribute()
    {
    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type"></param>
    public ConditionalAttribute(Type type)
    {
        this.Type = type;
    }

    /// <summary>
    /// type必须是继承了Condition的接口
    /// </summary>
    public Type Type { get; set; }
}