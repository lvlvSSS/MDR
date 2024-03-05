using System.Reflection;
using Autofac;
using MDR.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Module = Autofac.Module;

namespace MDR.Server.Startups;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 配置所有控制器均支持属性注入
        foreach (Assembly assembly in AssemblyExtension.GetMatchAssemblies())
        {
            var types = assembly.ExportedTypes.Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToArray();
            if (!types.Any())
                continue;
            builder.RegisterTypes(types.ToArray()).PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}