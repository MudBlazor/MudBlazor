// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor;

/// <summary>
/// A set of contiguous characters used to build a <see cref="BlockMask"/>.
/// </summary>
/// <remarks>
/// Example: a mask character of <c>a</c>, <c>Min</c> of <c>2</c>, and <c>Max</c> of <c>3</c>, would allow <c>ABC</c> as a valid value.<br />
/// Example: a mask character of <c>0</c>, <c>Min</c> of <c>5</c>, and <c>Max</c> of <c>7</c>, would allow <c>09123</c> as a valid value.<br />
/// Example: a mask character of <c>*</c>, <c>Min</c> of <c>1</c>, and <c>Max</c> of <c>4</c>, would allow <c>B2A7</c> as a valid value.<br />
/// </remarks>
/// <param name="MaskChar">The mask character.</param>
/// <param name="Min">The minimum required number of characters.</param>
/// <param name="Max">The maximum allowed number of characters.</param>
public record struct Block(char MaskChar, int Min = 1, int Max = 1);

/// <summary>
/// A mask consisting of contiguous sets of characters.
/// </summary>
/// <remarks>
/// This mask is typically used for text which consists of blocks of letters and numbers, such as a flight number (e.g. <c>LH4234</c>) or product code (e.g. <c>SKU1920</c>).
/// </remarks>
/// <seealso cref="DateMask" />
/// <seealso cref="MultiMask" />
/// <seealso cref="PatternMask" />
/// <seealso cref="RegexMask" />
public class BlockMask : RegexMask
{
    /// <summary>
    /// Creates a new block mask.
    /// </summary>
    /// <param name="blocks">The blocks which define this mask.</param>
    /// <remarks>
    /// This mask is typically used for text which consists of blocks of letters and numbers, such as a flight number (e.g. <c>LH4234</c>) or product code (e.g. <c>SKU1920</c>).
    /// </remarks>
    public BlockMask(params Block[] blocks) : base(null)
    {
        if (blocks.Length == 0)
            throw new ArgumentException("supply at least one block", nameof(blocks));
        Blocks = blocks;
        Delimiters = "";
    }

    /// <summary>
    /// Creates a new block mask.
    /// </summary>
    /// <param name="delimiters">The characters which are skipped over when entering new characters.</param>
    /// <param name="blocks">The blocks which define this mask.</param>
    /// <remarks>
    /// This mask is typically used for text which consists of blocks of letters and numbers, such as a flight number (e.g. <c>LH4234</c>) or product code (e.g. <c>SKU1920</c>).
    /// </remarks>
    public BlockMask(string delimiters, params Block[] blocks) : this(blocks)
    {
        Delimiters = delimiters ?? "";
    }

    /// <summary>
    /// The sets of characters used to build this mask.
    /// </summary>
    public Block[] Blocks { get; protected set; }

    /// <inheritdoc />
    protected override void InitInternals()
    {
        base.InitInternals();
        Blocks ??= Array.Empty<Block>();
        Mask = BuildRegex(Blocks);
        _regex = new Regex(Mask);
    }

    /// <inheritdoc />
    protected override void InitRegex()
    {
        // BlockMask inits regex itself (but after base init), no need to do it twice
        // so don't call base init here
        //base.InitRegex();
    }

    /// <summary>
    /// Creates a regular expression from the specified blocks.
    /// </summary>
    /// <param name="blocks">The list of blocks to combine into an expression.</param>
    /// <returns>A progressive regular expression which represents all of the blocks.</returns>
    protected virtual string BuildRegex(Block[] blocks)
    {
        var regexBuilder = new StringBuilder();
        var openParenthesisCount = 0;
        regexBuilder.Append('^');

        for (var i = 0; i < blocks.Length; i++)
        {
            var block = blocks[i];
            AddRequiredCharacters(regexBuilder, block, ref openParenthesisCount);
            AddOptionalCharacters(regexBuilder, block, ref openParenthesisCount);
            AddDelimiter(regexBuilder, i, blocks, ref openParenthesisCount);
        }

        CloseOpenParentheses(regexBuilder, openParenthesisCount);
        regexBuilder.Append('$');
        return regexBuilder.ToString();
    }

    // Helper method to add required characters for the block
    private void AddRequiredCharacters(StringBuilder regexBuilder, Block block, ref int openParenthesisCount)
    {
        for (var i = 0; i < block.Min; i++)
        {
            regexBuilder.Append('(');
            openParenthesisCount++;

            if (_maskDict.TryGetValue(block.MaskChar, out var maskDef))
                regexBuilder.Append(maskDef.Regex);
            else
                regexBuilder.Append(Regex.Escape(block.MaskChar.ToString()));
        }
    }

    // Helper method to add optional characters for the block
    private void AddOptionalCharacters(StringBuilder regexBuilder, Block block, ref int openParenthesisCount)
    {
        if (block.Max > block.Min)
        {
            for (var i = block.Min; i < block.Max; i++)
            {
                regexBuilder.Append('(');
                openParenthesisCount++;

                if (_maskDict.TryGetValue(block.MaskChar, out var maskDef))
                    regexBuilder.Append(maskDef.Regex);
                else
                    regexBuilder.Append(Regex.Escape(block.MaskChar.ToString()));
            }

            for (var i = block.Min; i < block.Max; i++)
            {
                regexBuilder.Append(")?");
                openParenthesisCount--;
            }
        }
    }

    // Helper method to add delimiter if there are more blocks to process
    private void AddDelimiter(StringBuilder regexBuilder, int index, Block[] blocks, ref int openParenthesisCount)
    {
        if (_delimiters.Count > 0 && index < blocks.Length - 1)
        {
            regexBuilder.Append("([");
            openParenthesisCount++;

            foreach (var delimiter in _delimiters)
                regexBuilder.Append(Regex.Escape(delimiter.ToString()));

            regexBuilder.Append(']');
        }
    }

    // Helper method to close any open parentheses
    private static void CloseOpenParentheses(StringBuilder regexBuilder, int openParenthesisCount)
    {
        for (var i = 0; i < openParenthesisCount; i++)
            regexBuilder.Append(")?");
    }

    /// <inheritdoc />
    public override void UpdateFrom(IMask other)
    {
        base.UpdateFrom(other);
        if (other is BlockMask o)
        {
            Blocks = o.Blocks ?? Array.Empty<Block>();
            Delimiters = o.Delimiters;
            _initialized = false;
            Refresh();
        }
    }
}
