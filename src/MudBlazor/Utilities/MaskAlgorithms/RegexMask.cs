// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;

namespace MudBlazor;

/// <summary>
/// An input mask consisting of a regular expression.
/// </summary>
/// <seealso cref="BlockMask" />
/// <seealso cref="DateMask" />
/// <seealso cref="MultiMask" />
/// <seealso cref="PatternMask" />
public class RegexMask : BaseMask
{
    /// <summary>
    /// Add this filter to the end of a mask to block any space, tab or newline character.
    /// </summary>
    private const string WhiteSpaceFilter = "(?!\\s)";

    /// <summary>
    /// Creates a mask using a regular expression.
    /// </summary>
    /// <param name="regex">The regular expression used to validate inputs.  Must begin with <c>^</c> and end with <c>$</c>.</param>
    /// <param name="mask">The structure of the accepted input.  When <c>null</c>, the regular expression is used for the mask.</param>
    /// <remarks>
    /// The regular expression must be able to match partial inputs, must begin with <c>^</c>, and must end with <c>$</c> to work properly (e.g. <c>^[0-9]+$</c>).<br />
    /// Consider using <see cref="BlockMask"/> to generate the regular expression automatically.
    /// </remarks>
    public RegexMask(string regex, string mask = null)
    {
        _regexPattern = regex;
        Mask = mask ?? regex;
    }

    protected string _regexPattern;
    protected Regex _regex;

    /// <summary>
    /// The characters which are jumped over when adding an input character.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  For example: for a delimiter of <c>.</c>, a mask of <c>^[0-9].[0-9].[0-9]$</c>, and characters typed of <c>012</c>, the resulting text would be <c>0.1.2</c>
    /// </remarks>
    public string Delimiters { get; protected set; }

    /// <inheritdoc />
    protected override void InitInternals()
    {
        base.InitInternals();
        Delimiters ??= "";
        _delimiters = new HashSet<char>(Delimiters);
        InitRegex();
    }

    /// <summary>
    /// Initializes the regular expression.
    /// </summary>
    protected virtual void InitRegex()
    {
        _regex = new Regex(_regexPattern);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
    /// Applies an input to the mask.
    /// </summary>
    /// <param name="text">The text to apply to the mask.</param>
    /// <returns>The text input with any delimiters and placeholders applied.</returns>
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

    /// <inheritdoc />
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
    /// Gets a mask for IPv4 addresses with optional port masking.
    /// </summary>
    /// <param name="includePort">Defaults to <c>false</c>.  When <c>true</c>, a port number (from <c>0</c> to <c>65535</c>) is allowed.</param>
    /// <param name="maskChar">Defaults to <c>0</c>.  The mask character for address digits.</param>
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
    /// Gets a mask for IPv6 addresses with optional port masking.
    /// </summary>
    /// <param name="includePort">Defaults to <c>false</c>.  When <c>true</c>, a port number (from <c>0</c> to <c>65535</c>) is allowed.</param>
    /// <param name="maskChar">Defaults to <c>X</c>.  The mask character for address digits.</param>
    /// <param name="portMaskChar">Defaults to <c>0</c>.  The mask character for port digits.</param>
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
    /// Gets a mask for email addresses.
    /// </summary>
    /// <param name="mask">Defaults to <c>Ex. user@domain.com</c>.  The mask to display.</param>
    public static RegexMask Email(string mask = "Ex. user@domain.com")
    {
        const string Regex = $"^(?>[\\w\\-\\+]+\\.?)+(?>@?|@)(?<!(\\.@))(?>\\w+\\.)*(\\w+)?{WhiteSpaceFilter}$";
        const string Delimiters = "@.";
        var regexMask = new RegexMask(Regex, mask) { Delimiters = Delimiters };
        return regexMask;
    }
}
