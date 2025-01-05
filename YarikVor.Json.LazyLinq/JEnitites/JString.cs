using System.Diagnostics;
using System.Text;

namespace YarikVor.Json.LazyLinq.JEnitites;


/*
 * Ex:
 *          "text   text"
 *    start ^ 0      12 ^ end
 *    [start, end]
 */
[DebuggerDisplay("{GetType().Name,nq}: {GetValue(),nq}")]
public sealed class JString(int startIndex, string source) : JBase(startIndex, source)
{
    private string? _value;
    private int _endIndex = -1;
    private bool FastCompare => IsObjectNullValue && ReadToEnd;
    
    public bool IsObjectNullValue => ReferenceEquals(_value, JConstants.ObjectNullString);
    private bool ReadToEnd => _endIndex != -1;

    public bool HasValue => _value is not null && !IsObjectNullValue;

    public string GetValue()
    {
        if (HasValue)
            return _value!;
        
        UpdateString();

        if (IsObjectNullValue)
        {
            _value = SourceText.AsSpan(StartBody, _endIndex - StartBody).ToString();
        }
        return _value;
    }

    public void UpdateString()
    {
        int index = StartBody;
        for (;; ++index)
        {
            var c = SourceText[index];

            if (c is '"')
            {
                _endIndex = index;
                _value = JConstants.ObjectNullString;
                return;
            }

            if (c is '\\')
            {
                break;
            }
        }

        var lastIndex = 0;
        var sb = new StringBuilder();
        
        for(;; ++index)
        {
            var c = SourceText[index];
            if (c is '\\')
            {
                sb.Append(SourceText.AsSpan(lastIndex, index - lastIndex));
                ++index;

                // TODO:
                lastIndex = index + 1;
                continue;
            }

            if (c is '"')
            {
                sb.Append(SourceText.AsSpan(lastIndex, index - lastIndex));
                _value = sb.ToString();
                return;
            }

            throw new Exception();
        }
    }

    public override int SkipToNext()
    {
        if (_endIndex == -1)
            _endIndex = GetEndString();
        return _endIndex;
    }

    private int GetEndString()
    {
        for (int i = StartBody;; i++)
        {
            var c = SourceText[i];
            if (c is '\\')
            {
                ++i;
            }
            else if (c is '"')
            {
                return i;
            }
        }
    }

    public bool Equals(string other)
    {
        if (!ReadToEnd)
        {
            UpdateString();
        }
        if (FastCompare)
        {
            return SourceText.AsSpan(StartBody, _endIndex - StartBody).SequenceEqual(other);
        }
        else
        {
            return GetValue() == other;
        }
    }
}