using Newtonsoft.Json;

namespace MDR.Infrastructure.Extensions;

public static class StringExtension
{
    public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Formatting = Formatting.Indented };
    /// <summary>
    /// 解析JSON字符串生成对象实体
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json字符串</param>
    /// <returns>对象实体</returns>
    public static T? FromJson<T>(this string json, JsonSerializerSettings? settings = null) where T : class
    {
        if (settings != null)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        return JsonConvert.DeserializeObject(json, typeof(T)) as T;
    }

    /// <summary>
    /// 解析JSON数组生成对象实体集合
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="json">json数组字符串</param>
    /// <returns>对象实体集合</returns>
    public static List<T>? FromJsonList<T>(this string json, JsonSerializerSettings? settings = null) where T : class
    {
        if (settings != null)
        {
            return JsonConvert.DeserializeObject<List<T>>(json, settings);
        }
        return JsonConvert.DeserializeObject(json, typeof(List<T>)) as List<T>;
    }

    /// <summary>
    /// 反序列化JSON到给定的匿名对象
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <param name="anonymousTypeObject">匿名对象</param>
    /// <param name="settings"></param>
    /// <typeparam name="T">匿名对象类型</typeparam>
    /// <returns></returns>
    public static T? FromJson<T>(this string json, T anonymousTypeObject, JsonSerializerSettings? settings = null)
    {
        if (settings != null)
        {
            return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject, settings);
        }
        return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
    }

    /// <summary>
    /// 转半角的函数(DBC case)
    ///
    /// 任意字符串
    /// 半角字符串
    /// 
    /// 全角空格为12288，半角空格为32
    /// 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    public static string ToDBC(this string src)
    {
        char[] c = src.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c);
    }
}
