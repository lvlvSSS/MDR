using System.Reflection;
using Autofac;
using MDR.Infrastructure.Extensions;
using Module = Autofac.Module;

namespace MDR.Infrastructure.Autofac.Extensions.Modules;

/// <summary>
/// 
/// </summary>
public class AnnotationModule : Module
{
    public AnnotationModule() : this(AssemblyExtension.LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory).ToArray())
    {
    }

    public AnnotationModule(params Assembly[] assemblies)
    {
        _assemblies = new();
        if (assemblies.Length == 0) return;
        _assemblies.AddRange(assemblies);
    }

    private readonly List<Assembly> _assemblies;


    protected override void Load(ContainerBuilder builder)
    {
        // 这里添加 对注解的类 利用Autofac进行注册
    }
}