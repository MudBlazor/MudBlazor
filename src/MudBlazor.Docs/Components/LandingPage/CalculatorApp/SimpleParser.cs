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

using System.Text;

namespace PrimitiveCalculator;

public class SimpleParser
{
    private readonly string _data;

    public SimpleParser(string data)
    {
        ArgumentNullException.ThrowIfNull(data);
        _data = data;
    }

    /// <summary>
    /// Gets or sets the current parser position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets the next character without consuming it.
    /// </summary>
    public char? NextChar => Position < _data.Length ? _data[Position] : null;

    /// <summary>
    /// Gets whether more characters can be parsed.
    /// </summary>
    public bool HasNext => Position < _data.Length;

    // Returns true if the next char is one of the given chars. Does not consume.
    public bool NextIs(params char[] chars)
    {
        var next = NextChar;
        return next.HasValue && Array.IndexOf(chars, next.Value) >= 0;
    }

    public string ConsumeAny(params char[] chars)
    {
        var sb = new StringBuilder();
        while (HasNext && NextIs(chars))
        {
            sb.Append(_data[Position]);
            Position++;
        }

        return sb.ToString();
    }

    public void Skip(int n)
    {
        Position += n;
    }
}
