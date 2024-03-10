using Autofac.Core;
using Autofac.Core.Registration;
using MDR.Infrastructure.Autofac.Extensions.Annotations.Conditions;

namespace MDR.Infrastructure.Autofac.Extensions.Conditions;

/// <summary>
/// 条件里面配置的 没有被注册过才要添加到容器
/// </summary>
internal class OnMissingBean : ICondition
{
    /// <summary>
    /// true代表要过滤
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool ShouldSkip(IComponentRegistryBuilder context, object config)
    {
        if (config is not ConditionalOnMissingBeanAttribute metaConfig)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(metaConfig.Name))
        {
            //匹配name加类型
            return context.IsRegistered(new KeyedService(metaConfig.Name, metaConfig.Type!));
        }

        //只匹配类型
        return context.IsRegistered(new TypedService(metaConfig.Type!));
    }
}