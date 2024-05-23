// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor;

public record struct Block(char MaskChar, int Min = 1, int Max = 1);

public class BlockMask : RegexMask
{
    public BlockMask(params Block[] blocks) : base(null)
    {
        if (blocks.Length == 0)
            throw new ArgumentException("supply at least one block", nameof(blocks));
        Blocks = blocks;
        Delimiters = "";
    }

    public BlockMask(string delimiters, params Block[] blocks) : this(blocks)
    {
        Delimiters = delimiters ?? "";
    }

    public Block[] Blocks { get; protected set; }

    protected override void InitInternals()
    {
        base.InitInternals();
        Blocks ??= Array.Empty<Block>();
        Mask = BuildRegex(Blocks);
        _regex = new Regex(Mask);
    }

    protected override void InitRegex()
    {
        // BlockMask inits regex itself (but after base init), no need to do it twice
        // so don't call base init here
        //base.InitRegex();
    }

    /// <summary>
    /// Build the progressive working regex from the block and delimiter definitions
    /// Note: a progressive regex must match partial input!!!!
    /// </summary>
    /// <param name="blocks"></param>
    /// <returns></returns>
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
