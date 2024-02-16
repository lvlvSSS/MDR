using Newtonsoft.Json;

namespace MDR.Infrastructure.Extensions;

public static class ObjectExtension
{
    /// <summary>
    /// json 序列化
    /// </summary>
    /// <param name="o"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static string ToJson(this object o, JsonSerializerSettings? settings = null)
    {
        settings ??= new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        return JsonConvert.SerializeObject(o, settings);

    }
}
