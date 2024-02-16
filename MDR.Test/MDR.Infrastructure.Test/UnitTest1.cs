using MDR.Infrastructure.Extensions;

namespace MDR.Infrastructure.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var demo = new { a = 123, b = "223" };
        Console.WriteLine(demo.ToJson());
    }
}