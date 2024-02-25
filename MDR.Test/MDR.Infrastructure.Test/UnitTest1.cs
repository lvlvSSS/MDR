using System.Reflection;
using MDR.Infrastructure.Extensions;
using MDR.Infrastructure.Resource;

namespace MDR.Infrastructure.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var demo = new { a = 123, b = "223" };
        Console.WriteLine(Resource.Properties.Resource.AnyRadixConvert_CharacterIsNotValid);
        Console.WriteLine(demo.ToJson());
    }
}