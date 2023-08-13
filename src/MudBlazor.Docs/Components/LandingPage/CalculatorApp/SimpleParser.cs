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
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace PrimitiveCalculator
{
    public class SimpleParser
    {
        public SimpleParser(string s)
        {
            Debug.Assert(s != null);
            _data = s;
        }
        protected string _data;
        public int Position { get; set; } = 0;

        // returns true if the next char is one of the given chars. does not consume
        public bool NextIs(params char[] chars)
        {
            if (Position >= _data.Length)
                return false;
            return chars.Contains(_data[Position]);
        }

        public bool NextIs(string s)
        {
            var current_i = Position;
            try
            {
                foreach (var ch in s)
                {
                    if (!NextIs(ch))
                        return false;
                    Skip(1);
                }
                return true;
            }
            finally
            {
                Position = current_i;
            }
        }

        public string ConsumeAny(params char[] chars)
        {
            var sb = new StringBuilder();
            while (Position < _data.Length)
            {
                if (chars.Contains(_data[Position]))
                {
                    sb.Append(_data[Position]);
                    Position += 1;
                }
                else
                    break;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Reads until it encounters one of the stop chars. Does not return the stopchar as part of the return string but consumes it.
        /// </summary>
        /// <param name="stop_chars"></param>
        /// <returns></returns>
        public string ReadUntil(params char[] stop_chars)
        {
            var s = new StringBuilder();
            while (Position < _data.Length)
            {
                var c1 = _data[Position];
                LastChar = c1;
                Position += 1;
                if (stop_chars.Any(stop_char => stop_char == c1))
                    break;
                s.Append(c1);
            }
            return s.ToString();
        }

        /// <summary>
        /// Reads until it encounters the expected string. Does not return the expected string as part of the return string but consumes it.
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        public string ReadUntil(string expected)
        {
            var s = new StringBuilder();
            while (Position < _data.Length)
            {
                s.Append(ReadUntil(expected[0]));
                var pos = Position;
                Unskip(1);
                if (NextIs(expected))
                {
                    Skip(expected.Length);
                    break;
                }
                Position = pos;
                if (pos < _data.Length)
                    s.Append(expected[0]); // we didn't find the expected string, so add the consumed stopchar, or else it would be missing due to ReadUntil(expected[0]) having not returned it.
            }
            return s.ToString();
        }

        public char LastChar
        {
            get;
            private set;
        }

        public char? NextChar
        {
            get
            {
                if (Position < _data.Length)
                    return _data[Position];
                return null;
            }
        }

        public bool HasNext { get { return Position < _data.Length; } }

        public void Skip(int n)
        {
            Position += n;
        }

        public void Unskip(int n)
        {
            Position = Math.Max(0, Position - n);
        }

        public bool SkipUntil(string expected)
        {
            if (_data.Length < expected.Length)
                return false;
            while (Position < _data.Length)
            {
                SkipUntil(expected[0]);
                var pos = Position;
                Unskip(1);
                if (NextIs(expected))
                {
                    Skip(expected.Length);
                    return true;
                }
                Position = pos;
            }
            return false;
        }

        public bool SkipUntil(params char[] stop_chars)
        {
            while (Position < _data.Length)
            {
                var c1 = _data[Position];
                LastChar = c1;
                Position += 1;
                if (stop_chars.Any(stop_char => stop_char == c1))
                    return true;
            }
            return false;
        }
    }


}
