// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.Highlighter; // Added for FragmentInfo
// Removed: using static MudBlazor.Components.Highlighter.Splitter; 
// We will call Splitter methods statically: Splitter.GetFragments, Splitter.GetHtmlAwareFragments

namespace MudBlazor;

#nullable enable

/// <summary>
/// A component which highlights words or phrases within text.
/// </summary>
public partial class MudHighlighter : MudComponentBase
{
    private Memory<string> _fragments;
    private string? _regex;
    private List<FragmentInfo> _htmlAwareFragments = [];

    /// <summary>
    /// The text to consider for highlighting.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public string? Text { get; set; }

    /// <summary>
    /// The text to highlight within <see cref="Text" />.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public string? HighlightedText { get; set; }

    /// <summary>
    /// The multiple text fragments to highlight within <see cref="Text" />.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public IEnumerable<string> HighlightedTexts { get; set; } = [];

    /// <summary>
    /// Whether highlighted text is case sensitive.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public bool CaseSensitive { get; set; }

    /// <summary>
    /// Highlights text until the next RegEx boundary.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Behavior)]
    public bool UntilNextBoundary { get; set; }

    /// <summary>
    /// Renders text as a <see cref="RenderFragment"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Highlighter.Appearance)]
    public bool Markup { get; set; }

    //TODO
    //Accept regex highlightings
    // [Parameter] public bool IsRegex { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Markup)
        {
            _htmlAwareFragments = Splitter.GetHtmlAwareFragments(Text, HighlightedText, HighlightedTexts, out _regex, CaseSensitive, UntilNextBoundary);
            _fragments = Memory<string>.Empty;
        }
        else
        {
            _fragments = Splitter.GetFragments(Text, HighlightedText, HighlightedTexts, out _regex, CaseSensitive, UntilNextBoundary);

            if (_htmlAwareFragments == null)
                _htmlAwareFragments = [];
            else
                _htmlAwareFragments.Clear();
        }
    }

    bool IsMatch(string fragment) => !string.IsNullOrWhiteSpace(fragment) &&
                                     !string.IsNullOrWhiteSpace(_regex) &&
                                     Regex.IsMatch(fragment, _regex, CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
}
