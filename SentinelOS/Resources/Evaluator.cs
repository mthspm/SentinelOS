using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelOS.Resources
{
    public class Evaluator
    {
        private readonly List<Token> tokens;
        private int position;

        public Evaluator(List<Token> tokens)
        {
            this.tokens = tokens;
            position = 0;
        }

        private Token CurrentToken => position < tokens.Count ? tokens[position] : null;

        private void Advance()
        {
            position++;
        }

        private double Factor()
        {
            Token token = CurrentToken;
            if (token.Type == TokenType.Number)
            {
                Advance();
                return double.Parse(token.Value);
            }
            else if (token.Type == TokenType.LeftParen || token.Type == TokenType.LeftBracket)
            {
                Advance();
                double result = Expression();
                if (CurrentToken.Type == TokenType.RightParen || CurrentToken.Type == TokenType.RightBracket)
                {
                    Advance();
                }
                return result;
            }
            else if (token.Type == TokenType.Plus)
            {
                Advance();
                return Factor();
            }
            else if (token.Type == TokenType.Minus)
            {
                Advance();
                return -Factor();
            }
            throw new Exception($"Unexpected token: {token}");
        }

        private double Power()
        {
            double result = Factor();
            while (CurrentToken != null && CurrentToken.Type == TokenType.Power)
            {
                Advance();
                result = Math.Pow(result, Factor());
            }
            return result;
        }

        private double Term()
        {
            double result = Power();
            while (CurrentToken != null && (CurrentToken.Type == TokenType.Multiply || CurrentToken.Type == TokenType.Divide))
            {
                if (CurrentToken.Type == TokenType.Multiply)
                {
                    Advance();
                    result *= Power();
                }
                else if (CurrentToken.Type == TokenType.Divide)
                {
                    Advance();
                    result /= Power();
                }
            }
            return result;
        }

        public double Expression()
        {
            double result = Term();
            while (CurrentToken != null && (CurrentToken.Type == TokenType.Plus || CurrentToken.Type == TokenType.Minus))
            {
                if (CurrentToken.Type == TokenType.Plus)
                {
                    Advance();
                    result += Term();
                }
                else if (CurrentToken.Type == TokenType.Minus)
                {
                    Advance();
                    result -= Term();
                }
            }
            return result;
        }
    }

}
