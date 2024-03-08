namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

/// <summary>
///     自动注册属性类型
/// </summary>
public enum InjectPropertyType
{
    /// <summary>
    ///     代表打了Autowired标签的才会装配
    /// </summary>
    Autowired,

    /// <summary>
    ///     代表全部自动装配
    /// </summary>
    ALL
}