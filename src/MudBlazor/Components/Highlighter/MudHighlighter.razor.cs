// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using static MudBlazor.Components.Highlighter.Splitter;

namespace MudBlazor;

#nullable enable
public partial class MudHighlighter : MudComponentBase
{
    private Memory<string> _fragments;
    private string? _regex;

    /// <summary>
    /// The whole text in which a fragment will be highlighted
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public string? Text { get; set; }

    /// <summary>
    /// The fragment of text to be highlighted
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public string? HighlightedText { get; set; }

    /// <summary>
    /// The fragments of text to be highlighted
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public IEnumerable<string> HighlightedTexts { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Whether or not the highlighted text is case sensitive
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public bool CaseSensitive { get; set; }

    /// <summary>
    /// If true, highlights the text until the next regex boundary
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public bool UntilNextBoundary { get; set; }

    //TODO
    //Accept regex highlightings
    // [Parameter] public bool IsRegex { get; set; }

    protected override void OnParametersSet()
    {
        _fragments = GetFragments(Text, HighlightedText, HighlightedTexts, out _regex, CaseSensitive, UntilNextBoundary);
    }
}
