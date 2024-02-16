namespace MDR.Infrastructure.Extensions;

public static class EnumerableExtension
{
    public static IEnumerable<T> Concat<T>(T item1, IEnumerable<T> rest)
    {
        yield return item1;
        foreach (var other in rest)
        {
            yield return other;
        }
    }
}
