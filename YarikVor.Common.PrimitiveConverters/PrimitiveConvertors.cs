using YarikVor.Json.LazyLinq.JEnitites;
namespace YarikVor.Common.PrimitiveConverters;


public static partial class PrimitiveConvertors
{
 
    public static SByte ParseSByte(ReadOnlySpan<char> source)
    {
        var neg = source[0] == '-';
        var num = 0;
        var span = source[(neg ? 1 : 0)..];
        foreach (var c in span)
        {
            var minus = c - '0';
            if (minus is < 0 or > 9)
                ThrowHelper.ThrowFormat("");
            num = num * 10 + minus;
            if (neg && num is > SByte.MinValue
                || !neg && num is > SByte.MinValue)
                ThrowHelper.Overflow("");
        }
        return neg ? (SByte)(-num) : (SByte)num;
    }

    public static Int16 ParseInt16(ReadOnlySpan<char> source)
    {
        var neg = source[0] == '-';
        var num = 0;
        var span = source[(neg ? 1 : 0)..];
        foreach (var c in span)
        {
            var minus = c - '0';
            if (minus is < 0 or > 9)
                ThrowHelper.ThrowFormat("");
            num = num * 10 + minus;
            if (neg && num is > Int16.MinValue
                || !neg && num is > Int16.MinValue)
                ThrowHelper.Overflow("");
        }
        return neg ? (Int16)(-num) : (Int16)num;
    }

   

    public static Int32 ParseInt32(ReadOnlySpan<char> source)
    {
        var neg = source[0] == '-';
        Int32 num = 0;
        var span = source[(neg ? 1 : 0)..];
        foreach (var c in span)
        {
            var minus = (c - '0');
            if (minus is < 0 or > 9)
                ThrowHelper.ThrowFormat("");
            checked {
                num = num * 10 + minus;
            }
        }
        return neg ? -num : num;
    }
    public static Int64 ParseInt64(ReadOnlySpan<char> source)
    {
        var neg = source[0] == '-';
        Int64 num = 0;
        var span = source[(neg ? 1 : 0)..];
        foreach (var c in span)
        {
            var minus = c - '0';
            if (minus is < 0 or > 9)
                ThrowHelper.ThrowFormat("");
            checked {
                num = num * 10 + minus;
            }
        }
        return neg ? -num : num;
    }
    public static Int128 ParseInt128(ReadOnlySpan<char> source)
    {
        var neg = source[0] == '-';
        Int128 num = 0;
        var span = source[(neg ? 1 : 0)..];
        foreach (var c in span)
        {
            var minus = c - '0';
            if (minus is < 0 or > 9)
                ThrowHelper.ThrowFormat("");
            checked {
                num = num * 10 + minus;
            }
        }
        return neg ? -num : num;
    }
   

    public static Byte ParseByte(ReadOnlySpan<char> source)
    {
        uint num = 0;
        foreach (var c in source)
        {
            var minus = (uint)(c - '0');
            if (minus > 9)
                ThrowHelper.ThrowFormat("");
            num = num * 10 + minus;
            
            if (num > Byte.MaxValue)
                ThrowHelper.Overflow("");
        }
        return (Byte)num;
    }
    public static UInt16 ParseUInt16(ReadOnlySpan<char> source)
    {
        uint num = 0;
        foreach (var c in source)
        {
            var minus = (uint)(c - '0');
            if (minus > 9)
                ThrowHelper.ThrowFormat("");
            num = num * 10 + minus;
            
            if (num > UInt16.MaxValue)
                ThrowHelper.Overflow("");
        }
        return (UInt16)num;
    }
   

    public static UInt32 ParseUInt32(ReadOnlySpan<char> source)
    {
        UInt32 num = 0;
        foreach (var c in source)
        {
            var minus = (uint)(c - '0');
            if (minus > 9)
                ThrowHelper.ThrowFormat("");
            var unsigned = (UInt32)minus;
            checked {
                num = num * 10 + unsigned;
            }
        }
        return num;
    }
    public static UInt64 ParseUInt64(ReadOnlySpan<char> source)
    {
        UInt64 num = 0;
        foreach (var c in source)
        {
            var minus = (uint)(c - '0');
            if (minus > 9)
                ThrowHelper.ThrowFormat("");
            var unsigned = (UInt64)minus;
            checked {
                num = num * 10 + unsigned;
            }
        }
        return num;
    }
    public static UInt128 ParseUInt128(ReadOnlySpan<char> source)
    {
        UInt128 num = 0;
        foreach (var c in source)
        {
            var minus = (uint)(c - '0');
            if (minus > 9)
                ThrowHelper.ThrowFormat("");
            var unsigned = (UInt128)minus;
            checked {
                num = num * 10 + unsigned;
            }
        }
        return num;
    }
   


}