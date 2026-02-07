// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// A base class for designing input masks for the <see cref="MudMask"/>, <see cref="MudTextField{T}"/>, and <see cref="MudPicker{T}"/> components.
/// </summary>
public abstract class BaseMask : IMask
{
    private bool _initialized;
    private Dictionary<char, MaskChar> _maskDict = [];
    private MaskChar[] _maskChars = [MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*')];
    private HashSet<char> _delimiters = [];

    /// <summary>
    /// Initialize all internal data structures. Can be called multiple times,
    /// will initialize only once. To re-initialize set _initialized to false.
    /// </summary>
    protected void Init()
    {
        if (_initialized)
            return;
        _initialized = true;
        InitInternals();
    }

    /// <summary>
    /// Initializes this mask's characters and delimiters.
    /// </summary>
    protected virtual void InitInternals()
    {
        _maskDict = _maskChars.ToDictionary(x => x.Char);
        _delimiters = string.IsNullOrEmpty(Mask)
            ? []
            : new HashSet<char>(Mask.Where(c => _maskChars.All(maskDef => maskDef.Char != c)));
    }

    /// <inheritdoc />
    public string? Mask { get; protected set; }

    /// <inheritdoc />
    public string? Text { get; protected set; }

    /// <inheritdoc />
    public virtual string? GetCleanText() => Text;

    /// <inheritdoc />
    public int CaretPos { get; set; }

    /// <inheritdoc />
    public (int, int)? Selection { get; set; }

    /// <summary>
    /// Allows showing text which consists only of delimiter characters.
    /// </summary>
    public bool AllowOnlyDelimiters { get; set; }

    /// <summary>
    /// The list of mask characters and their meanings.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>a</c> for letters, <c>0</c> for digits, and <c>*</c> for letters or digits.
    /// </remarks>
    public MaskChar[] MaskChars
    {
        get => _maskChars;
        set
        {
            _maskChars = value;
            // force re-initialization
            _initialized = false;
        }
    }

    /// <summary>
    /// Gets the mask dictionary for internal use.
    /// </summary>
    protected IReadOnlyDictionary<char, MaskChar> MaskDictionary => _maskDict;

    /// <summary>
    /// Gets the set of delimiter characters.
    /// </summary>
    protected IReadOnlySet<char> Delimiters => _delimiters;

    /// <summary>
    /// Adds a delimiter character to the set of delimiters.
    /// </summary>
    /// <param name="delimiter">The delimiter character to add.</param>
    protected void AddDelimiter(char delimiter)
    {
        _delimiters.Add(delimiter);
    }

    /// <summary>
    /// Clears all delimiters and sets new ones.
    /// </summary>
    /// <param name="delimiters">The string of delimiter characters.</param>
    protected void SetDelimiters(string delimiters)
    {
        _delimiters.Clear();
        foreach (var d in delimiters)
        {
            _delimiters.Add(d);
        }
    }

    /// <summary>
    /// Forces reinitialization on next access.
    /// </summary>
    protected void ForceReinitialize()
    {
        _initialized = false;
    }

    /// <inheritdoc />
    public abstract void Insert(string? input);

    /// <inheritdoc />
    public abstract void Delete();

    /// <inheritdoc />
    public abstract void Backspace();

    /// <inheritdoc />
    public void Clear()
    {
        Init();
        Text = string.Empty;
        CaretPos = 0;
        Selection = null;
    }

    /// <inheritdoc />
    public void SetText(string? text)
    {
        Clear();
        Insert(text);
    }

    /// <summary>
    /// Overwrites the text and updates the cursor position.
    /// </summary>
    /// <param name="text">The text to set.</param>
    protected virtual void UpdateText(string text)
    {
        // don't show a text consisting only of delimiters and placeholders (no actual input)
        if (!AllowOnlyDelimiters && text.All(c => Delimiters.Contains(c)))
        {
            Text = string.Empty;
            return;
        }

        Text = text;
        CaretPos = ConsolidateCaret(Text, CaretPos);
    }

    /// <summary>
    /// Deletes the selected characters.
    /// </summary>
    /// <param name="align">When <c>true</c>, the text to the right of the selection will be shifted to the left.</param>
    protected abstract void DeleteSelection(bool align);

    /// <summary>
    /// Gets whether the specified character is a mask character.
    /// </summary>
    /// <param name="maskChar">The character to examine.</param>
    /// <returns>When <c>true</c>, the character is a delimiter.</returns>
    protected virtual bool IsDelimiter(char maskChar)
    {
        return Delimiters.Contains(maskChar);
    }

    /// <inheritdoc />
    public virtual void UpdateFrom(IMask? mask)
    {
        if (mask is BaseMask baseMask)
        {
            if (baseMask.Mask != Mask)
            {
                Mask = baseMask.Mask;
                _initialized = false;
            }

            var maskChars = new HashSet<MaskChar>(_maskChars);
            if (baseMask.MaskChars.Length != MaskChars.Length || baseMask.MaskChars.Any(maskChar => !maskChars.Contains(maskChar)))
            {
                _maskChars = baseMask.MaskChars;
                _initialized = false;
            }

            Refresh();
        }
    }

    /// <summary>
    /// Reapplies parameters after they change, while preserving <see cref="Text"/>, <see cref="CaretPos"/>, and <see cref="Selection"/>.
    /// </summary>
    protected virtual void Refresh()
    {
        var caret = CaretPos;
        var sel = Selection;
        SetText(Text);
        CaretPos = ConsolidateCaret(Text, caret);
        Selection = sel;
        if (sel is not null)
        {
            ConsolidateSelection();
        }
    }

    /// <summary>
    /// Divides a string into two strings at the specified index.
    /// </summary>
    /// <param name="text">The text to split.</param>
    /// <param name="pos">The index of the string to split.</param>
    /// <returns>Two strings split at the specified position.</returns>
    internal static (string, string) SplitAt(string? text, int pos)
    {
        text ??= string.Empty;
        if (pos <= 0)
            return (string.Empty, text);
        if (pos >= text.Length)
            return (text, string.Empty);
        return (text[..pos], text[pos..]);
    }

    /// <summary>
    /// Adjusts the cursor position to be within the text.
    /// </summary>
    /// <remarks>
    /// If <see cref="Text"/> is empty, the position becomes <c>0</c>.  If the position was beyond the length of <see cref="Text"/>, the position is adjusted to the end.  Otherwise, no change is made.
    /// </remarks>
    protected static int ConsolidateCaret(string? text, int caretPos)
    {
        if (string.IsNullOrEmpty(text) || caretPos < 0)
            return 0;
        if (caretPos < text.Length)
            return caretPos;
        return text.Length;
    }

    /// <summary>
    /// Adjusts the selection to be within the text.
    /// </summary>
    /// <remarks>
    /// If the selection has zero length, the selection is deleted.  If the selection start is less than <c>0</c>, the start becomes <c>0</c>.  If the end is longer than the length of <see cref="Text"/>, the end becomes the text length.
    /// </remarks>
    protected void ConsolidateSelection()
    {
        if (Selection is null)
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
        if (Text is not null && sel.Item2 >= Text.Length)
            sel.Item2 = Text.Length;
    }

    internal static (string, string, string) SplitSelection(string? text, (int, int) selection)
    {
        var start = ConsolidateCaret(text, selection.Item1);
        var end = ConsolidateCaret(text, selection.Item2);
        var (s1, rest) = SplitAt(text, start);
        var (s2, s3) = SplitAt(rest, end - start);
        return (s1, s2, s3);
    }

    /// <summary>
    /// Gets the current input including markers for caret and selection.
    /// </summary>
    /// <remarks>
    /// This method is used frequently by unit tests.
    /// </remarks>
    public override string ToString()
    {
        var text = Text ?? string.Empty;
        ConsolidateSelection();
        if (Selection is null)
        {
            var pos = ConsolidateCaret(text, CaretPos);
            if (pos < text.Length)
                return text.Insert(pos, "|");
            return text + "|";
        }

        var sel = Selection.Value;
        var start = ConsolidateCaret(text, sel.Item1);
        var end = ConsolidateCaret(text, sel.Item2);
        var (s1, rest) = SplitAt(text, start);
        var (s2, s3) = SplitAt(rest, end - start);
        return s1 + "[" + s2 + "]" + s3;
    }
}
