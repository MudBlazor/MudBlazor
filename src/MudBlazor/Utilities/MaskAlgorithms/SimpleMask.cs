// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudBlazor;

public class SimpleMask : BaseMask
{
    public SimpleMask(string mask)
    {
        Mask = mask;
    }

    public char? Placeholder { get; set; }

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
        var alignedInput = AlignAgainstMask(input, pos);
        CaretPos = pos += alignedInput.Length;
        var alignedAfter = AlignAgainstMask(afterText, pos);
        UpdateText( FillWithPlaceholder(beforeText + alignedInput + alignedAfter));
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
            UpdateText( s1 + s3);
        else
            UpdateText(  s1 + AlignAgainstMask(s3, CaretPos));
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
        var alignedAfter = AlignAgainstMask(restText, pos);
        var numDeleted = afterText.Length - restText.Length;
        if (numDeleted > 1)
        {
            // since we just auto-deleted delimiters which were re-created by AlignAgainstMask we can just as well
            // adjust the cursor position to after the delimiters
            CaretPos += (numDeleted - 1);
        }
        UpdateText( FillWithPlaceholder(beforeText + alignedAfter));
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
        var alignedAfter = AlignAgainstMask(afterText, CaretPos);
        UpdateText( FillWithPlaceholder(restText + alignedAfter));
    }

    /// <summary>
    /// Fill the rest of the text with Placeholder but only if it is set
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected virtual string FillWithPlaceholder(string text)
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
            if (IsDelimiter(maskChar))
                filledText += maskChar;
            else
                filledText += Placeholder.Value;
        }
        return filledText;
    }

    /// <summary>
    /// Applies the mask to the given text starting at the given offset and returns the masked text. 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maskOffset"></param>
    protected virtual string AlignAgainstMask(string text, int maskOffset = 0)
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
            if (IsDelimiter(maskChar))
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
            if (!IsDelimiter(maskChar))
                break;
            alignedText += maskChar;
        }
        return alignedText;
    }
    
    protected virtual bool IsMatch(char maskChar, char textChar)
    {
        if (!_maskDict.TryGetValue(maskChar, out var maskDef))
            return false;
        return Regex.IsMatch(textChar.ToString(), maskDef.Regex);
    }

}
