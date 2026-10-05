// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MudBlazor.Utilities;

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

    protected string _regexPattern = string.Empty;
    protected Regex? _regex;

    /// <summary>
    /// The characters which are jumped over when adding an input character.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  For example: for a delimiter of <c>.</c>, a mask of <c>^[0-9].[0-9].[0-9]$</c>, and characters typed of <c>012</c>, the resulting text would be <c>0.1.2</c>
    /// </remarks>
    public string? DelimiterCharacters { get; protected set; }

    /// <summary>
    /// Creates a mask using a regular expression.
    /// </summary>
    /// <param name="regex">The regular expression used to validate inputs.  Must begin with <c>^</c> and end with <c>$</c>.</param>
    /// <param name="mask">The structure of the accepted input.  When <c>null</c>, the regular expression is used for the mask.</param>
    /// <remarks>
    /// The regular expression must be able to match partial inputs, must begin with <c>^</c>, and must end with <c>$</c> to work properly (e.g. <c>^[0-9]+$</c>). Use open-ended quantifiers like <c>^[0-9]{0,5}$</c>, not exact ones like <c>^[0-9]{5}$</c>, which never match a shorter prefix and block all input.<br />
    /// Consider using <see cref="BlockMask"/> to generate the regular expression automatically.
    /// </remarks>
    public RegexMask(string regex, string? mask = null)
    {
        _regexPattern = regex ?? throw new ArgumentNullException(nameof(regex));
        Mask = mask ?? regex;
    }

    /// <summary>
    /// Protected constructor for derived classes that will set the regex pattern later.
    /// </summary>
    protected RegexMask()
    {
    }

    /// <inheritdoc />
    protected override void InitInternals()
    {
        base.InitInternals();
        DelimiterCharacters ??= string.Empty;
        SetDelimiters(DelimiterCharacters);
        InitRegex();
    }

    /// <summary>
    /// Initializes the regular expression.
    /// </summary>
    protected virtual void InitRegex()
    {
        _regex = new Regex(_regexPattern, RegexOptions.None, RegexDefaults.MatchTimeout);
    }

    /// <inheritdoc />
    public override void Insert(string? input)
    {
        Init();
        DeleteSelection(align: false);
        var text = Text ?? string.Empty;
        var pos = ConsolidateCaret(text, CaretPos);
        var (beforeText, afterText) = SplitAt(text, pos);
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
        var (s1, _, s3) = SplitSelection(Text, sel);
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
        var text = Text ?? string.Empty;
        var pos = ConsolidateCaret(text, CaretPos);
        if (pos >= text.Length)
            return;
        var (beforeText, afterText) = SplitAt(text, pos);
        // delete as many delimiters as there are plus one char
        var restText = new string(afterText.SkipWhile(IsDelimiter).Skip(1).ToArray());
        UpdateText(AlignAgainstMask(beforeText + restText));
        var numDeleted = afterText.Length - restText.Length;
        if (numDeleted > 1)
        {
            // since we just auto-deleted delimiters which were re-created by AlignAgainstMask we can just as well
            // adjust the cursor position to after the delimiters
            CaretPos += numDeleted - 1;
        }
    }

    /// <inheritdoc />
    public override void Backspace()
    {
        Init();
        if (Selection is not null)
        {
            DeleteSelection(align: true);
            return;
        }
        var text = Text ?? string.Empty;
        var pos = ConsolidateCaret(text, CaretPos);
        if (pos == 0)
            return;
        var (beforeText, afterText) = SplitAt(text, pos);
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
        Debug.Assert(_regex is not null);

        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder();

        foreach (var textChar in text)
        {
            // Build current accumulated text once per character to avoid repeated StringBuilder.ToString() calls
            var current = sb.ToString();
            var testWithChar = current + textChar;

            if (_regex.IsMatch(testWithChar))
            {
                sb.Append(textChar);
            }
            // try to skip over a delimiter (input of values only i.e. 31122021 => 31.12.2021)
            else if (!string.IsNullOrEmpty(DelimiterCharacters))
            {
                // Find first delimiter that makes the pattern match
                var matchingDelimiter = DelimiterCharacters.FirstOrDefault(delimiter =>
                    _regex.IsMatch(current + delimiter + textChar));

                if (matchingDelimiter != default(char))
                {
                    sb.Append(matchingDelimiter).Append(textChar);
                }
            }
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public override void UpdateFrom(IMask? other)
    {
        base.UpdateFrom(other);
        if (other is RegexMask regexMask)
        {
            if (DelimiterCharacters != regexMask.DelimiterCharacters)
            {
                DelimiterCharacters = regexMask.DelimiterCharacters;
                ForceReinitialize();
            }

            Refresh();
        }
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
        var regexMask = new RegexMask(regex, mask) { DelimiterCharacters = delimiters };
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
        var regexMask = new RegexMask(regex, mask) { DelimiterCharacters = delimiters, AllowOnlyDelimiters = true };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for email addresses.
    /// </summary>
    /// <param name="mask">Defaults to <c>Ex. user@domain.com</c>.  The mask to display.</param>
    public static RegexMask Email(string mask = "Ex. user@domain.com")
    {
        const string Regex = $"^(?>[\\w\\-\\+]+\\.?)+(?>@?|@)(?<!(\\.@))(?>\\w+[\\.-])*([a-zA-Z0-9]+)?{WhiteSpaceFilter}$";
        const string Delimiters = "@.";
        var regexMask = new RegexMask(Regex, mask) { DelimiterCharacters = Delimiters };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for United States ZIP codes with an optional four-digit extension.
    /// </summary>
    /// <param name="maskChar">Defaults to <c>0</c>.  The mask character for ZIP code digits.</param>
    /// <remarks>
    /// Accepts a five-digit ZIP code such as <c>90210</c> and the ZIP+4 form such as <c>90210-0123</c>.  The dash is inserted automatically as soon as a sixth digit is typed.
    /// </remarks>
    public static RegexMask UsZipCode(char maskChar = '0')
    {
        const string Regex = $"^(?:[0-9]{{0,5}}|[0-9]{{5}}-[0-9]{{0,4}}){WhiteSpaceFilter}$";
        const string Delimiters = "-";
        var mask = $"{new string(maskChar, 5)}-{new string(maskChar, 4)}";
        var regexMask = new RegexMask(Regex, mask) { DelimiterCharacters = Delimiters };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for MAC addresses (EUI-48) such as <c>1A:2B:3C:4D:5E:6F</c>.
    /// </summary>
    /// <param name="separator">Defaults to <c>:</c>.  The character between each pair of hexadecimal digits.</param>
    /// <param name="maskChar">Defaults to <c>X</c>.  The mask character for address digits.</param>
    /// <remarks>
    /// The separator is inserted automatically when hexadecimal digits are typed without it.
    /// </remarks>
    public static RegexMask MacAddress(char separator = ':', char maskChar = 'X')
    {
        const string HexPair = "[0-9A-Fa-f]{0,2}";
        var delimiters = separator.ToString();
        // The separator comes from the caller, so it has to be escaped before it goes into the pattern.
        var escapedSeparator = System.Text.RegularExpressions.Regex.Escape(delimiters);
        var regex = $"^{HexPair}(?:{escapedSeparator}{HexPair}){{0,5}}{WhiteSpaceFilter}$";
        var mask = string.Join(delimiters, Enumerable.Repeat(new string(maskChar, 2), 6));
        var regexMask = new RegexMask(regex, mask) { DelimiterCharacters = delimiters };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for globally unique identifiers such as <c>2f9d8e7a-1b3c-4d5e-6f70-8192a3b4c5d6</c>.
    /// </summary>
    /// <param name="maskChar">Defaults to <c>X</c>.  The mask character for identifier digits.</param>
    /// <remarks>
    /// Accepts the canonical 8-4-4-4-12 hexadecimal form in upper or lower case.  Dashes are inserted automatically when hexadecimal digits are typed without them.  The braced form <c>{...}</c> is not accepted.
    /// </remarks>
    public static RegexMask Guid(char maskChar = 'X')
    {
        const string Hex = "[0-9A-Fa-f]";
        const string Delimiters = "-";
        // The blocks nest from the last one outwards so a dash is only accepted once the block before it is complete.
        var fifthBlock = $"{Hex}{{0,12}}";
        var fourthBlock = $"(?:{Hex}{{0,3}}|{Hex}{{4}}(?:-{fifthBlock})?)";
        var thirdBlock = $"(?:{Hex}{{0,3}}|{Hex}{{4}}(?:-{fourthBlock})?)";
        var secondBlock = $"(?:{Hex}{{0,3}}|{Hex}{{4}}(?:-{thirdBlock})?)";
        var firstBlock = $"(?:{Hex}{{0,7}}|{Hex}{{8}}(?:-{secondBlock})?)";
        var regex = $"^{firstBlock}{WhiteSpaceFilter}$";
        var mask = $"{new string(maskChar, 8)}-{new string(maskChar, 4)}-{new string(maskChar, 4)}-{new string(maskChar, 4)}-{new string(maskChar, 12)}";
        var regexMask = new RegexMask(regex, mask) { DelimiterCharacters = Delimiters };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for hexadecimal colors such as <c>#1A2B3C</c>.
    /// </summary>
    /// <param name="maskChar">Defaults to <c>X</c>.  The mask character for color digits.</param>
    /// <remarks>
    /// The leading <c>#</c> is inserted automatically.  Up to eight hexadecimal digits are accepted, which covers the CSS <c>#RGB</c>, <c>#RGBA</c>, <c>#RRGGBB</c>, and <c>#RRGGBBAA</c> forms.
    /// </remarks>
    public static RegexMask HexColor(char maskChar = 'X')
    {
        const string Regex = $"^#[0-9A-Fa-f]{{0,8}}{WhiteSpaceFilter}$";
        const string Delimiters = "#";
        var mask = $"#{new string(maskChar, 8)}";
        var regexMask = new RegexMask(Regex, mask) { DelimiterCharacters = Delimiters };
        return regexMask;
    }

    /// <summary>
    /// Gets a mask for times on a 24-hour clock such as <c>07:30</c>.
    /// </summary>
    /// <param name="includeSeconds">Defaults to <c>false</c>.  When <c>true</c>, seconds are allowed.</param>
    /// <param name="maskChar">Defaults to <c>0</c>.  The mask character for time digits.</param>
    /// <remarks>
    /// Hours are limited to <c>0</c> through <c>23</c>, and minutes and seconds to <c>0</c> through <c>59</c>.  Colons are inserted automatically when digits are typed without them, so typing <c>2400</c> yields <c>2:40</c>.
    /// </remarks>
    public static RegexMask Time24(bool includeSeconds = false, char maskChar = '0')
    {
        const string Hour = "(?:[01][0-9]?|2[0-3]?|[3-9])";
        const string Delimiters = ":";

        // A lone tens digit is a valid prefix, and the following field is nested so it can only begin once this one holds a complete pair.
        static string Field(string next) => $"(?::(?:[0-5]|[0-5][0-9]{next})?)?";

        var seconds = includeSeconds ? Field(string.Empty) : string.Empty;
        var regex = $"^{Hour}{Field(seconds)}{WhiteSpaceFilter}$";
        var digitPair = new string(maskChar, 2);
        var mask = includeSeconds
            ? $"{digitPair}:{digitPair}:{digitPair}"
            : $"{digitPair}:{digitPair}";
        var regexMask = new RegexMask(regex, mask) { DelimiterCharacters = Delimiters };
        return regexMask;
    }
}
