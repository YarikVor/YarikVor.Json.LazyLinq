using YarikVor.Json.LazyLinq.JEnitites.Queryable;

namespace YarikVor.Json.LazyLinq.JEnitites;

public static class JQueryableExtensions
{
    public static IQueryable<T> AsQueryable<T>(this JArray jArray)
    {
        return new JArrayQueryable<T>(jArray, new JArrayQueryProvider(jArray));
    }
}