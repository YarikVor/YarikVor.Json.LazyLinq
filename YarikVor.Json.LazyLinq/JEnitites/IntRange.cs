using System.Diagnostics;

namespace YarikVor.Json.LazyLinq.JEnitites;

public readonly struct IntRange
{
    public readonly int Start;
    public readonly int End;

    public IntRange(int start, int end)
    {
        Debug.Assert(end >= start);
        Start = start;
        End = end;
    }
    
    public int Length => End - Start;
    
    public bool IsEmpty => Start == End;
}