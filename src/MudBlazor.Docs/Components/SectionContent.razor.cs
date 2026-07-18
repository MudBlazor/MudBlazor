// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
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
    [Inject] protected ISnippetsService SnippetsService { get; set; }
    [Inject] protected ICodeHtmlService CodeHtmlService { get; set; }

    protected string Classname =>
        new CssBuilder("docs-section-content")
            .AddClass($"outlined", Outlined && ChildContent != null)
            .AddClass($"darken", DarkenBackground)
            .AddClass("show-code", _hasCode && ShowCode)
            .AddClass(Class)
            .Build();
    protected string ToolbarClassname =>
        new CssBuilder("docs-section-content-toolbar")
            .AddClass($"outlined", Outlined && ChildContent != null)
            .AddClass("darken", ChildContent == null && Codes != null)
            .Build();

    protected string InnerClassname =>
        new CssBuilder("docs-section-content-inner")
            .AddClass($"relative d-flex flex-grow-1 flex-wrap justify-center align-center", !Block)
            .AddClass($"d-block mx-auto", Block)
            .AddClass($"mud-width-full", Block && FullWidth)
            .AddClass("pa-8", !_hasCode && !IsApiSection)
            .AddClass("px-8 pb-8 pt-2", _hasCode && !IsApiSection)
            .AddClass("pa-2", IsApiSection)
            .Build();

    protected string SourceClassname =>
        new CssBuilder("docs-section-source")
            .AddClass($"outlined", Outlined && ChildContent != null)
            .AddClass("show-code", _hasCode && ShowCode)
            .Build();

    private readonly string _snippetId = Identifier.Create();

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
    // The highlighted markup is fetched on demand (see CodeHtmlService) instead of being embedded in the assembly.
    private MarkupString _activeCodeHtml;

    protected override async Task OnParametersSetAsync()
    {
        if (Codes != null)
        {
            _hasCode = true;
            _activeCode = Codes.FirstOrDefault()?.Code;
        }
        else if (!string.IsNullOrWhiteSpace(Code))
        {
            _hasCode = true;
            _activeCode = Code;
        }

        await LoadActiveCodeHtmlAsync();
    }

    public void OnShowCode()
    {
        ShowCode = !ShowCode;
    }

    public async Task SetActiveCode(string value)
    {
        _activeCode = value;
        await LoadActiveCodeHtmlAsync();
    }

    private async Task LoadActiveCodeHtmlAsync()
    {
        if (!_hasCode || string.IsNullOrEmpty(_activeCode))
        {
            _activeCodeHtml = default;
            return;
        }

        var html = await CodeHtmlService.GetHtmlAsync(_activeCode);
        if (string.IsNullOrEmpty(html))
        {
            _activeCodeHtml = default;
            return;
        }

        // Ensure the code uses spaces for indentation regardless of the formatting within the source code.
        html = html.Replace("\t", "    ");

        if (!string.IsNullOrEmpty(HighLight))
        {
            if (HighLight.Contains(','))
            {
                foreach (var value in HighLight.Split(","))
                {
                    html = Regex.Replace(html, $"{value}(?=\\s|\")", "<mark>$&</mark>");
                }
            }
            else
            {
                html = Regex.Replace(html, $"{HighLight}(?=\\s|\")", "<mark>$&</mark>");
            }
        }

        _activeCodeHtml = new MarkupString(html);
    }

    private string GetActiveCode(string value)
    {
        return value == _activeCode
            ? "file-button active"
            : "file-button";
    }

    private async Task CopyTextToClipboard()
    {
        var code = await SnippetsService.GetSourceAsync(Code);
        code ??= await DocsJsApiService.GetInnerTextByIdAsync(_snippetId);
        await JsApiService.CopyToClipboardAsync(code ?? $"Snippet '{Code}' not found!");
        SnackbarService.Add("Copied to clipboard");
    }

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
        var codeFiles = "__Main.razor" + (char)31 + await SnippetsService.GetSourceAsync(firstFile);

        // Add dialogs for dialog examples
        if (firstFile.StartsWith("Dialog"))
        {
            var regex = ShowDialogRegularExpression();
            var dialogCodeName = regex.Match(codeFiles).Groups["dialogname"].Value;
            if (dialogCodeName != string.Empty)
            {
                var dialogCodeFile = dialogCodeName + ".razor" + (char)31 + await SnippetsService.GetSourceAsync(dialogCodeName);
                codeFiles = codeFiles + (char)31 + dialogCodeFile;
            }
        }

        // Data models
        if (codeFiles.Contains("MudBlazor.Examples.Data.Models"))
        {
            if (ElementRegularExpression().IsMatch(codeFiles))
            {
                var elementCodeFile = "Element.cs" + (char)31 + await SnippetsService.GetSourceAsync("Element");
                codeFiles = codeFiles + (char)31 + elementCodeFile;
            }

            if (ServerRegularExpression().IsMatch(codeFiles))
            {
                var serverCodeFile = "Server.cs" + (char)31 + await SnippetsService.GetSourceAsync("Server");
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
