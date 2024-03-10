using System.Collections.Concurrent;
using System.Reflection;
using MDR.Infrastructure.Autofac.Extensions.Annotations;

namespace MDR.Infrastructure.Autofac.Extensions.Definitions;

/// <summary>
///     注册信息
/// </summary>
public class BeanDefinition
{
    /// <summary>
    ///     ctor
    /// </summary>
    public BeanDefinition()
    {
    }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="type">要注册的类型</param>
    public BeanDefinition(Type type)
    {
        CurrentType = type;
        Bean = new ComponentAttribute();
    }

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="type">要注册的类型</param>
    /// <param name="asType">注册成为的类型</param>
    public BeanDefinition(Type type, Type asType)
    {
        CurrentType = type;
        Bean = new ComponentAttribute(asType);
    }

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="type">要注册的类型</param>
    /// <param name="key">注册的key</param>
    public BeanDefinition(Type type, string key)
    {
        CurrentType = type;
        Bean = new ComponentAttribute(key);
    }

    /// <summary>
    ///     当前类型
    /// </summary>
    public Type? CurrentType { get; set; }

    /// <summary>
    ///     注册定义
    /// </summary>
    public ComponentAttribute? Bean { get; set; }

    /// <summary>
    ///     按照从小到大的顺序注册 如果同一个Type被处理多次会被覆盖！
    /// </summary>
    internal int OrderIndex { get; set; }

    /// <summary>
    ///     toString
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{CurrentType?.Namespace}.{CurrentType?.Name}({OrderIndex})";
    }
}