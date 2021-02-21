using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Extensions;
using MudBlazor.Services;
using MudBlazor.Extensions;

namespace MudBlazor.Docs.Components
{
    public partial class SectionSource
    {
        [Inject]
        protected IJsApiService JsApiService { get; set; }
        
        [Parameter] public string Title { get; set; }

        [Parameter] public string Code { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string GitHubFolderName { get; set; }

        [Parameter] public bool ShowCode { get; set; } = true;

        [Parameter] public bool HideButtons { get; set; }

        [Parameter] public bool NoToolbar { get; set; }

        private string GitHubSourceCode { get; set; }

        public string TooltipSourceCodeText { get; set; }

        private string showCodeExampleString { get; set; } = "Show code example";
        private string hideCodeExampleString { get; set; } = "Hide code example";
        private string showComponentCodeExampleString { get; set; } = "Show component code example";
        private string hideComponentCodeExampleString { get; set; } = "Hide component code example";

        private async Task CopyTextToClipboard()
        {
            await JsApiService.CopyToClipboardAsync(Snippets.GetCode(Code));
        }

        public void OnShowCode()
        {
            if (!string.IsNullOrEmpty(Code))
            {
                ShowCode = !ShowCode;
                if (ShowCode)
                {
                    TooltipSourceCodeText = hideCodeExampleString;
                }
                else
                {
                    TooltipSourceCodeText = showCodeExampleString;
                }
            }
        }

        private Type CodeType => Type.GetType("MudBlazor.Docs.Examples.Markup." + Code + "Code");

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

            // Add Element.cs model for webapi periodic table
            if (codeFiles.Contains("webapi/periodictable", StringComparison.InvariantCultureIgnoreCase))
            {
                var elementCodeFile = "Element.cs" + (char)31 + Snippets.GetCode("Element");
                codeFiles = codeFiles + (char)31 + elementCodeFile;
            }

            var codeFileEncoded = codeFiles.ToCompressedEncodedUrl();
            // var tryMudBlazorLocation = "https://localhost:5001/";
            var tryMudBlazorLocation = "https://try.mudblazor.com/";
            var url = $"{tryMudBlazorLocation}snippet/{codeFileEncoded}";
            await JsApiService.OpenInNewTabAsync(url);
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
                TooltipSourceCodeText = hideCodeExampleString;
            }
            else
            {
                TooltipSourceCodeText = showCodeExampleString;
            }
        }
    }
}
