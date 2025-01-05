namespace YarikVor.Json.LazyLinq;

public static class LazyLinqExtensions
{
    public static IEnumerable<T> SelectGenerate<T>(this IEnumerable<LazyClass<T>> source)
    {
        return source.Select(x => x.Generate());
    }
}