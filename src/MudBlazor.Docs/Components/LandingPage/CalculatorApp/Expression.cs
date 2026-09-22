//MIT License

//Copyright (c) 2020 Meinrad Recheis

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Globalization;

namespace PrimitiveCalculator
{
    public class Expression
    {
        private readonly SimpleParser _parser;

        /// <summary>
        /// Gets the evaluated value of this expression.
        /// </summary>
        public double? Value { get; set; }

        public Expression(string expression)
        {
            _parser = new SimpleParser(expression.Trim());
        }

        public Expression(SimpleParser parser)
        {
            _parser = parser;
        }

        public Expression(double nr)
        {
            Value = nr;
        }

        public double Eval()
        {
            if (Value.HasValue)
            {
                return Value.Value;
            }

            try
            {
                SkipWhitespace();
                if (!_parser.HasNext)
                {
                    return double.NaN;
                }

                var result = ParseExpression();
                SkipWhitespace();
                if (_parser.HasNext)
                {
                    return double.NaN;
                }

                Value = result;
                return result;
            }
            catch
            {
                return double.NaN;
            }
        }

        private void SkipWhitespace()
        {
            _parser.ConsumeAny(' ');
        }

        private double ParseExpression()
        {
            var value = ParseTerm();

            while (true)
            {
                SkipWhitespace();
                if (_parser.NextIs('+'))
                {
                    _parser.Skip(1);
                    value += ParseTerm();
                    continue;
                }

                if (_parser.NextIs('-'))
                {
                    _parser.Skip(1);
                    value -= ParseTerm();
                    continue;
                }

                break;
            }

            return value;
        }

        private double ParseTerm()
        {
            var value = ParsePower();

            while (true)
            {
                SkipWhitespace();
                if (_parser.NextIs('*'))
                {
                    _parser.Skip(1);
                    value *= ParsePower();
                    continue;
                }

                if (_parser.NextIs('/'))
                {
                    _parser.Skip(1);
                    value /= ParsePower();
                    continue;
                }

                if (_parser.NextIs('%'))
                {
                    _parser.Skip(1);
                    value %= ParsePower();
                    continue;
                }

                break;
            }

            return value;
        }

        private double ParsePower()
        {
            var value = ParseFactor();

            while (true)
            {
                SkipWhitespace();
                if (!_parser.NextIs('^'))
                {
                    break;
                }

                _parser.Skip(1);
                value = Math.Pow(value, ParseFactor());
            }

            return value;
        }

        private double ParseFactor()
        {
            SkipWhitespace();

            if (_parser.NextIs('+'))
            {
                _parser.Skip(1);
                return ParseFactor();
            }

            if (_parser.NextIs('-'))
            {
                _parser.Skip(1);
                return -ParseFactor();
            }

            if (_parser.NextIs('('))
            {
                _parser.Skip(1);
                var inner = ParseExpression();
                SkipWhitespace();
                if (!_parser.NextIs(')'))
                {
                    throw new FormatException("Missing closing bracket.");
                }

                _parser.Skip(1);
                return inner;
            }

            return ReadNumber(_parser);
        }

        private static double ReadNumber(SimpleParser parser)
        {
            if (!char.IsDigit(parser.NextChar ?? '\0') && !parser.NextIs('.'))
            {
                throw new FormatException("Expected number.");
            }

            var token = parser.ConsumeAny("0123456789.".ToCharArray());
            if (!double.TryParse(token, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                return double.NaN;
            }

            return result;
        }
    }
}
