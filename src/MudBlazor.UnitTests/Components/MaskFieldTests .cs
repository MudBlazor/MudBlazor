// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1998 // async without await
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    public class SimpleMask
    {
        public SimpleMask(string mask)
        {
            Mask = mask;
            _maskDict = _maskChars.ToDictionary(x => x.Char);
        }

        private Dictionary<char, MaskChar> _maskDict;

        private MaskChar[] _maskChars = new MaskChar[]
        {
            MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*'),
            new MaskChar { Char = 'l', AddToValue = false, Regex = "[a-zıäöüßşçğ]" },
            new MaskChar { Char = 'u', AddToValue = false, Regex = "[A-ZİÄÖÜŞÇĞ]" },
        };

        public string Mask { get; }

        public string Text { get; private set; }

        public int CaretPos { get; set; }

        public (int, int)? Selection { get; set; }

        public char? Placeholder { get; set; }

        /// <summary>
        /// Inserts given text at caret position
        /// </summary>
        /// <param name="input">One or multiple characters of input</param>
        public void Insert(string input)
        {
            DeleteSelection(align: false);
            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            (var beforeText, var afterText) = SplitAt(text, pos);
            var alignedInput = AlignAgainstMask(input, pos);
            CaretPos = pos += alignedInput.Length;
            var alignedAfter = AlignAgainstMask(afterText, pos);
            Text = FillWithPlaceholder(beforeText + alignedInput + alignedAfter);
        }

        private void DeleteSelection(bool align)
        {
            ConsolidateSelection();
            if (Selection == null)
                return;
            var sel = Selection.Value;
            var start = ConsolidateCaret(Text, sel.Item1);
            var end = ConsolidateCaret(Text, sel.Item2);
            (var s1, var rest) = SplitAt(Text, start);
            (_, var s3) = SplitAt(rest, end - start);
            Selection = null;
            CaretPos = sel.Item1;
            if (!align)
                Text = s1 + s3;
            else
                Text = s1 + AlignAgainstMask(s3, CaretPos);
        }

        /// <summary>
        /// Implements the effect of the Del key at the current cursor position
        /// </summary>
        public void Delete()
        {
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }

            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            if (pos >= text.Length)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // delete as many delimiters as there are plus one char
            var restText = new string(afterText.SkipWhile(IsTextCharDelimiter).Skip(1).ToArray());
            var alignedAfter = AlignAgainstMask(restText, pos);
            var numDeleted = afterText.Length - restText.Length;
            if (numDeleted > 1)
            {
                // since we just auto-deleted delimiters which were re-created by AlignAgainstMask we can just as well
                // adjust the cursor position to after the delimiters
                CaretPos += (numDeleted - 1);
            }

            Text = FillWithPlaceholder(beforeText + alignedAfter);
        }

        /// <summary>
        /// Implements the effect of the Backspace key at the current cursor position
        /// </summary>
        public void Backspace()
        {
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }

            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            if (pos == 0)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // backspace as many delimiters as there are plus one char
            var restText = new string(beforeText.Reverse().SkipWhile(IsTextCharDelimiter).Skip(1).Reverse().ToArray());
            var numDeleted = beforeText.Length - restText.Length;
            CaretPos -= numDeleted;
            var alignedAfter = AlignAgainstMask(afterText, CaretPos);
            Text = FillWithPlaceholder(restText + alignedAfter);
        }

        /// <summary>
        /// Fill the rest of the text with Placeholder but only if it is set
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FillWithPlaceholder(string text)
        {
            if (Placeholder == null)
                return text;
            // fill the rest with placeholder
            // don't fill if text is still empty
            var filledText = text;
            var len = text.Length;
            var mask = Mask ?? "";
            if (len == 0 || len >= mask.Length)
                return text;
            for (var maskIndex = len; maskIndex < mask.Length; maskIndex++)
            {
                var maskChar = mask[maskIndex];
                if (IsMaskCharDelimiter(maskChar))
                    filledText += maskChar;
                else
                    filledText += Placeholder.Value;
            }

            return filledText;
        }

        internal static (string, string) SplitAt(string text, int pos)
        {
            if (pos <= 0)
                return ("", text);
            if (pos >= text.Length)
                return (text, "");
            return (text.Substring(0, pos), text.Substring(pos));
        }

        /// <summary>
        /// Applies the mask to the given text starting at the given offset and returns the masked text. 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maskOffset"></param>
        private string AlignAgainstMask(string text, int maskOffset = 0)
        {
            text ??= "";
            var mask = Mask ?? "";
            var alignedText = "";
            var maskIndex = maskOffset; // index in mask
            var textIndex = 0; // index in text
            while (textIndex < text.Length)
            {
                if (maskIndex >= mask.Length)
                    break;
                var maskChar = mask[maskIndex];
                var textChar = text[textIndex];
                if (IsMaskCharDelimiter(maskChar))
                {
                    alignedText += maskChar;
                    maskIndex++;
                    continue;
                }

                if (IsMatch(maskChar, textChar))
                {
                    alignedText += textChar;
                    maskIndex++;
                }

                textIndex++;
            }

            // fill any delimiters if possible
            for (int i = maskIndex; i < mask.Length; i++)
            {
                var maskChar = mask[i];
                if (!IsMaskCharDelimiter(maskChar))
                    break;
                alignedText += maskChar;
            }

            return alignedText;
        }

        private bool IsMaskCharDelimiter(char maskChar)
        {
            if (!_maskDict.TryGetValue(maskChar, out var maskDef))
                return true;
            return false;
        }

        private bool IsTextCharDelimiter(char textChar)
        {
            var result = !_maskChars.Any(maskDef => Regex.IsMatch(textChar.ToString(), maskDef.Regex));
            return result;
        }

        private bool IsMatch(char maskChar, char textChar)
        {
            if (!_maskDict.TryGetValue(maskChar, out var maskDef))
                return false;
            return Regex.IsMatch(textChar.ToString(), maskDef.Regex);
        }

        private int ConsolidateCaret(string text, int caretPos)
        {
            if (string.IsNullOrEmpty(text) || caretPos < 0)
                return 0;
            if (caretPos < text.Length)
                return caretPos;
            return text.Length;
        }

        public void Clear()
        {
            Text = "";
            CaretPos = 0;
            Selection = null;
        }

        private void ConsolidateSelection()
        {
            if (Selection == null)
                return;
            var sel = Selection.Value;
            if (sel.Item1 == sel.Item2)
            {
                CaretPos = sel.Item1;
                Selection = null;
                return;
            }

            if (sel.Item1 < 0)
                sel.Item1 = 0;
            if (sel.Item2 >= Text.Length)
                sel.Item2 = Text.Length;
        }

        public override string ToString()
        {
            var text = Text ?? "";
            ConsolidateSelection();
            if (Selection == null)
            {
                var pos = ConsolidateCaret(text, CaretPos);
                if (pos < text.Length)
                    return text.Insert(pos, "|");
                return text + "|";
            }
            else
            {
                var sel = Selection.Value;
                var start = ConsolidateCaret(text, sel.Item1);
                var end = ConsolidateCaret(text, sel.Item2);
                (var s1, var rest) = SplitAt(text, start);
                (var s2, var s3) = SplitAt(rest, end - start);
                return s1 + "[" + s2 + "]" + s3;
            }
        }
    }


    public record struct Block(char MaskChar, int Min = 1, int Max = 1);

    public class BlockMask
    {
        public BlockMask(string delimiters, params Block[] blocks)
        {
            if (blocks.Length == 0)
                throw new ArgumentException("supply at least one block", nameof(blocks));
            Delimiters = delimiters ?? "";
            Blocks = blocks;
            Init();
        }

        public Block[] Blocks { get; private set; }

        private Dictionary<char, MaskChar> _maskDict;

        private MaskChar[] _maskChars = new MaskChar[]
        {
            MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*'),
            new MaskChar { Char = 'l', AddToValue = false, Regex = "[a-zıäöüßşçğ]" },
            new MaskChar { Char = 'u', AddToValue = false, Regex = "[A-ZİÄÖÜŞÇĞ]" },
        };

        private void Init()
        {
            _delimiters = new HashSet<char>(Delimiters ?? "");
            _maskDict = _maskChars.ToDictionary(x => x.Char);
            // this must be last
            Mask = BuildRegex(Blocks ?? new Block[0]);
            _regex = new Regex(Mask);
            //_delimiterRegex = new Regex("[" + string.Join("", Delimiters.Select(x => Regex.Escape(x.ToString())).ToArray()) + "]");
        }

        private string BuildRegex(Block[] blocks)
        {
            var s = new StringBuilder();
            int i = 0;
            int blockIndex = 0;
            s.Append("^");
            foreach (var b in blocks)
            {
                for (int j = 0; j < b.Min; j++)
                {
                    if (i > 0)
                        s.Append("(");
                    if (_maskDict.TryGetValue(b.MaskChar, out var maskDef))
                        s.Append(maskDef.Regex);
                    else
                        s.Append(Regex.Escape(b.MaskChar.ToString()));
                }

                i += b.Min;
                if (b.Max > b.Min)
                {
                    for (int j = b.Min; j < b.Max; j++)
                    {
                        s.Append("(");
                        if (_maskDict.TryGetValue(b.MaskChar, out var maskDef))
                            s.Append(maskDef.Regex);
                        else
                            s.Append(Regex.Escape(b.MaskChar.ToString()));
                    }

                    for (int j = b.Min; j < b.Max; j++)
                        s.Append(")?");
                }

                if (_delimiters.Count > 0 && blockIndex < blocks.Length - 1)
                {
                    s.Append("([");
                    foreach (var d in _delimiters)
                        s.Append(Regex.Escape(d.ToString()));
                    s.Append("]");
                    i++;
                }

                blockIndex++;
            }

            for (int j = 0; j < i - 1; j++)
                s.Append(")?");
            s.Append("$");
            return s.ToString();
        }

        private Regex _regex; //, _delimiterRegex;
        private HashSet<char> _delimiters;

        public string Mask { get; private set; }

        public string Text { get; private set; }

        public int CaretPos { get; set; }

        public (int, int)? Selection { get; set; }

        public string Delimiters { get; }

        /// <summary>
        /// Inserts given text at caret position
        /// </summary>
        /// <param name="input">One or multiple characters of input</param>
        public void Insert(string input)
        {
            DeleteSelection(align: false);
            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            (var beforeText, var afterText) = SplitAt(text, pos);
            var alignedInput = AlignAgainstMask(beforeText + input);
            CaretPos = alignedInput.Length;
            //Text = AlignAgainstMask(alignedInput + _delimiterRegex.Replace(afterText, ""));
            Text = AlignAgainstMask(alignedInput + afterText);
        }

        private void DeleteSelection(bool align)
        {
            ConsolidateSelection();
            if (Selection == null)
                return;
            var sel = Selection.Value;
            var start = ConsolidateCaret(Text, sel.Item1);
            var end = ConsolidateCaret(Text, sel.Item2);
            (var s1, var rest) = SplitAt(Text, start);
            (_, var s3) = SplitAt(rest, end - start);
            Selection = null;
            CaretPos = sel.Item1;
            if (!align)
                Text = s1 + s3;
            else
                Text = AlignAgainstMask(s1 + s3);
        }

        /// <summary>
        /// Implements the effect of the Del key at the current cursor position
        /// </summary>
        public void Delete()
        {
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }

            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            if (pos >= text.Length)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // delete as many delimiters as there are plus one char
            var restText = new string(afterText.SkipWhile(IsDelimiter).Skip(1).ToArray());
            Text = AlignAgainstMask(beforeText + restText);
            var numDeleted = afterText.Length - restText.Length;
            if (numDeleted > 1)
            {
                // since we just auto-deleted delimiters which were re-created by AlignAgainstMask we can just as well
                // adjust the cursor position to after the delimiters
                CaretPos += (numDeleted - 1);
            }
        }

        /// <summary>
        /// Implements the effect of the Backspace key at the current cursor position
        /// </summary>
        public void Backspace()
        {
            if (Selection != null)
            {
                DeleteSelection(align: true);
                return;
            }

            var text = Text ?? "";
            var pos = ConsolidateCaret(text, CaretPos);
            if (pos == 0)
                return;
            (var beforeText, var afterText) = SplitAt(text, pos);
            // backspace as many delimiters as there are plus one char
            var restText = new string(beforeText.Reverse().SkipWhile(IsDelimiter).Skip(1).Reverse().ToArray());
            var numDeleted = beforeText.Length - restText.Length;
            CaretPos -= numDeleted;
            Text = AlignAgainstMask(restText + afterText);
        }

        internal static (string, string) SplitAt(string text, int pos)
        {
            if (pos <= 0)
                return ("", text);
            if (pos >= text.Length)
                return (text, "");
            return (text.Substring(0, pos), text.Substring(pos));
        }

        /// <summary>
        /// Applies the mask to the given text starting at the given offset and returns the masked text. 
        /// </summary>
        /// <param name="text"></param>
        private string AlignAgainstMask(string text)
        {
            text ??= "";
            var alignedText = "";
            var textIndex = 0; // index in text
            while (textIndex < text.Length)
            {
                var textChar = text[textIndex];
                if (_regex.IsMatch(alignedText + textChar))
                    alignedText += textChar;
                // try to skip over a delimiter (input of values only i.e. 31122021 => 31.12.2021)
                else if (Delimiters.Length > 0 && _regex.IsMatch(alignedText + Delimiters[0] + textChar))
                {
                    alignedText += (Delimiters[0].ToString() + textChar);
                }

                textIndex++;
            }

            return alignedText;
        }

        private bool IsDelimiter(char maskChar)
        {
            return _delimiters.Contains(maskChar);
        }

        private int ConsolidateCaret(string text, int caretPos)
        {
            if (string.IsNullOrEmpty(text) || caretPos < 0)
                return 0;
            if (caretPos < text.Length)
                return caretPos;
            return text.Length;
        }

        public void Clear()
        {
            Text = "";
            CaretPos = 0;
            Selection = null;
        }

        private void ConsolidateSelection()
        {
            if (Selection == null)
                return;
            var sel = Selection.Value;
            if (sel.Item1 == sel.Item2)
            {
                CaretPos = sel.Item1;
                Selection = null;
                return;
            }

            if (sel.Item1 < 0)
                sel.Item1 = 0;
            if (sel.Item2 >= Text.Length)
                sel.Item2 = Text.Length;
        }

        public override string ToString()
        {
            var text = Text ?? "";
            ConsolidateSelection();
            if (Selection == null)
            {
                var pos = ConsolidateCaret(text, CaretPos);
                if (pos < text.Length)
                    return text.Insert(pos, "|");
                return text + "|";
            }
            else
            {
                var sel = Selection.Value;
                var start = ConsolidateCaret(text, sel.Item1);
                var end = ConsolidateCaret(text, sel.Item2);
                (var s1, var rest) = SplitAt(text, start);
                (var s2, var s3) = SplitAt(rest, end - start);
                return s1 + "[" + s2 + "]" + s3;
            }
        }
    }


    [TestFixture]
    public class MaskFieldTests : BunitTest
    {
        [Test]
        public async Task BlockMask_Insert()
        {
            var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
            mask.ToString().Should().Be("|");
            mask.Insert("12.");
            mask.ToString().Should().Be("12.|");
            mask.Clear();
            mask.Insert("xx12.34xx.5678");
            mask.Text.Should().Be("12.34.5678");
            mask.Clear();
            mask.Insert("1.1.99");
            mask.ToString().Should().Be("1.1.99|");
            mask.CaretPos = 0;
            mask.Insert("0");
            mask.ToString().Should().Be("0|1.1.99");
            mask.Insert("0");
            mask.ToString().Should().Be("00|.1.199");
            mask.Insert("0");
            mask.ToString().Should().Be("00.0|.1199");
            mask.Insert("0");
            mask.ToString().Should().Be("00.00|.1199");
            // w/o separator
            mask = new BlockMask("", new Block('0', 1, 2), new Block('a', 1, 2), new Block('0', 2, 4));
            mask.Insert("xx12.34xx.5678");
            mask.Text.Should().Be("12xx5678");
            mask.Clear();
            mask.Insert("1.x.99");
            mask.ToString().Should().Be("1x99|");
            mask.CaretPos = 0;
            mask.Insert("0");
            mask.ToString().Should().Be("0|1x99");
            mask.Insert("0");
            mask.ToString().Should().Be("00|x99");
            mask.Insert("y");
            mask.ToString().Should().Be("00y|x99");
            mask.Insert("z");
            mask.ToString().Should().Be("00yz|99");
            mask.Insert("1");
            mask.ToString().Should().Be("00yz1|99");
        }

        [Test]
        public async Task BlockMask_Delete()
        {
            var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
            mask.ToString().Should().Be("|");
            mask.Insert("12.34.5678");
            mask.ToString().Should().Be("12.34.5678|");
            mask.Delete();
            mask.ToString().Should().Be("12.34.5678|");
            mask.CaretPos = 0;
            mask.Delete();
            mask.ToString().Should().Be("|2.34.5678");
            mask.Delete();
            mask.ToString().Should().Be("|34.56.78");
        }
        
        [Test]
        public async Task BlockMask_Backspace()
        {
            var mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
            mask.ToString().Should().Be("|");
            mask.Insert("12.34.5678");
            mask.ToString().Should().Be("12.34.5678|");
            mask.Backspace();
            mask.ToString().Should().Be("12.34.567|");
            mask.CaretPos = 3;
            mask.ToString().Should().Be("12.|34.567");
            mask.Backspace();
            mask.ToString().Should().Be("1|3.4.567");
            mask.Backspace();
            mask.ToString().Should().Be("|3.4.567");
            mask.Backspace();
            mask.ToString().Should().Be("|3.4.567");
        }

        [Test]
        public async Task BlockMask_Internals()
        {
            var mask = new BlockMask(".", new Block('('), new Block('0', 2, 2), new Block(')'));
            mask.Mask.Should().Be(@"^\(([\.](\d(\d([\.](\))?)?)?)?)?$");
            mask = new BlockMask(".", new Block('0', 1, 2), new Block('0', 1, 2), new Block('0', 2, 4));
            mask.Mask.Should().Be(@"^\d(\d)?([\.](\d(\d)?([\.](\d(\d(\d(\d)?)?)?)?)?)?)?$");
        }


        [Test]
        public async Task SimpleMask_Insert()
        {
            var mask = new SimpleMask("(aa) 00-0");
            mask.ToString().Should().Be("|");
            mask.Insert("ab123");
            mask.Text.Should().Be("(ab) 12-3");
            mask.ToString().Should().Be("(ab) 12-3|");
            mask.CaretPos = 2;
            mask.ToString().Should().Be("(a|b) 12-3");
            mask.Insert("x");
            mask.ToString().Should().Be("(ax) |12-3");
            mask.Text.Should().Be("(ax) 12-3");
            mask.Insert("9");
            mask.ToString().Should().Be("(ax) 9|1-2");
            mask.Text.Should().Be("(ax) 91-2");
            mask.Insert("99");
            mask.ToString().Should().Be("(ax) 99-9|");
            mask.Text.Should().Be("(ax) 99-9");
            mask.Insert("xyz1234");
            mask.ToString().Should().Be("(ax) 99-9|");
            mask.Text.Should().Be("(ax) 99-9");
            mask.Clear();
            mask.ToString().Should().Be("|");
            mask.Text.Should().Be("");
            mask.Insert("1");
            mask.ToString().Should().Be("(|");
            mask.Text.Should().Be("(");
            mask.Insert("x");
            mask.ToString().Should().Be("(x|");
            mask.Text.Should().Be("(x");
            mask.Insert("y");
            mask.ToString().Should().Be("(xy) |");
            mask.Text.Should().Be("(xy) ");
            mask.Insert("z");
            mask.ToString().Should().Be("(xy) |");
            mask.Text.Should().Be("(xy) ");
            // paste
            mask.Clear();
            mask.Insert("(XX) 99-9");
            mask.ToString().Should().Be("(XX) 99-9|");
        }

        [Test]
        public async Task SimpleMask_AutoFilling()
        {
            var mask = new SimpleMask("---0---");
            mask.ToString().Should().Be("|");
            mask.Insert("1");
            mask.Text.Should().Be("---1---");
            mask.ToString().Should().Be("---1---|");
            mask.CaretPos = 1;
            mask.ToString().Should().Be("-|--1---");
            mask.Insert("x");
            mask.Text.Should().Be("---1---");
            mask.ToString().Should().Be("---|1---");
            mask.Insert("9");
            mask.Text.Should().Be("---9---");
            mask.ToString().Should().Be("---9---|");
        }

        [Test]
        public async Task SimpleMask_Placeholder()
        {
            var mask = new SimpleMask("(+00) 000 0000") { Placeholder = '_' };
            mask.ToString().Should().Be("|");
            mask.Insert("43");
            mask.Text.Should().Be("(+43) ___ ____");
            mask.ToString().Should().Be("(+43) |___ ____");
            mask.Insert("abc123");
            mask.ToString().Should().Be("(+43) 123 |____");
            mask.Insert("5678901234");
            mask.ToString().Should().Be("(+43) 123 5678|");
            // del key
            mask.Delete();
            mask.ToString().Should().Be("(+43) 123 5678|");
            mask.CaretPos = 0;
            mask.ToString().Should().Be("|(+43) 123 5678");
            mask.Delete();
            mask.ToString().Should().Be("(+|31) 235 678_");
            mask.Delete();
            mask.ToString().Should().Be("(+|12) 356 78__");
            mask.Insert("430");
            mask.ToString().Should().Be("(+43) 0|12 3567");
        }

        [Test]
        public async Task SimpleMask_Delete()
        {
            var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
            mask.ToString().Should().Be("|");
            mask.Insert("43");
            mask.Text.Should().Be("(+43) ");
            mask.ToString().Should().Be("(+43) |");
            mask.Insert("abc123");
            mask.ToString().Should().Be("(+43) 123 |");
            mask.Insert("5678901234");
            mask.ToString().Should().Be("(+43) 123 5678|");
            // del key
            mask.Delete();
            mask.ToString().Should().Be("(+43) 123 5678|");
            mask.CaretPos = 0;
            mask.ToString().Should().Be("|(+43) 123 5678");
            mask.Delete();
            mask.ToString().Should().Be("(+|31) 235 678");
            mask.Delete();
            mask.ToString().Should().Be("(+|12) 356 78");
            mask.Insert("430");
            mask.ToString().Should().Be("(+43) 0|12 3567");
        }

        [Test]
        public async Task SimpleMask_Backspace()
        {
            var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
            mask.ToString().Should().Be("|");
            mask.Insert("43abc1235678901234");
            mask.ToString().Should().Be("(+43) 123 5678|");
            // Backspace key
            mask.Backspace();
            mask.ToString().Should().Be("(+43) 123 567|");
            mask.CaretPos = 0;
            mask.ToString().Should().Be("|(+43) 123 567");
            mask.Backspace();
            mask.ToString().Should().Be("|(+43) 123 567");
            mask.CaretPos = 6;
            mask.ToString().Should().Be("(+43) |123 567");
            mask.Backspace();
            mask.ToString().Should().Be("(+4|1) 235 67");
            mask.Backspace();
            mask.ToString().Should().Be("(+|12) 356 7");
            mask.Backspace();
            mask.ToString().Should().Be("|(+12) 356 7");
            mask.Insert("4309");
            mask.ToString().Should().Be("(+43) 09|1 2356");
        }

        [Test]
        public async Task SimpleMask_Selection()
        {
            var mask = new SimpleMask("(+00) 000 0000"); // no placeholder
            mask.ToString().Should().Be("|");
            mask.Insert("43abc1235678901234");
            mask.ToString().Should().Be("(+43) 123 5678|");
            // set selection
            mask.Selection = (-1, 111);
            mask.ToString().Should().Be("[(+43) 123 5678]");
            mask.CaretPos = 0;
            mask.Selection = (1, 1);
            mask.ToString().Should().Be("(|+43) 123 5678");
            mask.Selection = (3, 11);
            mask.ToString().Should().Be("(+4[3) 123 5]678");
            // input with selection
            mask.Insert("9");
            mask.ToString().Should().Be("(+49) |678 ");
            mask.Selection = (0, 6);
            mask.ToString().Should().Be("[(+49) ]678 ");
            mask.Insert("01");
            mask.ToString().Should().Be("(+01) |678 ");
            // del with selection
            mask.Selection = (0, 6);
            mask.ToString().Should().Be("[(+01) ]678 ");
            mask.Delete();
            mask.ToString().Should().Be("|(+67) 8");
            // backspace with selection
            mask.Selection = (0, 6);
            mask.ToString().Should().Be("[(+67) ]8");
            mask.Backspace();
            mask.ToString().Should().Be("|(+8");
        }

        [Test]
        public async Task SimpleMask_Internals()
        {
            SimpleMask.SplitAt("asdf", 1).Should().Be(("a", "sdf"));
            SimpleMask.SplitAt("", 1).Should().Be(("", ""));
            SimpleMask.SplitAt("asdf", -1).Should().Be(("", "asdf"));
            SimpleMask.SplitAt("asdf", 10).Should().Be(("asdf", ""));
        }

        /// <summary>
        /// Test all IsMatch variants: letter, digit and symbols.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task MaskFieldTest_Fundamentals()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();
            var impl = maskField.Instance.Mask;
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(0));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Symbols should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            //Check uppercase character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(6));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "d" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));
            //Symbols should have no effect in digit
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "+" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));
            //Symbol as a mask character should have no effect in letter
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "*" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(10));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-A_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120A"));
            //Check culture character
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ı" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-Aı"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120Aı"));
            //Keys should have no effect if the mask completed
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Z" }));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-Aı"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120Aı"));
            //Middle input should move the after characters
            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(9));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bA"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bA"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(11));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-bc"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120bc"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-b_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120b"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(11));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 120-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC120"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC12"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(8));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC1"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(3));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(1));
            //Backspace should have no effect on empty value
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.SetBothValueAndText("abc120ac"));
            //await comp.InvokeAsync(() => maskField.Instance.Value = "abc120ac");
            //await comp.InvokeAsync(() => maskField.Instance.SetRawValueDictionary("abc120ac"));
            //await comp.InvokeAsync(() => maskField.Instance.ImplementMask(null, maskField.Instance.Mask));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(1));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(0));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 20_-c_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc20c"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(7));
            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(bc_) 0__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("bc0"));

            await comp.InvokeAsync(() => maskField.Instance.Clear());
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(impl.FindLastCaretLocation()));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(11));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(10));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowLeft" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(8));

            await comp.InvokeAsync(() =>
                maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowRight" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(10));

            await comp.InvokeAsync(() =>
                maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "ArrowRight" }));
            comp.WaitForAssertion(() => maskField.Instance.Mask.CaretPosition.Should().Be(11));
        }

        [Test]
        public async Task MaskFieldTest_Int()
        {
            var comp = Context.RenderComponent<MaskFieldIntTest>();

            var maskField = comp.FindComponent<MudMaskField<int>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
            //Unmatched keys should have no effect
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("1"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("12"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));

            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-3"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("123"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(123));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)2-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("12"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(12));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(1)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.GetRawValue().Should().Be("1"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(1));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(_)_-_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(0));
        }

        [Test]
        public async Task MaskFieldTest_PlaceholderCheck()
        {
            var comp = Context.RenderComponent<MaskFieldStringPlaceholderTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(
                () => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be(""));

            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be(null));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.OnBlurred(new FocusEventArgs()));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));
        }

        [Test]
        public async Task MaskFieldTest_InsertCharactersIntoMiddle()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) 1__-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(10));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(___) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("1a"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a1a"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ba_) 1__-a_"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ba1a"));
        }

        [Test]
        public async Task MaskFieldTest_ChangeMaskCharacters()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.Instance.Mask.MaskDefinition = new MaskChar[]
            {
                MaskChar.Letter('b'), MaskChar.Digit('9'), MaskChar.LetterOrDigit('+'),
                new MaskChar { Char = 'l', AddToValue = false },
            });

            await comp.InvokeAsync(() => maskField.Instance.Mask.Mask = "(bb+) 999-bb");
            await comp.InvokeAsync(() => maskField.Instance.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(2));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ImplementMask(null));
            await comp.InvokeAsync(() => maskField.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ab"));
        }

        [Test]
        public async Task MaskFieldTest_RegexMasking()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.Mask.Mask = "(ll) uu");
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(aa) __"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("aa"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "A" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(aa) A_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("aaA"));
        }

        [Test]
        public async Task MaskFieldTest_KeepCharacterPositions()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => comp.SetParam("Mask", "(aaa) 000-aa"));
            await comp.InvokeAsync(() => comp.SetParam("KeepCharacterPositions", true));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("a"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ab"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "c" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(abc) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("abc"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(3));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) ___-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) 1__-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac1"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "0" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(a_c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("ac10"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(__c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("c10"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(__c) 10_-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("c10"));
        }

        [Test]
        public async Task MaskFieldTest_Paste()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField = comp.FindComponent<MudMaskField<string>>();

            await comp.InvokeAsync(() => maskField.Instance.Value = "abc");
            await comp.InvokeAsync(() => maskField.Instance.Mask.SetRawValueDictionary("abc"));
            await comp.InvokeAsync(() => maskField.Instance.Mask.ImplementMask(null));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(10));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("zxc"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(abc) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("abczx"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(2));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("defgh"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) ___-zx"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("adezx"));

            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(7));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("120"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(ade) _12-_x"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("ade12x"));
            //Symbols should not be paste but remove the related index
            await comp.InvokeAsync(() => maskField.Instance.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.Instance.OnPaste("+-"));
            comp.WaitForAssertion(() => maskField.Instance.Text.Should().Be("(__e) _12-_x"));
            comp.WaitForAssertion(() => maskField.Instance.Value.Should().Be("e12x"));
        }

        [Test]
        public async Task MaskFieldTest_Selection()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;

            await comp.InvokeAsync(() => maskField.Mask.Mask = "0000 0000 000");
            await comp.InvokeAsync(() => maskField.Value = "1234567899");
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.Mask.SetRawValueDictionary("1234567899"));
            await comp.InvokeAsync(() => maskField.Mask.ImplementMask(null));
            //await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(12));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "9" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 5678 999"));
            //Select and delete
            await comp.InvokeAsync(() => maskField.OnSelect(10, 12));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 5678 9__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123456789"));
            //Select with a whitespace and test again
            await comp.InvokeAsync(() => maskField.OnSelect(4, 8));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 89__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(7));
            await comp.InvokeAsync(() => maskField.OnSelect(7, 11));
            await comp.InvokeAsync(() => maskField.OnPaste("567"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("_234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("23489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 8956 7__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123489567"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(6));
            await comp.InvokeAsync(() => maskField.OnSelect(6, 11));
            await comp.InvokeAsync(() => maskField.OnPaste("1Mud9"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1234 81__ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1234819"));

            await comp.InvokeAsync(() => maskField.Mask.KeepCharacterPositions = true);
            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.OnSelect(1, 3));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1__4 81__ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("14819"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(3));
            await comp.InvokeAsync(() => maskField.OnSelect(3, 7));
            await comp.InvokeAsync(() => maskField.OnPaste("a1a"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1___ 1___ _9_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("119"));
        }

        [Test]
        public async Task MaskFieldTest_TwoWayBinding()
        {
            var comp = Context.RenderComponent<MaskFieldStringTest>();
            var maskField1 = comp.FindComponents<MudMaskField<string>>().First();
            var maskField2 = comp.FindComponents<MudMaskField<string>>().Last();
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be(""));

            // input in maskField1
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "a" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("a"));

            // check maskField2
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(a__) ___-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("a"));

            // input in maskField1
            await comp.InvokeAsync(() => maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "b" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("ab"));

            // check maskField2
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(ab_) ___-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("ab"));

            // input in maskField2
            await comp.InvokeAsync(() => maskField2.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "C" }));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC"));
            comp.WaitForAssertion(() => maskField2.Instance.Mask.CaretPosition.Should().Be(6));

            // check maskField1
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC"));

            // input in maskField2
            await comp.InvokeAsync(() => maskField2.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC1"));

            // check maskField1
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC1"));

            // input in maskField2
            await comp.InvokeAsync(() => maskField2.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC12"));

            // check maskField1
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 12_-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC12"));

            // input in maskField1
            await comp.InvokeAsync(() =>
                maskField1.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC1"));

            // check maskField2
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 1__-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC1"));

            // input in maskField2
            await comp.InvokeAsync(() =>
                maskField2.Instance.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC"));

            // check maskField1
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) ___-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC"));

            await comp.InvokeAsync(() => maskField1.Instance.OnPaste("123"));
            comp.WaitForAssertion(() => maskField1.Instance.Text.Should().Be("(abC) 123-__"));
            comp.WaitForAssertion(() => maskField1.Instance.Value.Should().Be("abC123"));
            comp.WaitForAssertion(() => maskField2.Instance.Text.Should().Be("(abC) 123-__"));
            comp.WaitForAssertion(() => maskField2.Instance.Value.Should().Be("abC123"));
        }

        [Test]
        public async Task MaskFieldTest_CaretPosition()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;
            var impl = maskField.Mask;

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindFirstCaretLocation(true)));
            comp.WaitForAssertion(() => maskField.Mask.CaretPosition.Should().Be(0));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindLastCaretLocation(true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(0));

            await comp.InvokeAsync(() =>
                maskField.SetCaretPosition(impl.FindPreviousCaretLocation(impl.CaretPosition)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(0));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindNextCaretLocation(impl.CaretPosition)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(0));

            comp.SetParam("Mask", "(000) 000-00-00");
            await comp.InvokeAsync(() => maskField.SetCaretPosition(1));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(1__) ___-__-__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(14));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("(1__) ___-__-_1"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("11"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindFirstCaretLocation(true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindFirstCaretLocation(false)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(1));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindLastCaretLocation(true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(13));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindLastCaretLocation(false)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(14));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindPreviousCaretLocation(2, false)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(1));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindPreviousCaretLocation(3, true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindNextCaretLocation(13, false)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(14));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindNextCaretLocation(13, true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(15));

            comp.SetParam("Mask", "()");
            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindFirstCaretLocation(true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(0));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindLastCaretLocation(true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindPreviousCaretLocation(2, true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindNextCaretLocation(2, true)));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));
        }

        [Test]
        public async Task MaskFieldTest_AddToValue()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;

            maskField.Mask.MaskDefinition = new MaskChar[]
            {
                MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*'),
                new MaskChar { Char = 'l', AddToValue = false, Regex = "^[a-zıöüşçğ]$" },
                new MaskChar { Char = 'u', AddToValue = false, Regex = "^[A-ZİÖÜŞÇĞ]$" },
                new MaskChar { Char = ':', AddToValue = true },
            };

            maskField.Mask.Mask = "00:00";

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1_:__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1:"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("12:"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "3" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:3_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("12:3"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "4" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("12:34"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("12:34"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(2));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("13:4_"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("13:4"));

            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Delete" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("14:__"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("14:"));
        }

        [Test]
        public async Task MaskFieldTest_Extreme()
        {
            var comp = Context.RenderComponent<MudMaskField<string>>();
            var maskField = comp.Instance;
            var impl = maskField.Mask;
            comp.WaitForAssertion(() => maskField.Text.Should().Be(null));
            await comp.InvokeAsync(() => maskField.OnPaste("abc"));
            comp.WaitForAssertion(() => maskField.Text.Should().Be(null));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => comp.SetParam("Placeholder", "Some Placeholder"));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "Backspace" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be(null));

            await comp.InvokeAsync(() => maskField.OnBlurred(new FocusEventArgs()));
            comp.WaitForAssertion(() => maskField.Text.Should().Be(""));

            comp.WaitForAssertion(() => maskField.GetRawValue().Should().Be(""));

            //comp.WaitForAssertion(() => maskField.GetCharacterType("").Should().Be(CharacterType.None));

            comp.WaitForAssertion(() => maskField.GetInputType().Should().Be(InputType.Text));

            await comp.InvokeAsync(() => maskField.OnCaretPositionChanged(2));
            comp.WaitForAssertion(() => impl.CaretPosition.Should().Be(2));

            await comp.InvokeAsync(() => maskField.Text = "");
            comp.WaitForAssertion(() => impl.GetRawValueFromText(maskField.Text).Should().Be(""));

            comp.SetParam("Mask", "*00 000");

            await comp.InvokeAsync(() => maskField.OnCopy());
            await comp.InvokeAsync(() => maskField.FocusAsync());
            await comp.InvokeAsync(() => maskField.SetCaretPosition(impl.FindFirstCaretLocation(false)));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "1" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("1__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("1"));

            await comp.InvokeAsync(() => maskField.SelectAsync());
            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.SelectRangeAsync(0, 7));
            await comp.InvokeAsync(() => maskField.OnSelect(0, 7));
            await comp.InvokeAsync(() => maskField.HandleKeyDown(new KeyboardEventArgs() { Key = "2" }));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("2__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("2"));

            await comp.InvokeAsync(() => impl.ImplementMask(null));
            comp.WaitForAssertion(() => maskField.Text.Should().Be("2__ ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("2"));

            await comp.InvokeAsync(() => maskField.SetCaretPosition(0));
            await comp.InvokeAsync(() => maskField.OnFocused(new FocusEventArgs()));
            await comp.InvokeAsync(() => maskField.SetBothValueAndText("123"));
            //comp.WaitForAssertion(() => maskField.Text.Should().Be("123 ___"));
            comp.WaitForAssertion(() => maskField.Value.Should().Be("123"));
        }
    }
}
