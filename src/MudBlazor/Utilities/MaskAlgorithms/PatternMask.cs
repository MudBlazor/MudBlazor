// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudBlazor;

public class PatternMask : BaseMask
{
    public PatternMask(string mask)
    {
        Mask = mask;
    }

    /// <summary>
    /// If set, the mask will print placeholders for all non-delimiters that haven't yet been typed.
    /// For instance a mask "000-000" with input "1" will show "1__-___" as Text.
    /// </summary>
    public char? Placeholder { get; set; }

    /// <summary>
    /// A function for changing input characters after they were typed, i.e. lower-case to upper-case, etc.
    /// </summary>
    public Func<char, char> Transformation { get; set; }

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
        var alignedBefore = AlignAgainstMask(beforeText, 0);
        CaretPos = pos = alignedBefore.Length;
        var alignedInput = AlignAgainstMask(input, pos);
        CaretPos = pos += alignedInput.Length;
        if (Placeholder != null)
        {
            var p = Placeholder.Value;
            if (afterText.Take(alignedInput.Length).All(c => IsDelimiter(c) || c == p))
                afterText = new string(afterText.Skip(alignedInput.Length).ToArray());
        }
        var alignedAfter = AlignAgainstMask(afterText, pos);
        UpdateText(FillWithPlaceholder(alignedBefore + alignedInput + alignedAfter));
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
            UpdateText(FillWithPlaceholder(s1 + AlignAgainstMask(s3, CaretPos)));
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
        var pos = CaretPos = ConsolidateCaret(text, CaretPos);
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
        UpdateText(FillWithPlaceholder(beforeText + alignedAfter));
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
        var pos = CaretPos = ConsolidateCaret(text, CaretPos);
        if (pos == 0)
            return;
        (var beforeText, var afterText) = SplitAt(text, pos);
        // backspace as many delimiters as there are plus one char
        var restText = new string(beforeText.Reverse().SkipWhile(IsDelimiter).Skip(1).Reverse().ToArray());
        var numDeleted = beforeText.Length - restText.Length;
        CaretPos -= numDeleted;
        var alignedAfter = AlignAgainstMask(afterText, CaretPos);
        UpdateText(FillWithPlaceholder(restText + alignedAfter));
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
                ModifyPartiallyAlignedMask(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
                continue;
            }
            var isPlaceholder = Placeholder != null && textChar == Placeholder.Value;
            if (IsMatch(maskChar, textChar) || isPlaceholder)
            {
                var c = Transformation == null ? textChar : Transformation(textChar);
                alignedText += c;
                maskIndex++;
            }
            textIndex++;
            ModifyPartiallyAlignedMask(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        }
        // fill any delimiters if possible
        for (var i = maskIndex; i < mask.Length; i++)
        {
            var maskChar = mask[i];
            if (!IsDelimiter(maskChar))
                break;
            alignedText += maskChar;
        }
        return alignedText;
    }

    protected virtual void ModifyPartiallyAlignedMask(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        /* this is an override hook for more specialized mask implementations deriving from this*/
    }

    protected virtual bool IsMatch(char maskChar, char textChar)
    {
        var maskDef = _maskDict[maskChar];
        return Regex.IsMatch(textChar.ToString(), maskDef.Regex);
    }

    /// <summary>
    /// If true, all characters which are not defined in the mask (delimiters) are stripped
    /// from text. 
    /// </summary>
    public bool CleanDelimiters { get; set; }

    /// <summary>
    /// Return the Text without Placeholders. If CleanDelimiters is enabled, then also strip all
    /// undefined characters. For instance, for a mask "0000 0000 0000 0000" the space would be
    /// an undefined character (a delimiter) unless it were defined as a mask character in MaskChars.
    /// </summary>
    public override string GetCleanText()
    {
        Init();
        var cleanText = Text;
        if (string.IsNullOrEmpty(cleanText))
            return cleanText;
        if (CleanDelimiters)
            cleanText = new string(cleanText.Where((c, i) => _maskDict.ContainsKey(Mask[i])).ToArray());
        if (Placeholder != null)
            cleanText = cleanText.Replace(Placeholder.Value.ToString(), "");
        return cleanText;
    }

    protected override void InitInternals()
    {
        base.InitInternals();
        if (Placeholder != null)
            _delimiters.Add(Placeholder.Value);
    }

    protected override void UpdateText(string text)
    {
        // don't show a text consisting only of delimiters and placeholders (no actual input)
        if (text.All(c => _delimiters.Contains(c) || (Placeholder != null && c == Placeholder.Value)))
        {
            Text = "";
            CaretPos = 0;
            return;
        }
        Text = ModifyFinalText(text);
        CaretPos = ConsolidateCaret(Text, CaretPos);
    }

    protected virtual string ModifyFinalText(string text)
    {
        /* this  can be overridden in derived classes to apply any necessary changes to the resulting text */
        return text;
    }

    public override void UpdateFrom(IMask other)
    {
        base.UpdateFrom(other);
        if (other is not PatternMask o)
            return;
        Placeholder = o.Placeholder;
        CleanDelimiters = o.CleanDelimiters;
        Transformation = o.Transformation;
        _initialized = false;
        Refresh();
    }
}
