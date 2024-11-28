using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources
{
    public enum TokenType
    {
        Number,
        Plus,
        Minus,
        Multiply,
        Divide,
        Power,
        LeftParen,
        RightParen,
        LeftBracket,
        RightBracket
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type}: {Value}";
        }
    }

    public class Lexer
    {
        private readonly string input;
        private int position;
        private char currentChar;

        public Lexer(string input)
        {
            this.input = input;
            position = 0;
            currentChar = input[position];
        }

        private void Advance()
        {
            position++;
            if (position < input.Length)
            {
                currentChar = input[position];
            }
            else
            {
                currentChar = '\0'; // End of input
            }
        }

        private void SkipWhitespace()
        {
            while (char.IsWhiteSpace(currentChar))
            {
                Advance();
            }
        }

        private string Number()
        {
            StringBuilder result = new StringBuilder();
            while (char.IsDigit(currentChar) || currentChar == '.')
            {
                result.Append(currentChar);
                Advance();
            }
            return result.ToString();
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();

            while (currentChar != '\0')
            {
                if (char.IsWhiteSpace(currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (char.IsDigit(currentChar))
                {
                    tokens.Add(new Token(TokenType.Number, Number()));
                    continue;
                }

                switch (currentChar)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus, "+"));
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.Minus, "-"));
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Multiply, "*"));
                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Divide, "/"));
                        break;
                    case '^':
                        tokens.Add(new Token(TokenType.Power, "^"));
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.LeftParen, "("));
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RightParen, ")"));
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LeftBracket, "["));
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RightBracket, "]"));
                        break;
                    default:
                        throw new Exception($"Unexpected character: {currentChar} at position {position}");
                }

                Advance();
            }

            ValidateTokens(tokens);

            return tokens;
        }

        private void ValidateTokens(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("Input cannot be empty.");
            }

            Token previousToken = null;
            int parenCount = 0;
            int bracketCount = 0;

            foreach (var token in tokens)
            {
                if (token.Type == TokenType.LeftParen)
                {
                    parenCount++;
                }
                else if (token.Type == TokenType.RightParen)
                {
                    parenCount--;
                }
                else if (token.Type == TokenType.LeftBracket)
                {
                    bracketCount++;
                }
                else if (token.Type == TokenType.RightBracket)
                {
                    bracketCount--;
                }

                if (parenCount < 0 || bracketCount < 0)
                {
                    throw new Exception("Mismatched parentheses or brackets.");
                }

                if (previousToken != null)
                {
                    if (IsOperator(previousToken.Type) && IsOperator(token.Type))
                    {
                        throw new Exception($"Invalid sequence: {previousToken.Value}{token.Value}");
                    }

                    if (previousToken.Type == TokenType.LeftParen && token.Type == TokenType.RightParen)
                    {
                        throw new Exception("Empty parentheses are not allowed.");
                    }

                    if (previousToken.Type == TokenType.LeftBracket && token.Type == TokenType.RightBracket)
                    {
                        throw new Exception("Empty brackets are not allowed.");
                    }
                }

                previousToken = token;
            }

            if (parenCount != 0 || bracketCount != 0)
            {
                throw new Exception("Mismatched parentheses or brackets.");
            }

            if (IsOperator(tokens.Last().Type))
            {
                throw new Exception($"Expression cannot end with an operator: {tokens.Last().Value}");
            }
        }

        private bool IsOperator(TokenType type)
        {
            return type == TokenType.Plus || type == TokenType.Minus ||
                   type == TokenType.Multiply || type == TokenType.Divide ||
                   type == TokenType.Power;
        }
    }

}
