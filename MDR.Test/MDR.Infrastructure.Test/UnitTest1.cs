using MDR.Infrastructure.Extensions;
using MDR.Infrastructure.LocalizeResource;
using Xunit.Abstractions;

namespace MDR.Infrastructure.Test;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Test1()
    {
        var demo = new { a = 123, b = "223" };
        testOutputHelper.WriteLine(string.Format(SharedResource.AnyRadixConvert_CharacterIsNotValid, "aaa", "bbb"));
        testOutputHelper.WriteLine(demo.ToJson());
    }
}