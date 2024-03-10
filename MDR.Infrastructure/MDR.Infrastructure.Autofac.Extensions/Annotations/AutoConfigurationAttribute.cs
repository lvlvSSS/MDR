namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

/// <summary>
/// 和Spring的Configuration功能类似
/// Bean标签在这个类里面才有作用
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoConfigurationAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public AutoConfigurationAttribute()
    {
        this.Key = "";
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="key"></param>
    public AutoConfigurationAttribute(string key)
    {
        this.Key = key;
    }

    /// <summary>
    /// 注册单个的key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 值越大越优先处理
    /// </summary>
    public int OrderIndex { get; set; }


    /// <summary>
    /// 被创建后执行的方法
    /// </summary>
    public string? InitMethod { get; set; }

    /// <summary>
    /// 被Release时执行的方法
    /// </summary>
    public string? DestroyMethod { get; set; }
}

internal class AutoConfigurationInfo
{
    public AutoConfigurationAttribute? AutoConfiguration { get; set; }
    public Type? Type { get; set; }
    public string? Key { get; set; }
    public int OrderIndex { get; set; }
}