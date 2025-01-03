namespace Laminar.Domain.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Yield<T>(this T single)
    {
        yield return single;
    }
}
