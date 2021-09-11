using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components
{
    public partial class SectionSource
    {
        [Inject]
        protected IJsApiService JsApiService { get; set; }

        [Parameter] public string Code { get; set; }
        [Parameter] public string Code2 { get; set; }

        [Parameter] public string ButtonTextCode1 { get; set; }
        [Parameter] public string ButtonTextCode2 { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string GitHubFolderName { get; set; }

        [Parameter] public bool ShowCode { get; set; } = true;

        [Parameter] public bool NoToolbar { get; set; }

        private string GitHubSourceCode { get; set; }

        public string TooltipSourceCodeText { get; set; }

        private string ShowCodeExampleString { get; set; } = "Show code example";
        private string HideCodeExampleString { get; set; } = "Hide code example";

        private string CurrentCode { get; set; }
        private Color Button1Color { get; set; }
        private Color Button2Color { get; set; }

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
                    TooltipSourceCodeText = HideCodeExampleString;
                }
                else
                {
                    TooltipSourceCodeText = ShowCodeExampleString;
                }
            }
        }

        RenderFragment CodeComponent(string code) => builder =>
        {
            try
            {
                var key = typeof(SectionSource).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains($".{code}Code.html"));
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
            // We use a separator that wont be in code so we can send 2 files later
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
            await JsApiService.OpenInNewTabAsync(url);
        }

        protected override void OnInitialized()
        {
            CurrentCode = Code;
            Button1Color = Color.Primary;

            if (!string.IsNullOrEmpty(GitHubFolderName))
            {
                var gitHubLink = "https://github.com/";
                GitHubSourceCode = $"{gitHubLink}MudBlazor/MudBlazor/blob/master/src/MudBlazor.Docs/Pages/Components/{GitHubFolderName}/Examples/{Code}.razor";
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

        private void SwapCode(string code)
        {
            CurrentCode = code;

            if (CurrentCode == Code)
            {
                Button1Color = Color.Primary;
                Button2Color = Color.Default;
            }
            else if (CurrentCode == Code2)
            {
                Button1Color = Color.Default;
                Button2Color = Color.Primary;
            }
        }
    }
}
