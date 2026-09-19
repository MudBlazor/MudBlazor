// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Services;
using MudBlazor.Utilities;

namespace MudBlazor.Docs.Components;

public partial class SectionContent
{
    [Inject] protected IJsApiService JsApiService { get; set; }
    [Inject] protected IDocsJsApiService DocsJsApiService { get; set; }
    [Inject] protected ISnackbar SnackbarService { get; set; }
    [Inject] protected ILogger<SectionContent> Logger { get; set; }

    protected string Classname =>
        new CssBuilder("docs-section-content")
            .AddClass($"outlined", Outlined && ChildContent != null)
            .AddClass($"darken", DarkenBackground)
            .AddClass(Class)
            .Build();

    protected string ToolbarClassname =>
        new CssBuilder("docs-section-content-toolbar")
            // The bar sits between the preview and its source, so it draws the
            // divider that the two cancelled border radii used to fake.
            .AddClass("seam", ChildContent != null)
            .AddClass("darken", ChildContent == null && Codes != null)
            .Build();

    protected string ToggleClassname =>
        new CssBuilder("docs-section-code-toggle")
            .AddClass("expanded", _showCode)
            .Build();

    protected string InnerClassname =>
        new CssBuilder("docs-section-content-inner")
            .AddClass($"relative d-flex flex-grow-1 flex-wrap justify-center align-center", !Block)
            .AddClass($"d-block mx-auto", Block)
            .AddClass($"mud-width-full", Block && FullWidth)
            // The pane is evenly padded in both states now. The old "px-8 pb-8 pt-2"
            // existed only to absorb the toolbar that used to sit on top of it.
            .AddClass("pa-8", !IsApiSection)
            .AddClass("pa-2", IsApiSection)
            .Build();

    protected string SourceClassname =>
        new CssBuilder("docs-section-source")
            // Nested in the shell the source inherits its border; standalone it
            // still needs its own.
            .AddClass($"outlined", Outlined && ChildContent == null)
            .Build();

    // _snippetId marks the element the clipboard falls back to reading;
    // _sourceId marks the collapsible region the toggle owns.
    private readonly string _snippetId = Identifier.Create();
    private readonly string _sourceId = Identifier.Create();

