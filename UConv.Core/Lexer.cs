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
        Semi
    }

    public class Token
    {
        public Token(TokenType ty)
        {
            Type = ty;
        }

        public TokenType Type { get; set; }
    }

    public class IdentToken : Token
    {
        public IdentToken(string ident) : base(TokenType.Ident)
        {
            Identifier = ident;
        }

        public string Identifier { get; set; }
    }

    public class NumToken : Token
    {
        public NumToken(double num) : base(TokenType.Num)
        {
            Number = num;
        }

        public double Number { get; set; }
    }

    public class ExprLexer
    {
        private char? current;
        private readonly int len;
        private int saved;
        private int position = -1;

        public ExprLexer(string content)
        {
            this.content = content;
            len = content.Length;
            current = null;
        }

        private string content { get; }

        private void savePosition()
        {
            saved = position;
        }

        private void rewindPosition()
        {
            position = saved;
        }

        private char? peekByte()
        {
            if (position < len - 1) return content[position + 1];

            return null;
        }

        private char? nextByte()
        {
            if (position < len - 1)
            {
                position++;
                current = content[position];
                return current;
            }

            return null;
        }

        private char? currByte()
        {
            return current;
        }

        private Token parseNumToken()
        {
            savePosition();
            var num = "";
            var ch = currByte();

            if (!ch.HasValue) return null;
            num += ch.Value;

            while (true)
            {
                ch = peekByte();
                if (!ch.HasValue) break;

                if (ch.Value == ' ' || ch.Value == '\t' || ch.Value == '\n' || ch.Value == '\r' ||
                    ch.Value == ';') break;
                num += ch.Value;
                nextByte();
            }

            return new NumToken(double.Parse(num));
        }

        private Token parseIdentToken()
        {
            savePosition();
            var ident = "";
            var ch = currByte();

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
            var ch = nextByte();
            while (true)
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
                    case ';':
                        return new Token(TokenType.Semi);
                    default:
                        if (char.IsDigit(ch.Value))
                            return parseNumToken();
                        else
                            return parseIdentToken();
                }
        }
    }
}