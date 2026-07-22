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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PrimitiveCalculator
{
    public class Expression
    {
        private List<Operation> _operations = new();
        private SimpleParser _parser;
        private bool _mustConsumeClosingBracket;
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
            var op_count = -1;
            while (_parser.HasNext)
            {
                if (_operations.Count > op_count)
                    op_count = _operations.Count;
                else
                    break;
                _parser.ConsumeAny(' ');
                if (_parser.NextIs('('))
                {
                    _parser.Skip(1);
                    var exp = new Expression(_parser) { _mustConsumeClosingBracket = true };
                    exp.Eval();
                    var op = new Operation { Operator = "+", Expression = exp };
                    _operations.Add(op);
                }
                else if (_parser.NextIs("+-*^/%".ToCharArray()))
                {
                    var op = new Operation() { Operator = _parser.NextChar.ToString() };
                    _parser.Skip(1);
                    _parser.ConsumeAny(' ');
                    if (_parser.NextIs('('))
                    {
                        _parser.Skip(1);
                        var exp = new Expression(_parser);
                        exp._mustConsumeClosingBracket = true;
                        op.Expression = exp;
                        exp.Eval();
                    }
                    else
                        op.Expression = new Expression(ReadNumber(_parser));
                    _operations.Add(op);
                }
                else if (_parser.NextIs("0.123456789".ToCharArray()))
                {
                    _operations.Add(new Operation { Operator = "+", Expression = new Expression(ReadNumber(_parser)) });
                }
                else if (_parser.NextIs(')') && _mustConsumeClosingBracket)
                {
                    _parser.Skip(1);
                    break;
                }
            }
            if (!_operations.Any())
                return double.NaN;
            var first_op = _operations.First();
            if (string.IsNullOrEmpty(first_op.Operator))
                first_op.Operator = "+";
            else if (first_op.Operator == "-")
            {
                first_op.Operator = "+";
                first_op.Expression.Value = (first_op.Expression.Value ?? double.NaN) * (-1);
            }
            if (_operations.Count == 1)
            {
                var op = _operations[0];
                var val = op.Expression.Value;
                if (val == null)
                    return double.NaN;
                if (op.Operator == "-")
                    Value = (-1.0 * (val.Value));
                else
                    Value = val;
            }
            else if (_operations.Count == 2)
            {
                Value = _operations[1].Apply(_operations[0].Apply(0));
            }
            else
            {
                if (Precedence(_operations[0].Operator) > 0)
                    return double.NaN; // only + and minus may be first operator!
                // repeated contraction by precedence
                while (_operations.Count > 1)
                {
                    var highest_op = _operations.Select(x => x.Operator).OrderByDescending(x => Precedence(x)).First();
                    if (Precedence(highest_op) == 0)
                    {
                        double sum = 0;
                        foreach (var op in _operations)
                            sum = op.Apply(sum);
                        return sum;
                    }
                    var ops = new List<Operation>();
                    var i = 0;
                    foreach (var op in _operations)
                    {
                        if (op.Operator == highest_op)
                        {
                            var last_op = ops[ops.Count - 1];
                            last_op.Expression.Value = op.Apply(last_op.Expression.Value ?? double.NaN);
                            i++;
                            continue;
                        }
                        ops.Add(op);
                        i++;
                    }
                    _operations = ops;
                }
                return _operations[0].Apply(0);
            }
            return Value ?? double.NaN;
        }

        private double ReadNumber(SimpleParser parser)
        {
            var factor = parser.NextIs('-') ? -1 : 1;
            parser.ConsumeAny('+', '-');
            if (!double.TryParse(parser.ConsumeAny("0.123456789".ToCharArray()), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return double.NaN;
            return factor * result;
        }

        private int Precedence(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 0;
                case "*":
                case "/":
                case "%":
                    return 1;
                case "^":
                    return 2;
            }
            return -1;
        }
    }

    public class Operation
    {
        public string Operator { get; set; }
        public Expression Expression { get; set; }

        public double Apply(double v)
        {
            if (Expression == null)
                return double.NaN;
            return Operator switch
            {
                "+" => v + Expression.Value ?? double.NaN,
                "-" => v - Expression.Value ?? double.NaN,
                "*" => v * Expression.Value ?? double.NaN,
                "/" => v / Expression.Value ?? double.NaN,
                "%" => v % Expression.Value ?? double.NaN,
                "^" => Math.Pow(v, Expression.Value ?? double.NaN),
                _ => double.NaN,
            };
        }
    }


}
