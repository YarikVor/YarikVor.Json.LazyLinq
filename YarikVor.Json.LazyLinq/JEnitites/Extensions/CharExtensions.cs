using System.Runtime.CompilerServices;

namespace YarikVor.Json.LazyLinq.JEnitites.Extensions;

public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsExSpace(this char c)
    {
        return c is ' ' or '\t' or '\n' or '\r';
    }
}