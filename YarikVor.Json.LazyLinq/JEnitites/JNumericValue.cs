using System.Globalization;
using System.Numerics;
using YarikVor.Common.PrimitiveConverters;

namespace YarikVor.Json.LazyLinq.JEnitites;

public sealed class JNumericValue(int startIndex, int endIndex, string source) : JBase(startIndex, source)
{
    public override int SkipToNext()
    {
        return endIndex;
    }

    public ReadOnlySpan<char> GetSpan()
    {
        return SourceText.AsSpan(StartIndex, endIndex - StartIndex + 1);
    }

    public T GetValue<T>()
        where T : INumberBase<T>
    {
        return T.Parse(GetSpan(), CultureInfo.InvariantCulture);
    }

    public int GetInt32()
    {
        return PrimitiveConvertors.ParseInt32(GetSpan());
    }

    public int GetInt32T()
    {
        return int.Parse(GetSpan(), CultureInfo.InvariantCulture);
    }

    public Int16 GetInt16()
    {
        return PrimitiveConvertors.ParseInt16(GetSpan());
    }

    public Int64 GetInt64()
    {
        return PrimitiveConvertors.ParseInt64(GetSpan());
    }

    public Int128 GetInt128()
    {
        return PrimitiveConvertors.ParseInt128(GetSpan());
    }

    public byte GetByte()
    {
        return PrimitiveConvertors.ParseByte(GetSpan());
    }

    public UInt16 GetUInt16()
    {
        return PrimitiveConvertors.ParseUInt16(GetSpan());
    }

    public UInt32 GetUInt32()
    {
        return PrimitiveConvertors.ParseUInt32(GetSpan());
    }

    public UInt64 GetUInt64()
    {
        return PrimitiveConvertors.ParseUInt64(GetSpan());
    }

    public UInt128 GetUInt128()
    {
        return PrimitiveConvertors.ParseUInt128(GetSpan());
    }
}