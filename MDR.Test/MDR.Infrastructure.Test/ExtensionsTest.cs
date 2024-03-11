using MDR.Infrastructure.Extensions;

namespace MDR.Infrastructure.Test;

public class ExtensionsTest
{
    [Fact]
    public void TestDeepClone()
    {
        var person = new Person() { Name = "123", Age = 12 };
        var personClone = person.DeepClone();

        Assert.Equal(person.Age, personClone!.Age);
        Assert.Equal(person.Name, personClone!.Name);
        Assert.NotEqual(person, personClone);
    }

    public class Person
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }
}