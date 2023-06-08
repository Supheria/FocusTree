using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Tokenizer
    {
        static List<char> Delimiter = new() { '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', '\0' };
        static List<char> Blank = new() { '\t', ' ', '\n', '\r', '\0' };
        static List<char> EndLine = new() { '\n', '\r', '\0' };
        static List<char> Marker = new() { '=', '>', '<', '}', '{' };
        static char Note = '#';
        static char Quote = '"';
        static char Escape = '\\';

        enum States
        {
            None,
            Quotation,
            Escape,
            Unquotation,
            Note
        }
        States State { get; set; } = States.None;
        byte[] Buffer { get; set; }
        uint BufferPosition { get; set; } = 0;
        uint Line { get; set; } = 1;
        uint Column { get; set; } = 0;
        ParseTree Tree { get; set; } = null;
        Element Composed { get; set; } = null;
        StringBuilder Composing { get; set; } = new();
        List<TokenAPI> tokens { get; set; } = new();


        public Tokenizer(string FilePath, List<TokenAPI> Tokens)
        {
            ReadBuffer(FilePath);
            Tree = new ParseTree();
            while (BufferPosition < Buffer.Length)
            {
                if (Compose((char)Buffer[BufferPosition]))
                {
                    var _Tree = Tree.Parse(Composed);
                    if (_Tree == null)
                    {
                        CacheList();
                        Tree = new ParseTree();
                    }
                    else { Tree = _Tree; }
                }
            }
            EndCheck();
        }
        private void ReadBuffer(string FilePath)
        {
            using var file = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            if (file.ReadByte() == 0xEF && file.ReadByte() == 0xBB && file.ReadByte() == 0xBF)
            {
                file.Read(Buffer, 3, (int)file.Length - 3);
            }
            else
            {
                file.Read(Buffer);
            }
        }
        private bool Compose(char ch)
        {
            if (Composed.OwnValue == true) { return true; }
            switch(State)
            {
                case States.Quotation:
                    if (ch == Escape)
                    {
                        Composing.Append(GetChar());
                        State = States.Escape;
                        return false;
                    }
                    else if (ch == Quote)
                    {
                        Composing.Append(GetChar());
                        Composed = new(Composing.ToString(), Line, Column);
                        State = States.None;
                        return true;
                    }
                    else if (EndLine.Contains(ch))
                    {
                        Composing.Append(Quote);
                        Composed = new(Composing.ToString(), Line, Column);
                        State = States.None;
                        return true;
                    }
                    Composing.Append(GetChar());
                    return false;
                case States.Escape:
                    if (EndLine.Contains(ch))
                    {
                        Composing.Append(Quote);
                        Composing.Append(Quote);
                        Composed = new(Composing.ToString(), Line, Column);
                        State = States.None;
                        return true;
                    }
                    else
                    {
                        Composing.Append(GetChar());
                        State = States.Quotation;
                        return false;
                    }
                case States.Unquotation:
                    if (Delimiter.Contains(ch)) 
                    {
                        Composed = new(Composing.ToString(), Line, Column);
                        State = States.None;
                        return true;
                    }
                    Composing.Append(GetChar());
                    return false;
                case States.Note:
                    if (EndLine.Contains(ch))
                    {
                        State = States.None;
                    }
                    GetChar();
                    return false;
                default:
                    if (ch == Quote)
                    {
                        Composing.Clear();
                        Composing.Append(GetChar());
                        State = States.Quotation;
                    }
                    else if (ch == Note)
                    {
                        State = States.Note;
                        GetChar();
                    }
                    else if (Marker.Contains(ch))
                    {
                        Composed = new(GetChar().ToString(), Line, Column);
                        return true;
                    }
                    else if (Blank.Contains(ch))
                    {
                        if (ch == '\n')
                        {
                            Line++;
                            Column = 0;
                        }
                        else if (ch == '\t')
                        {
                            Column += 3;
                        }
                        GetChar();
                    }
                    else
                    {
                        Composing.Clear();
                        Composing.Append(GetChar());
                        State = States.Unquotation;
                    }
                    return false;
            }
        }
        private void CacheList()
        {
            var token = Tree.OnceGet();
            if (token == null) { return; }
            tokens.Add(token);
        }
        private char GetChar()
        {
            char ch = (char)Buffer[BufferPosition++];
            Column++;
            return ch;
        }
        private void EndCheck()
        {
            if (Tree.From != null)
            {
                Exceptions.Exception($"interruption at line({Line}), column({Column})");
                Tree.From.Append(Tree.OnceGet());
                Tree = Tree.From;
                while (Tree.From != null)
                {
                    Tree.From.Append(Tree.OnceGet());
                    Tree = Tree.From;
                }
            }
            CacheList();
        }
    }
}