    [Parameter] public string Class { get; set; }
    [Parameter] public bool DarkenBackground { get; set; }
    [Parameter] public bool Outlined { get; set; } = true;
    [Parameter] public bool ShowCode { get; set; } = true;
    [Parameter] public bool Block { get; set; }
    [Parameter] public bool FullWidth { get; set; }
    [Parameter] public string Code { get; set; }
    [Parameter] public string HighLight { get; set; }
    [Parameter] public IReadOnlyList<CodeFile> Codes { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public bool IsApiSection { get; set; }

    private bool _hasCode;
    private string _activeCode;

    // ShowCode and the selected file are reader state, so the component owns
    // them. Mutating the [Parameter] directly meant Blazor overwrote the
    // reader's choice on the next parent render - and QueuedContent renders
    // sections progressively, so parents keep re-rendering after page load.
    private bool _showCode;
    private bool _showCodeInitialized;

    protected override void OnParametersSet()
    {
        // Was only ever set to true, never back to false when the parameters
        // changed away from having code.
        _hasCode = Codes != null || !string.IsNullOrWhiteSpace(Code);

        // Keep the reader's tab selection unless it is no longer on offer.
        var activeIsStillValid = _activeCode != null &&
                                 (Codes != null
                                     ? Codes.Any(x => x.Code == _activeCode)
                                     : _activeCode == Code);

        if (!activeIsStillValid)
        {
            _activeCode = Codes?.FirstOrDefault()?.Code ?? Code;
        }

        if (!_showCodeInitialized)
        {
            _showCode = ShowCode;
            _showCodeInitialized = true;
        }
    }

    public void OnShowCode()
    {
        _showCode = !_showCode;
    }

    public void SetActiveCode(string value)
    {
        _activeCode = value;
    }

    private string GetActiveCode(string value)
    {
        return value == _activeCode
            ? "file-button active"
            : "file-button";
    }

    private async Task CopyTextToClipboard()
    {
        // _activeCode, not Code: in a multi-file section Code is null and this
        // used to fall through to reading the rendered DOM.
        var code = Snippets.GetCode(_activeCode ?? Code);
        code ??= await DocsJsApiService.GetInnerTextByIdAsync(_snippetId);

        if (string.IsNullOrWhiteSpace(code))
        {
            // Never put the failure on the reader's clipboard and then tell them
            // it was copied.
            Logger.LogWarning("No source available to copy for snippet '{Snippet}'.", _activeCode ?? Code);
            SnackbarService.Add($"This example's source is missing from the build.", Severity.Error);

            return;
        }

        await JsApiService.CopyToClipboardAsync(code);
        SnackbarService.Add("Copied to clipboard");
    }

    private RenderFragment CodeComponent(string code) => builder =>
    {
        try
        {
            var key = typeof(SectionContent).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains($".{code}Code.html"));
            if (key == null)
            {
                // Used to render an empty pane and tell nobody.
                Logger.LogError("No embedded source found for snippet '{Snippet}'.", code);

                return;
            }

            using (var stream = typeof(SectionContent).Assembly.GetManifestResourceStream(key))
            using (var reader = new StreamReader(stream!))
            {
                var read = reader.ReadToEnd();

                // Ensure the code uses spaces for indentation regardless of the formatting within the source code.
                read = read.Replace("\t", "    ");

                if (!string.IsNullOrEmpty(HighLight))
                {
                    var highlights = HighLight.Contains(',')
                        ? HighLight.Split(",")
                        : [HighLight];

                    foreach (var value in highlights)
                    {
                        // Regex.Escape: an unescaped term containing regex
                        // metacharacters threw straight into the catch below,
                        // which blanked the whole pane.
                        read = Regex.Replace(read, $"{Regex.Escape(value)}(?=\\s|\")", "<mark>$&</mark>",
                            RegexOptions.None, TimeSpan.FromMilliseconds(100));
                    }
                }

                builder.AddMarkupContent(0, read);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not render the source for snippet '{Snippet}'.", code);
        }
    };

    protected virtual async Task RunOnTryMudBlazorAsync()
    {
        string firstFile;
        if (Codes == null)
        {
            firstFile = Code;
        }
        else
        {
            firstFile = Codes.FirstOrDefault()?.Code ?? Code;
        }

        if (string.IsNullOrWhiteSpace(firstFile))
        {
            return;
        }

        // We use a separator that won't be in code so we can send 2 files later
        var codeFiles = "__Main.razor" + (char)31 + Snippets.GetCode(firstFile);

        // Add dialogs for dialog examples
        if (firstFile.StartsWith("Dialog"))
        {
            var regex = ShowDialogRegularExpression();
            var dialogCodeName = regex.Match(codeFiles).Groups["dialogname"].Value;
            if (dialogCodeName != string.Empty)
            {
                var dialogCodeFile = dialogCodeName + ".razor" + (char)31 + Snippets.GetCode(dialogCodeName);
                codeFiles = codeFiles + (char)31 + dialogCodeFile;
            }
        }

        // Data models
        if (codeFiles.Contains("MudBlazor.Examples.Data.Models"))
        {
            if (ElementRegularExpression().IsMatch(codeFiles))
            {
                var elementCodeFile = "Element.cs" + (char)31 + Snippets.GetCode("Element");
                codeFiles = codeFiles + (char)31 + elementCodeFile;
            }

            if (ServerRegularExpression().IsMatch(codeFiles))
            {
                var serverCodeFile = "Server.cs" + (char)31 + Snippets.GetCode("Server");
                codeFiles = codeFiles + (char)31 + serverCodeFile;
            }
        }

        var codeFileEncoded = codeFiles.ToCompressedEncodedUrl();
        // var tryMudBlazorLocation = "https://localhost:5001/";
        const string TryMudBlazorLocation = "https://try.mudblazor.com/";
        var url = $"{TryMudBlazorLocation}snippet/{codeFileEncoded}";
        await JsApiService.OpenInNewTabAsync(url);
    }

    [GeneratedRegex(@"Show(?:Async)?<(?<dialogname>Dialog.*?_Dialog)>")]
    private static partial Regex ShowDialogRegularExpression();

    [GeneratedRegex(@"\bElement\b")]
    private static partial Regex ElementRegularExpression();

    [GeneratedRegex(@"\bServer\b")]
    private static partial Regex ServerRegularExpression();
}
