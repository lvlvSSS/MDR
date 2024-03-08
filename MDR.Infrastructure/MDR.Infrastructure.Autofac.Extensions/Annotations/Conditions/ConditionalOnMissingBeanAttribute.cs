using MDR.Infrastructure.Autofac.Extensions.Conditions;

namespace MDR.Infrastructure.Autofac.Extensions.Annotations.Conditions;

/// <summary>
/// 只能打在标有Bean的方法上面
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class ConditionalOnMissingBeanAttribute : ConditionalAttribute
{
    /// <summary>
    /// ctor
    /// </summary>
    public ConditionalOnMissingBeanAttribute()
    {
        this.Type = typeof(OnMissingBean);
    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type"></param>
    public ConditionalOnMissingBeanAttribute(Type type) : this()
    {
        this.Type = type;
    }


    /// <summary>
    /// ctor
    /// </summary>
    public ConditionalOnMissingBeanAttribute(Type type, string name) : this(type)
    {
        this.Name = name;
    }


    /// <summary>
    /// keyname
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 类型判断
    /// </summary>
    public Type Type { get; set; }
}