// Copyright (c) MudBlazor 2021
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
    ///     Add this filter to the end of a mask to block any space, tab or newline character.
    /// </summary>
    private const string WhiteSpaceFilter = "(?!\\s)";

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
    /// <param name="mask">
    /// The mask defining the structure of the accepted input.
    /// 
    /// Note: if not included the regex will be the mask.   
    /// </param>
    public RegexMask(string regex, string mask = null)
    {
        _regexPattern = regex;
        Mask = mask ?? regex;
    }

    protected string _regexPattern;
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
        _regex = new Regex(_regexPattern);
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
        UpdateText(AlignAgainstMask(beforeText + restText));
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
        if (other is not RegexMask o)
            return;
        if (Delimiters != o.Delimiters)
        {
            Delimiters = o.Delimiters;
            _initialized = false;
        }
        Refresh();
    }

    /// <summary>
    /// Creates a predefined RegexMask for an IPv4 Address with or without port masking.
    /// </summary>
    /// <param name="includePort">
    /// Set to true to include port to the mask.
    /// </param>
    /// <param name="maskChar">
    /// Set the IPv4 maskChar. Default is '0'
    /// </param>
    public static RegexMask IPv4(bool includePort = false, char maskChar = '0')
    {
        const string Octet = "25[0-5]|2[0-4][0-9]|[0-1]?[0-9]{0,2}";

        var ipv4 = $"(?:{Octet})(?:\\.(?:{Octet})){{0,3}}";
        var delimiters = ".";
        var octetMask = new string(maskChar, 3);
        var mask = string.Join(delimiters, Enumerable.Repeat(octetMask, 4));
        if (includePort)
        {
            const string IpPort =
                "(:|:(6553[0-5]|655[0-2][0-9]|65[0-4][0-9]{2}|6[0-4][0-9]{3}|[1-5][0-9]{4}|[1-9][0-9]{0,3}))?";
            ipv4 = $"{ipv4}{IpPort}";
            mask = $"{mask}:{new string(maskChar, 5)}";
            delimiters += ":";
        }

        var regex = $"^{ipv4}{WhiteSpaceFilter}$";
        var regexMask = new RegexMask(regex, mask) { Delimiters = delimiters };
        return regexMask;
    }

    /// <summary>
    /// Creates a predefined RegexMask for an IPv6 Address with or without port masking.
    /// </summary>
    /// <param name="includePort">
    /// Set to true to include port to the mask.
    /// </param>
    /// <param name="maskChar">
    /// Set the IPv6 maskChar. Default is 'X'
    /// </param>
    /// <param name="portMaskChar">
    /// Set the IPv6 portMask. Default is '0'
    /// </param>
    public static RegexMask IPv6(bool includePort = false, char maskChar = 'X', char portMaskChar = '0')
    {
        const string Hex = "[0-9A-Fa-f]{0,4}";
        const string IPv6Filter = "(?!.*?[:]{2}?:)";
        var ipv6 = $"{Hex}(:{Hex}){{0,7}}";
        var delimiters = ":";
        var hexMask = new string(maskChar, 4);
        var mask = string.Join(delimiters, Enumerable.Repeat(hexMask, 8));
        if (includePort)
        {
            const string IpPort =
                "(\\]|\\]:|\\]:(6553[0-5]|655[0-2][0-9]|65[0-4][0-9]{2}|6[0-4][0-9]{3}|[1-5][0-9]{4}|[1-9][0-9]{0,3}))?";
            ipv6 = $"((\\[{ipv6}){IpPort})";
            mask = $"[{mask}]:{new(portMaskChar, 5)}";
            delimiters += "[]";
        }

        var regex = $"^{IPv6Filter}{ipv6}{WhiteSpaceFilter}$";
        var regexMask = new RegexMask(regex, mask) { Delimiters = delimiters, AllowOnlyDelimiters = true };
        return regexMask;
    }

    /// <summary>
    /// Creates a predefined RegexMask for Email Address.
    /// </summary>
    /// <param name="mask">
    /// Set the email mask. Default is "Ex. user@domain.com"
    /// </param>
    public static RegexMask Email(string mask = "Ex. user@domain.com")
    {
        const string Regex = $"^(?>[\\w\\-\\+]+\\.?)+(?>@?|@)(?<!(\\.@))(?>\\w+\\.)*(\\w+)?{WhiteSpaceFilter}$";
        const string Delimiters = "@.";
        var regexMask = new RegexMask(Regex, mask) { Delimiters = Delimiters };
        return regexMask;
    }
}
