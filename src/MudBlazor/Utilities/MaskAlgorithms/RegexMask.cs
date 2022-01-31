﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor;

public class RegexMask : BaseMask
{
    /// <summary>
    /// Create a mask that uses a regex to restrict input.   
    /// </summary>
    /// <param name="regex">
    /// The general or progressive regex to be used for input checking.
    /// 
    /// Note: a general regex must match every possible input, i.e. ^[0-9]+$.
    /// Note: a progressive regex must match even partial input successfully! The
    /// progressive regex must start with ^ and end with $ to work correctly!
    /// 
    /// Example: to match input "abc" a progressive regex must match "a" or "ab" or "abc". The
    /// progressive regex would look like this: ^a(b(c)?)?$ or like this ^(a|ab|abc)$
    /// It is best to generate the progressive regex automatically like BlockMask does.
    /// </param>
    public RegexMask(string regex)
    {
        Mask=regex;
    }
    
    protected Regex _regex; 
    
    /// <summary>
    /// Optional delimiter chars which will be jumped over if the caret is
    /// in front of one and the user inputs the next non-delimiter 
    /// </summary>
    public string Delimiters { get; protected set; }

    protected override void InitInternals()
    {
        base.InitInternals();
        Delimiters ??= "";
        _delimiters = new HashSet<char>(Delimiters);
        InitRegex();
    }

    protected virtual void InitRegex()
    {
        _regex = new Regex(Mask);
    }

    /// <summary>
    /// Inserts given text at caret position
    /// </summary>
    /// <param name="input">One or multiple characters of input</param>
    public override void Insert(string input)
    {
        Init();
        DeleteSelection(align: false);
        var text = Text ?? "";
        var pos = ConsolidateCaret(text, CaretPos);
        (var beforeText, var afterText) = SplitAt(text, pos);
        var alignedInput = AlignAgainstMask(beforeText + input);
        CaretPos = alignedInput.Length;
        UpdateText(AlignAgainstMask(alignedInput + afterText));
    }

    protected override void DeleteSelection(bool align)
    {
        ConsolidateSelection();
        if (Selection == null)
            return;
        var sel = Selection.Value;
        (var s1, _, var s3) = SplitSelection(Text, sel);
        Selection = null;
        CaretPos = sel.Item1;
        if (!align)
            UpdateText(s1 + s3);
        else
            UpdateText(AlignAgainstMask(s1 + s3));
    }

    /// <summary>
    /// Implements the effect of the Del key at the current cursor position
    /// </summary>
    public override void Delete()
    {
        Init();
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
        UpdateText( AlignAgainstMask(beforeText + restText));
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
    public override void Backspace()
    {
        Init();
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
        UpdateText(AlignAgainstMask(restText + afterText));
    }
    
    /// <summary>
    /// Applies the mask to the given text starting at the given offset and returns the masked text. 
    /// </summary>
    /// <param name="text"></param>
    protected virtual string AlignAgainstMask(string text)
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
            else if (Delimiters.Length > 0)
            {
                foreach (var d in Delimiters)
                {
                    if (_regex.IsMatch(alignedText + d + textChar))
                    {
                        alignedText += (d.ToString() + textChar);
                        break;
                    }
                }
            }
            textIndex++;
        }
        return alignedText;
    }
    
    public override void UpdateFrom(IMask other)
    {
        base.UpdateFrom(other);
        var o = other as RegexMask;
        if (o == null)
            return;
        if (Delimiters != o.Delimiters)
        {
            Delimiters = o.Delimiters;
            _initialized = false;
        }
        Refresh();
    }
}
