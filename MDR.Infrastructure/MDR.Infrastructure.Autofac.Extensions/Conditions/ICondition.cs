using Autofac.Core.Registration;

namespace MDR.Infrastructure.Autofac.Extensions.Conditions;

/// <summary>
/// match
/// </summary>
public interface ICondition
{
    /// <summary>
    /// return true = skip Register
    /// </summary>
    bool ShouldSkip(IComponentRegistryBuilder context, object metadata);
}