using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
using MudBlazor.Services;

namespace MudBlazor.Docs.Components
{
    public partial class SectionSource
    {
        [Inject]
        protected IJSRuntime  JSRuntime { get; set; }

        [Inject]
        protected IDomService DomService { get; set; }
        
        [Parameter] public string Title { get; set; }

        [Parameter] public string Code { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string GitHubFolderName { get; set; }

        [Parameter] public bool ShowCode { get; set; } = true;

        [Parameter] public bool NoToolbar { get; set; }

        private bool HideEditOnTryMudBlazor => Code.EndsWith("_Dialog");

        private string GitHubSourceCode { get; set; }

        public string TooltipSourceCodeText { get; set; }

        private string ShowCodeExampleString { get; set; } = "Show code example";
        private string HideCodeExampleString { get; set; } = "Hide code example";

        private async Task CopyTextToClipboard()
        {
            await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", Snippets.GetCode(Code));
        }

        public void OnShowCode()
        {
            if (!string.IsNullOrEmpty(Code))
            {
                ShowCode = !ShowCode;
                if (ShowCode)
                {
                    TooltipSourceCodeText = HideCodeExampleString;
                }
                else
                {
                    TooltipSourceCodeText = ShowCodeExampleString;
                }
            }
        }

        RenderFragment CodeComponent() => builder =>
        {
            try
            {
                var key = typeof(SectionSource).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains($".{Code}Code.html"));
                using (var stream = typeof(SectionSource).Assembly.GetManifestResourceStream(key))
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

        protected virtual async void EditOnTryMudBlazor()
        {
            // We use a seperator that wont be in code so we can send 2 files later
            var codeFiles = "__Main.razor" + (char)31 + Snippets.GetCode(Code);

            // Add dialogs for dialog examples
            if (Code.StartsWith("Dialog"))
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
            await DomService.OpenInNewTab(url);
        }

        protected override void OnInitialized()
        {
            if (!string.IsNullOrEmpty(GitHubFolderName))
            {
                var gitHubLink = "https://github.com/";
                GitHubSourceCode = $"{gitHubLink}Garderoben/MudBlazor/blob/master/src/MudBlazor.Docs/Pages/Components/{GitHubFolderName}/Examples/{Code}.razor";
            }
            if (ShowCode)
            {
                TooltipSourceCodeText = HideCodeExampleString;
            }
            else
            {
                TooltipSourceCodeText = ShowCodeExampleString;
            }
        }
    }
}