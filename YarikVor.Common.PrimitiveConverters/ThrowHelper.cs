using System.Diagnostics.CodeAnalysis;

namespace YarikVor.Json.LazyLinq.JEnitites;

public static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowFormat(string message)
    {
        throw Format(message);
    }

    [DoesNotReturn]
    public static FormatException Format(string message)
    {
        return new FormatException(message);
    }

    [DoesNotReturn]
    public static void Overflow(string message)
    {
        throw new OverflowException(message);
    }
}