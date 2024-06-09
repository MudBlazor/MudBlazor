// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudBlazor;

public abstract class BaseMask : IMask
{
    protected bool _initialized;
    protected Dictionary<char, MaskChar> _maskDict;

    protected MaskChar[] _maskChars = new MaskChar[]
    {
        MaskChar.Letter('a'), MaskChar.Digit('0'), MaskChar.LetterOrDigit('*'),
    };

    // per definition (unless defined otherwise in subclasses) delimiters are chars
    // in the mask which do not match any MaskChar
    protected HashSet<char> _delimiters;

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

    protected virtual void InitInternals()
    {
        _maskDict = _maskChars.ToDictionary(x => x.Char);
        if (Mask == null)
            _delimiters = new();
        else
            _delimiters =
                new HashSet<char>(Mask.Where(c => _maskChars.All(maskDef => maskDef.Char != c)));
    }

    /// <summary>
    /// The mask defining the structure of the accepted input. 
    /// The mask format depends on the implementation.
    /// </summary>
    public string Mask { get; protected set; }

    /// <summary>
    /// The current text as it is displayed in the component
    /// </summary>
    public string Text { get; protected set; }

    /// <summary>
    /// Get the Text without delimiters or placeholders. Depends on the implementation entirely.
    /// Clean text will usually be used for the Value property of a mask field. 
    /// </summary>
    public virtual string GetCleanText() => Text;

    /// <summary>
    /// The current caret position
    /// </summary>
    public int CaretPos { get; set; }

    /// <summary>
    /// The currently selected sub-section of the Text
    /// </summary>
    public (int, int)? Selection { get; set; }

    /// <summary>
    /// Allow showing a text consisting only of delimiters
    /// </summary>
    public bool AllowOnlyDelimiters { get; set; }

    /// <summary>
    /// The mask chars define the meaning of single mask characters such as 'a', '0'
    /// </summary>
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
    /// Implements user input at the current caret position (single key strokes or pasting longer text)
    /// </summary>
    /// <param name="input"></param>
    public abstract void Insert(string input);

    /// <summary>
    /// Implements the effect of the Del key at the current cursor position
    /// </summary>
    public abstract void Delete();

    /// <summary>
    /// Implements the effect of the Backspace key at the current cursor position
    /// </summary>
    public abstract void Backspace();

    /// <summary>
    /// Reset the mask as if the whole textfield was cleared
    /// </summary>
    public void Clear()
    {
        Init();
        Text = "";
        CaretPos = 0;
        Selection = null;
    }

    /// <summary>
    /// Overwrite the mask text from the outside without losing caret position
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        Clear();
        Insert(text);
    }

    /// <summary>
    /// Update Text from the inside
    /// </summary>
    /// <param name="text"></param>
    protected virtual void UpdateText(string text)
    {
        // don't show a text consisting only of delimiters and placeholders (no actual input)
        if (!AllowOnlyDelimiters && text.All(c => _delimiters.Contains(c)))
        {
            Text = "";
            return;
        }
        Text = text;
        CaretPos = ConsolidateCaret(Text, CaretPos);
    }

    protected abstract void DeleteSelection(bool align);

    protected virtual bool IsDelimiter(char maskChar)
    {
        return _delimiters.Contains(maskChar);
    }

    public virtual void UpdateFrom(IMask o)
    {
        if (o is not BaseMask other)
            return;
        if (other.Mask != Mask)
        {
            Mask = other.Mask;
            _initialized = false;
        }
        if (other.MaskChars != null)
        {
            var maskChars = new HashSet<MaskChar>(_maskChars ?? new MaskChar[0]);
            if (other.MaskChars.Length != MaskChars.Length || other.MaskChars.Any(x => !maskChars.Contains(x)))
            {
                _maskChars = other.MaskChars;
                _initialized = false;
            }
        }
        Refresh();
    }

    /// <summary>
    /// Re-applies parameters (i.e. after they changed) without loosing internal state
    /// such as Text, CaretPos and Selection
    /// </summary>
    protected virtual void Refresh()
    {
        var caret = CaretPos;
        var sel = Selection;
        SetText(Text);
        CaretPos = ConsolidateCaret(Text, caret);
        Selection = sel;
        if (sel != null)
            ConsolidateSelection();
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
    /// Performs simple border checks and corrections to the caret position
    /// </summary>
    protected static int ConsolidateCaret(string text, int caretPos)
    {
        if (string.IsNullOrEmpty(text) || caretPos < 0)
            return 0;
        if (caretPos < text.Length)
            return caretPos;
        return text.Length;
    }

    /// <summary>
    /// Performs simple border checks and corrections to the selection
    /// and removes zero-width selections
    /// </summary>
    protected void ConsolidateSelection()
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

    internal static (string, string, string) SplitSelection(string text, (int, int) selection)
    {
        var start = ConsolidateCaret(text, selection.Item1);
        var end = ConsolidateCaret(text, selection.Item2);
        (var s1, var rest) = SplitAt(text, start);
        (var s2, var s3) = SplitAt(rest, end - start);
        return (s1, s2, s3);
    }

    /// <summary>
    /// Prints a representation of the input including markers for caret and selection
    /// Used heavily by the tests
    /// </summary>
    /// <returns></returns>
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
