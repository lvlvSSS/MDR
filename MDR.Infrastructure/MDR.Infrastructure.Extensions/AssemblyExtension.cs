using System.Reflection;
using System.Text.RegularExpressions;

namespace MDR.Infrastructure.Extensions;

public sealed class AssemblyExtension
{
    /// <summary>
    /// 默认的跳过的程序集
    /// </summary>
    const string DEFAULT_SKIP_ASSEMBLIES = "^Mscorlib|^Netstandard|^Microsoft|^Autofac|" +
                                           "^AutoMapper|^EntityFramework|^Newtonsoft|^Castle|^NLog|" +
                                           "^Pomelo|^AspectCore|^Xunit|^Nito|^Npgsql|^Exceptionless|" +
                                           "^MySqlConnector|^Anonymously Hosted|^libuv|^api-ms|^clrcompression|" +
                                           "^clretwrc|^clrjit|^coreclr|^dbgshim|^e_sqlite3|^hostfxr|^hostpolicy|" +
                                           "^MessagePack|^mscordaccore|^mscordbi|^mscorrc|sni|sos|SOS.NETCore|" +
                                           "^sos_amd64|^SQLitePCLRaw|^StackExchange|^Swashbuckle|^WindowsBase|" +
                                           "^ucrtbase|^Serilog|^JetBrains|^Spring.EL|^EnyimMemcachedCore|" +
                                           "^HealthChecks|^aspnetcorev|^Elasticsearch|^MySql|^Z.EntityFramework|" +
                                           "^Z.Expressions.Eval|^MediatR|^Devart|^Dapper|^NrClientSDK|" +
                                           "^Quartz|^NHapi|^Log4net|^Interop.MSScriptControl|" +
                                           "^Common.Log|^Owin|^UAParser|^TopRafters42|^SQLite.Interop";

    /// <summary>
    /// 获取匹配的程序集列
    /// </summary>
    public static List<Assembly> GetMatchAssemblies(string? extendSkipAssemblies = null)
    {
        return GetAssembliesFromCurrentAppDomain(true, extendSkipAssemblies);
    }

    /// <summary>
    /// 从当前应用程序域获取程序集列表
    /// </summary>
    /// <param name="skip">是否需要过滤程序集</param>
    /// <param name="extendSkipAssemblies">扩展默认的过滤的程序集</param>
    /// <returns></returns>
    private static List<Assembly> GetAssembliesFromCurrentAppDomain(
        bool skip = true,
        string? extendSkipAssemblies = null)
    {
        var result = new List<Assembly>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (skip && Skip(assembly, extendSkipAssemblies))
            {
                result.Add(assembly);
            }
            else
            {
                result.Add(assembly);
            }
        }

        return result.Distinct().ToList();
    }

    /// <summary>
    /// 获取当前程序集中所有程序集 包括 默认的过滤掉的程序集
    /// </summary>
    /// <returns></returns>
    public static List<Assembly> GetAllAssemblies()
    {
        return GetAssembliesFromCurrentAppDomain(false);
    }

    /// <summary>
    /// 加载程序集到当前应用程序域
    /// </summary>
    /// <param name="path">目录绝对路径</param>
    /// <param name="extendSkipAssemblies"></param>
    public static List<Assembly> LoadAssemblies(string path, string? extendSkipAssemblies = null)
    {
        var ass = new List<Assembly>();
        foreach (var file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
        {
            if (!Skip(Path.GetFileName(file), extendSkipAssemblies))
                continue;
            try
            {
                var assembly = Assembly.LoadFrom(file);
                ass.Add(assembly);
            }
            catch (Exception e)
            {
                var ex = new Exception($"load {file} error：{e.Message} , \nstack trace：{e.StackTrace}");
                throw ex;
            }
        }

        return ass;
    }

    /// <summary>
    /// 程序集是否匹配
    /// </summary>
    private static bool Skip(string assemblyName, string? extendSkipAssemblies = null)
    {
        var skipAssemblies = DEFAULT_SKIP_ASSEMBLIES +
                             (string.IsNullOrWhiteSpace(extendSkipAssemblies) ? "" : $"|{extendSkipAssemblies}");
        return !Regex.IsMatch(assemblyName, skipAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    /// <summary>
    /// 程序集是否匹配
    /// </summary>
    private static bool Skip(Assembly assembly, string? extendSkipAssemblies = null)
    {
        var skipAssemblies = DEFAULT_SKIP_ASSEMBLIES +
                             (string.IsNullOrWhiteSpace(extendSkipAssemblies) ? "" : $"|{extendSkipAssemblies}");
        return !Regex.IsMatch(assembly.FullName!, skipAssemblies, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static List<Assembly> LoadFiles(IEnumerable<string> files)
    {
        var assemblies = new List<Assembly>();
        foreach (var file in files)
        {
            var name = new AssemblyName(file);
            try
            {
                assemblies.Add(Assembly.Load(name));
            }
            catch (FileNotFoundException)
            {
            }
        }

        return assemblies;
    }
}