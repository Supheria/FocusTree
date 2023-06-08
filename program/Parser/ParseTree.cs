using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class ParseTree
    {
        const char OpenBrace = '{';
        const char CloseBrace = '}';
        const char Equal = '=';
        const char Greater = '>';
        const char Less = '<';

        enum Steps
        {
            None = 0,
            Key = 0b1,
            Operator = 0b1 << 1,
            Value = 0b1 << 2,
            Tag = 0b1 << 3,
            Array = 0b1 << 4,
            Sub = 0b1 << 5,
            On = 0b1 << 6,
            Off = 0b1 << 7
        }
        Steps Step { get; set; } = Steps.None;
        string Key { get; set; } = string.Empty;
        string Operator { get; set; } = string.Empty;
        string Value { get; set; } = string.Empty;
        string Array { get; set; } = string.Empty;
        TokenAPI Builder { get; set; } = null;
        public ParseTree From { get; init; }
        uint Level { get; init; }
        bool OwnBuilder { get; set; } = true;

        public ParseTree()
        {
            From = null;
            Level = 0;
        }

        public ParseTree(ParseTree from, uint level, string key, string operat)
        {
            From = from;
            Level = level;
            Key = key;
            Operator = operat;
        }

        public TokenAPI OnceGet()
        {
            if (OwnBuilder)
            {
                OwnBuilder = false;
                return Builder;
            }
            else { return null; }
        }
        private void Done()
        {
            From?.Append(Builder);
        }

        public void Append(TokenAPI token)
        {
            ((Scope)Builder).Append(token);
        }

        public ParseTree Parse(Element element)
        {
            char ch = element.Head();
            if ((Step & Steps.Sub) > 0)
            {
                return Parse_Sub(element);
            }
            else if ((Step & Steps.Array) > 0)
            {
                return Parse_Array(element);
            }
            else if ((Step & Steps.Operator) > 0)
            {
                return Parse_Operator(element);
            }
            //
            // 1
            //
            else if ((Step & Steps.Key) > 0)
            {
                switch(ch)
                {
                    case Equal:
                    case Greater:
                    case Less:
                        Step = Steps.Operator;
                        Operator = element.GetValue();
                        return this;
                    default:
                        Exceptions.UnexpectedOperator(element);
                        element.GetValue();
                        return From;
                }
            }
            //
            // 3
            //
            else if ((Step & Steps.Value) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                        Step = Steps.Value;
                        element.GetValue();
                        return this;
                    default:
                        Done();
                        // element.GetValue(); // leave element to next tree
                        return From;
                }
            }
            //
            // 4
            //
            else if ((Step & Steps.Tag) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        element.GetValue();
                        Done();
                        return From;
                    default:
                        Step = Steps.Tag;
                        ((TaggedValue)Builder).Append(Value);
                        return this;
                }
            }
            //
            // 0 - None
            //
            else
            {
                switch(ch)
                {
                    case OpenBrace:
                    case CloseBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedKey(element);
                        element.GetValue();
                        return From;
                    default:
                        Step = Steps.Key;
                        Key = element.GetValue();
                        return this;
                }
            }
        }
        public ParseTree Parse_Sub(Element element)
        {
            char ch = element.Head();
            //
            // 6
            //
            if ((Step & Steps.Key) > 0)
            {
                switch (ch)
                {
                    case OpenBrace:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        ((Scope)Builder).Append(new Token(Key, Level + 1));
                        element.GetValue();
                        Done();
                        return From;
                    case Equal:
                    case Greater:
                    case Less:
                        Step = Steps.Sub;
                        return new ParseTree(this, Level + 1, Key, element.GetValue());
                    default:
                        ((Scope)Builder).Append(new Token(Value, Level + 1));
                        Value = element.GetValue();
                        return this;
                }
            }
            //
            // 7 - Sub
            //
            else
            {
                switch (ch)
                {
                    case OpenBrace:
                    case CloseBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        element.GetValue();
                        Done();
                        return From;
                    default:
                        Step = Steps.Sub | Steps.Key;
                        element.GetValue();
                        return this;
                }
            }
        }
        public ParseTree Parse_Array(Element element)
        {
            char ch = element.Head();
            if ((Step & Steps.Tag) > 0)
            { 
                return Parse_Tag_Array(element);
            }
            else if ((Step & Steps.Value) > 0)
            {
                return Parse_Value_Array(element);
            }
            //
            // 9
            //
            else if ((Step & Steps.Off) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                        Step = Steps.Array;
                        element.GetValue();
                        return this;
                    case CloseBrace:
                        element.GetValue();
                        Done();
                        return From;
                    default:
                        Exceptions.UnexpectedArraySyntax(element);
                        element.GetValue();
                        return From;
                }
            }
            //
            // 10
            //
            else if ((Step & Steps.Key) > 0)
            {
                switch (ch)
                {
                    case OpenBrace:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedOperator(element);
                        element.GetValue();
                        return From;
                    case Equal:
                        Step = Steps.Array | Steps.Tag;
                        Builder = new TagArray(Key, Level);
                        ((TagArray)Builder).Append_new(Array);
                        element.GetValue();
                        return this;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Value | Steps.Off;
                        Builder = new ValueArray(Key, Level);
                        ((ValueArray)Builder).Append_new(Array);
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Array | Steps.Value;
                        Builder = new ValueArray(Key, Level);
                        ((ValueArray)Builder).Append_new(Array);
                        ((ValueArray)Builder).Append(element.GetValue());
                        return this;
                }
            }
            //
            // 8 - Array
            //
            else
            {
                switch(ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Off;
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Array | Steps.Key;
                        Array = element.GetValue();
                        return this;
                }
            }
        }
        public ParseTree Parse_Tag_Array(Element element)
        {
            char ch = element.Head();
            //
            // 12
            //
            if ((Step & Steps.Off) > 0)
            {
                switch (ch)
                {
                    case OpenBrace:
                        Step = Steps.Array | Steps.Value | Steps.On;
                        element.GetValue();
                        return this;
                    case CloseBrace:
                        element.GetValue();
                        Done();
                        return From;
                    default:
                        Exceptions.UnexpectedArraySyntax(element);
                        element.GetValue();
                        return From;
                }
            }
            //
            // 13
            //
            else if ((Step & Steps.On) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue(); 
                        return From;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Value | Steps.Off;
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Array | Steps.Value | Steps.Key;
                        ((ValueArray)Builder).Append_new(element.GetValue());
                        return this;
                }
            }
            //
            // 14
            //
            else if ((Step & Steps.Key) > 0)
            {
                switch (ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedArrayType(element);
                        element.GetValue(); 
                        return From;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Value | Steps.Off;
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Array | Steps.Value;
                        ((ValueArray)Builder).Append(element.GetValue());
                        return this;
                }
            }
            //
            // 11 - Array | Value
            //
            else
            {
                switch(ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Value | Steps.Off;
                        element.GetValue();
                        return this;
                    default:
                        ((ValueArray)Builder).Append(element.GetValue());
                        return this;
                }
            }
        }
        public ParseTree Parse_Value_Array(Element element)
        {
            char ch = element.Head();
            if ((Step & Steps.Value) > 0)
            {
                //
                // 17
                //
                if ((Step & Steps.Off) > 0)
                {
                    switch (ch)
                    {
                        case OpenBrace:
                        case Equal:
                        case Greater:
                        case Less:
                            Exceptions.UnexpectedKey(element);
                            element.GetValue();
                            return From;
                        case CloseBrace:
                            Step = Steps.Array | Steps.Tag | Steps.Off;
                            element.GetValue();
                            return this;
                        default:
                            Step = Steps.Array | Steps.Tag | Steps.Key;
                            ((TagArray)Builder).Append(element.GetValue());
                            return this;
                    }
                }
                //
                // 16 - Array | Tag | Value
                //
                else
                {
                    switch (ch)
                    {
                        case OpenBrace:
                        case Equal:
                        case Greater:
                        case Less:
                            Exceptions.UnexpectedValue(element);
                            element.GetValue();
                            return From;
                        case CloseBrace:
                            Step = Steps.Array | Steps.Tag | Steps.Value | Steps.Off;
                            element.GetValue();
                            return this;
                        default:
                            element.GetValue();
                            ((TagArray)Builder).Append(element.GetValue());
                            return this;
                    }
                }
            }
            //
            // 18
            //
            else if ((Step & Steps.Off) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                        Step = Steps.Array | Steps.Tag | Steps.On;
                        element.GetValue();
                        return this;
                    case CloseBrace:
                        element.GetValue();
                        Done();
                        return From;
                    default:
                        Exceptions.UnexpectedArraySyntax(element);
                        element.GetValue();
                        return From;
                }
            }
            //
            // 19
            //
            else if ((Step & Steps.On) > 0)
            {
                switch(ch)
                {
                    case OpenBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedKey(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        Step = Steps.Array | Steps.Tag | Steps.Off;
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Array | Steps.Tag | Steps.Key;
                        ((TagArray)Builder).Append_new(element.GetValue());
                        return this;
                }
            }
            //
            // 20
            //
            else if ((Step & Steps.Key) > 0)
            {
                switch(ch)
                {
                    case Equal:
                        Step = Steps.Array | Steps.Tag;
                        element.GetValue();
                        return this;
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedOperator(element);
                        element.GetValue();
                        return From;
                    default:
                        Exceptions.UnexpectedArrayType(element);
                        element.GetValue();
                        return From;
                }
            }
            //
            //
            // 15 - Array | Tag
            else
            {
                switch(ch)
                {
                    case OpenBrace:
                        Step = Steps.Array | Steps.Tag | Steps.Value;
                        element.GetValue();
                        return this;
                    default:
                        Exceptions.UnexpectedArrayType(element);
                        element.GetValue();
                        return From;
                }
            }
        }

        #region ==== Parse Operator (5, 2) ====

        public ParseTree Parse_Operator(Element element)
        {
            char ch = element.Head();
            //
            // 5
            //
            if ((Step & Steps.On) > 0)
            {
                switch (ch)
                {
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case CloseBrace:
                        element.GetValue();
                        Done();
                        return From;
                    case OpenBrace:
                        Step = Steps.Array;
                        element.GetValue();
                        return this;
                    default:
                        Step = Steps.Sub | Steps.Key;
                        Value = element.GetValue();
                        Builder = new Scope(Key, Level);
                        return this;
                }
            }
            //
            // 2 - Op
            //
            else
            {
                switch (ch)
                {
                    case CloseBrace:
                    case Equal:
                    case Greater:
                    case Less:
                        Exceptions.UnexpectedValue(element);
                        element.GetValue();
                        return From;
                    case OpenBrace:
                        if (Operator[0] != Equal)
                        {
                            Exceptions.UnexpectedOperator(element);
                            element.GetValue();
                            return From;
                        }
                        else
                        {
                            Step = Steps.Operator | Steps.On;
                            element.GetValue();
                            return this;
                        }
                    default:
                        Step = Steps.Value;
                        Builder = new TaggedValue(Key, Level, Operator, element.GetValue());
                        return this;
                }
            }
        }

        #endregion
    }
}
