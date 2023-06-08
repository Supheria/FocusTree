using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Token : TokenAPI
    {
        public string Name { get; init; }
        public TokenTypes Type { get; init; }
        public uint Level { get; init; }
        public Token(string name, uint level)
        {
            Type = TokenTypes.Token;
            Name = name;
            Level = level;
        }
    }

    public class TaggedValue : TaggedValueAPI
    {
        public string Name { get; init; }
        public TokenTypes Type { get; init; }
        public uint Level { get; init; }
        public string Operator { get; init; }
        public string Tag { get; init; }
        public List<string> Value { get; private set; }
        public TaggedValue(string name, uint level, string operat, string tag)
        {
            Type = TokenTypes.TaggedValue;
            Name = name;
            Level = level;
            Operator = operat;
            Tag = tag;
            Value = new();
        }
        public void Append(string value)
        {
            Value.Add(value);
        }
    }

    public class ValueArray : ValueArrayAPI
    {
        public string Name { get; init; }
        public TokenTypes Type { get; init; }
        public uint Level { get; init; }
        public List<List<string>> Value { get; private set; }
        public ValueArray(string name, uint level)
        {
            Type = TokenTypes.ValueArray;
            Name = name;
            Level = level;
            Value = new();
        }
        public void Append(string value)
        {
            Value.LastOrDefault()?.Add(value);
        }
        public void Append_new(string value)
        {
            Value.Add(new() { value });
        }
    }

    public class TagArray : TagArrayAPI
    {
        public string Name { get; init; }
        public TokenTypes Type { get; init; }
        public uint Level { get; init; }
        public List<List<KeyValuePair<string, List<string>>>> Value { get; private set; }
        public TagArray(string name, uint level)
        {
            Type = TokenTypes.TagArray;
            Name = name;
            Level = level;
            Value = new();
        }
        public void Append(string value)
        {
            Value.LastOrDefault()?.LastOrDefault().Value.Add(value);
        }
        public void Append_tag(string value)
        {
            Value.LastOrDefault()?.Add(new(value, new()));
        }
        public void Append_new(string value)
        {
            Value.Add(new() { new(value, new()) });
        }
    }

    public class Scope : ScopeAPI
    {
        public string Name { get; init; }
        public TokenTypes Type { get; init; }
        public uint Level { get; init; }
        public List<TokenAPI> Property { get; private set; }
        public Scope(string name, uint level)
        {
            Type = TokenTypes.Scope;
            Name = name;
            Level = level;
            Property = new();
        }
        public void Append(TokenAPI property)
        {
            if (property == null) { return; }
            if (property.Level != Level + 1)
            {
                // ex_log()->Append(fn_tt, "level mismatched of Appending in Scope", ExLog::ERR);
                return;
            }
            Property.Add(property);
        }
    }
}
