// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

public partial class SectionContent
{
    [Inject] protected IJsApiService JsApiService { get; set; }

    protected string Classname =>
        new CssBuilder("docs-section-content")
            .AddClass($"outlined", Outlined)
            .AddClass($"darken {_luckyColor}", DarkenBackground)
            .AddClass("show-code", _hasCode && ShowCode)
            .AddClass(Class)
            .Build();
    protected string ToolbarClassname =>
        new CssBuilder("docs-section-content-toolbar")
            .AddClass($"outlined", Outlined)
            .AddClass("darken", ChildContent == null && Codes != null)
            .Build();

    protected string InnerClassname =>
        new CssBuilder("docs-section-content-inner")
            .AddClass($"relative d-flex flex-grow-1 flex-wrap justify-center")
            .AddClass("pa-8", !_hasCode)
            .AddClass("px-8 pb-8 pt-2", _hasCode)
            .Build();
    
    protected string SourceClassname =>
        new CssBuilder("docs-section-source")
            .AddClass($"outlined", Outlined)
            .AddClass("show-code", _hasCode && ShowCode)
            .Build();

    [Parameter] public string Class { get; set; }
    [Parameter] public bool DarkenBackground { get; set; }
    [Parameter] public bool Outlined { get; set; } = true;
    [Parameter] public bool ShowCode { get; set; } = true;
    [Parameter] public string Code { get; set; }
    [Parameter] public IEnumerable<CodeFile> Codes { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    
    private bool _hasCode;
    private string _activeCode;
    private string _luckyColor;
    
    protected override void OnParametersSet()
    {
        if(Codes != null)
        {
            _hasCode = true;
            _activeCode = Codes.FirstOrDefault().code;
        }
        else if(!String.IsNullOrWhiteSpace(Code))
        {
            _hasCode = true;
            _activeCode = Code;
        }

        if (DarkenBackground)
        {
            _luckyColor = GetDarkenColor();
        }
    }
    
    public void OnShowCode()
    {
        ShowCode = !ShowCode;
    }

    public void SetActiveCode(string value)
    {
        _activeCode = value;
    }

    private string GetActiveCode(string value)
    {
        if (value == _activeCode)
        {
            return "file-button active";
        }
        else
        {
            return "file-button";
        }
    }
    
    private async Task CopyTextToClipboard()
    {
        await JsApiService.CopyToClipboardAsync(Snippets.GetCode(Code));
    }

    private string GetDarkenColor()
    {
        string[] bgColors = {"primary", "secondary", "info", "warning", "error"};
        Random rnd = new Random();
        return bgColors[rnd.Next(4)];
    }

    RenderFragment CodeComponent(string code) => builder =>
    {
        try
        {
            var key = typeof(SectionContent).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains($".{code}Code.html"));
            using (var stream = typeof(SectionContent).Assembly.GetManifestResourceStream(key))
            using (var reader = new StreamReader(stream))
            {
                builder.AddMarkupContent(0, reader.ReadToEnd());
            }
        }
        catch (Exception)
        {
            // todo: log this
        }
    };
    
    protected virtual async void RunOnTryMudBlazor()
    {
        string firstFile = "";
        
        if(Codes != null)
        {
            firstFile = Codes.FirstOrDefault().code;
        }
        else
        {
            firstFile = Code;
        }
        
        // We use a separator that wont be in code so we can send 2 files later
        var codeFiles = "__Main.razor" + (char)31 + Snippets.GetCode(firstFile);

        // Add dialogs for dialog examples
        if (firstFile.StartsWith("Dialog"))
        {
            var regex = new Regex(@"\Show<(Dialog.*?_Dialog)\>");
            var dialogCodeName = regex.Match(codeFiles).Groups[1].Value;
            if (dialogCodeName != string.Empty)
            {
                var dialogCodeFile = dialogCodeName + ".razor" + (char)31 + Snippets.GetCode(dialogCodeName);
                codeFiles = codeFiles + (char)31 + dialogCodeFile;
            }
        }

        // Data models
        if (codeFiles.Contains("MudBlazor.Examples.Data.Models"))
        {
            if (Regex.Match(codeFiles, @"\bElement\b").Success)
            {
                var elementCodeFile = "Element.cs" + (char)31 + Snippets.GetCode("Element");
                codeFiles = codeFiles + (char)31 + elementCodeFile;
            }

            if (Regex.Match(codeFiles, @"\bServer\b").Success)
            {
                var serverCodeFile = "Server.cs" + (char)31 + Snippets.GetCode("Server");
                codeFiles = codeFiles + (char)31 + serverCodeFile;
            }
        }

        var codeFileEncoded = codeFiles.ToCompressedEncodedUrl();
        // var tryMudBlazorLocation = "https://localhost:5001/";
        var tryMudBlazorLocation = "https://try.mudblazor.com/";
        var url = $"{tryMudBlazorLocation}snippet/{codeFileEncoded}";
        await JsApiService.OpenInNewTabAsync(url);
    }
}
