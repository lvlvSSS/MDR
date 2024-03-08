namespace MDR.Infrastructure.Autofac.Extensions.Annotations;

/// <summary>
///     拦截器类型
/// </summary>
public enum InterceptorType
{
    /// <summary>
    ///     使用接口模式 自己指定拦截器
    /// </summary>
    Interface,

    /// <summary>
    ///     使用class的虚方法模式 自己指定拦截器
    /// </summary>
    Class
}