using MDR.Infrastructure.Extensions;
using Xunit.Abstractions;

namespace MDR.Infrastructure.Test;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Test1()
    {
        var demo = new { a = 123, b = "223" };
        testOutputHelper.WriteLine(Resource.Properties.Resource.AnyRadixConvert_CharacterIsNotValid);
        testOutputHelper.WriteLine(demo.ToJson());
    }
}