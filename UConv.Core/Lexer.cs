using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UConv.Core
{

    public enum TokenType
    {
        Ident,
        Num,
        Eq,
        Asterisk,
        Gt,
        Lt,
        Plus,
        Minus,
        LBracket,
        RBracket,
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public Token(TokenType ty)
        {
            Type = ty;
        }
    }

    public class IdentToken : Token
    {
        public string Identifier { get; set; }
        public IdentToken(string ident) : base(TokenType.Ident)
        {
            Identifier = ident;
        }
    }

    public class NumToken : Token
    {
        public double Number { get; set; }
        public NumToken(double num) : base(TokenType.Num)
        {
            Number = num;
        }
    }

    public class ExprLexer
    {
        private string content { get; set; }
        private int position = -1;
        private int len, saved = 0;
        private Nullable<char> current;
        
        public ExprLexer(string content)
        {
            this.content = content;
            this.len = content.Length;
            this.current = null;
        }

        private void savePosition()
        {
            saved = position;
        }

        private void rewindPosition()
        {
            position = saved;
        }

        private Nullable<char> peekByte()
        {
            if (position < len - 1)
            {
                return content[position + 1];
            }

            return null;
        }

        private Nullable<char> nextByte()
        {
            if (position < len - 1)
            {
                position++;
                current = content[position];
                return current;
            }

            return null;
        }

        private Nullable<char> currByte()
        {
            return current;
        }

        private Token parseNumToken()
        {
            savePosition();
            string num = "";
            Nullable<char> ch = currByte();

            if (!ch.HasValue) return null;
            num += ch.Value;

            while (true)
            {
                ch = peekByte();
                if (!ch.HasValue) break;

                if (ch.Value == ' ' || ch.Value == '\t' || ch.Value == '\n' || ch.Value == '\r') break;
                num += ch.Value;
                nextByte();
            }

            return new NumToken(Double.Parse(num));
        }

        private Token parseIdentToken()
        {
            savePosition();
            string ident = "";
            Nullable<char> ch = currByte();

            if (!ch.HasValue) return null;
            ident += ch.Value;

            while (true)
            {
                ch = peekByte();
                if (!ch.HasValue) break;

                if (ch.Value == ' ' || ch.Value == '\t' || ch.Value == '\n' || ch.Value == '\r') break;
                ident += ch.Value;
                nextByte();
            }

            return new IdentToken(ident);
        }

        public Token NextToken()
        {
            Nullable<char> ch = nextByte();
            while (true)
            {
                switch (ch)
                {
                    case null:
                        return null;
                    case ' ':
                        ch = nextByte();
                        continue;
                    case '=':
                        return new Token(TokenType.Eq);
                    case '<':
                        return new Token(TokenType.Lt);
                    case '>':
                        return new Token(TokenType.Gt);
                    case '+':
                        return new Token(TokenType.Plus);
                    case '-':
                        return new Token(TokenType.Minus);
                    case '(':
                        return new Token(TokenType.LBracket);
                    case ')':
                        return new Token(TokenType.RBracket);
                    case '*':
                        return new Token(TokenType.Asterisk);
                    default:
                        if (Char.IsDigit(ch.Value))
                        {
                            return parseNumToken();
                        }
                        else
                        {
                            return parseIdentToken();
                        }
                }
            }

        }
    }


}
