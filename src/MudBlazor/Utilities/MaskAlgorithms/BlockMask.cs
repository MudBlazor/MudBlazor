// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
        Blocks ??= new Block[0];
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
        var s = new StringBuilder();
        int i = 0;
        int blockIndex = 0;
        s.Append("^");
        foreach (var b in blocks)
        {
            for (int j = 0; j < b.Min; j++)
            {
                if (i > 0)
                    s.Append("(");
                if (_maskDict.TryGetValue(b.MaskChar, out var maskDef))
                    s.Append(maskDef.Regex);
                else
                    s.Append(Regex.Escape(b.MaskChar.ToString()));
            }

            i += b.Min;
            if (b.Max > b.Min)
            {
                for (int j = b.Min; j < b.Max; j++)
                {
                    s.Append("(");
                    if (_maskDict.TryGetValue(b.MaskChar, out var maskDef))
                        s.Append(maskDef.Regex);
                    else
                        s.Append(Regex.Escape(b.MaskChar.ToString()));
                }

                for (int j = b.Min; j < b.Max; j++)
                    s.Append(")?");
            }

            if (_delimiters.Count > 0 && blockIndex < blocks.Length - 1)
            {
                s.Append("([");
                foreach (var d in _delimiters)
                    s.Append(Regex.Escape(d.ToString()));
                s.Append("]");
                i++;
            }

            blockIndex++;
        }

        for (int j = 0; j < i - 1; j++)
            s.Append(")?");
        s.Append("$");
        return s.ToString();
    }

    public override void UpdateFrom(IMask other)
    {
        base.UpdateFrom(other);
        var o = other as BlockMask;
        if (o == null)
            return;
        Blocks = o.Blocks ?? new Block[0];
        Delimiters = o.Delimiters;
        _initialized = false;
        Refresh();
    }
}
