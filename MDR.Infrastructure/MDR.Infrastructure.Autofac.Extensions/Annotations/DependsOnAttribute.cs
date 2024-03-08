using MDR.Infrastructure.Autofac.Extensions.Util;

namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

/// <summary>
/// DependsOn标签 配合Bean和Component标签使用
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DependsOnAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public DependsOnAttribute(params Type[] types) : this()
    {
        this.DependsOn = types;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public DependsOnAttribute(params string[] types) : this()
    {
        this.DependsTypes = types;
    }

    /// <summary>
    /// ctor
    /// </summary>
    public DependsOnAttribute()
    {
        DependsOnLazy = new Lazy<Type[]>(getDependsOn);
    }

    /// <summary>
    /// 依赖的 是用来表示一个bean A的实例化依赖另一个bean B的实例化， 但是A并不需要持有一个B的对象
    /// </summary>
    public Type[]? DependsOn { get; set; }

    /// <summary>
    /// 依赖的 是用来表示一个bean A的实例化依赖另一个bean B的实例化
    /// 类的完全路径数组
    /// </summary>
    public String[]? DependsTypes { get; set; }

    /// <summary>
    ///     依赖的 是用来表示一个bean A的实例化依赖另一个bean B的实例化， 但是A并不需要持有一个B的对象
    /// </summary>
    internal readonly Lazy<Type[]> DependsOnLazy;

    /// <summary>
    /// 解析配置的depends
    /// </summary>
    /// <returns></returns>
    private Type[] getDependsOn()
    {
        var list = new List<Type>();
        if (DependsOn != null && DependsOn.Any())
        {
            list.AddRange(DependsOn);
        }

        if (DependsTypes != null && DependsTypes.Any())
        {
            list.AddRange(DependsTypes.Select(type => type.FindClassIgnoreErr()).Where(temp => temp != null));
        }

        return list.Distinct().ToArray();
    }
}