namespace BlazorRepl.Client.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Components;
    using BlazorRepl.Client.Services;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class Repl
    {
        private const string BasicUserComponentCodePrefix =
    @"@page ""/user-page""
@using System.ComponentModel.DataAnnotations
@using System.Linq
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
";

        private DotNetObjectReference<Repl> dotNetInstance;

        [Inject]
        public SnippetsService SnippetsService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ComponentCompilationService CompilationService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string SnippetId { get; set; }

        public CodeEditor CodeEditor { get; set; }

        public string SnippetContent { get; set; }

        public string Preset { get; set; } = "basic";

        public string UserComponentCodePrefix => BasicUserComponentCodePrefix;

        public IReadOnlyCollection<CompilationDiagnostic> Diagnostics { get; set; } = Array.Empty<CompilationDiagnostic>();

        public bool AreDiagnosticsShown { get; set; }

        public string LoaderText { get; set; }

        public bool Loading { get; set; }

        public int UserComponentCodeStartLine => this.UserComponentCodePrefix.Count(ch => ch == '\n');

        public async Task UpdateLoaderText(string loaderText)
        {
            this.LoaderText = loaderText;

            this.StateHasChanged();

            await Task.Delay(10); // Ensure rendering has time to be called
        }

        public async Task Compile()
        {
            this.Loading = true;
            this.LoaderText = "Processing";

            await Task.Delay(10); // Ensure rendering has time to be called

            var code = await this.CodeEditor.GetCode();

            var result = await this.CompilationService.CompileToAssembly(
                "UserPage.razor",
                this.UserComponentCodePrefix + code,
                this.Preset,
                this.UpdateLoaderText);

            this.Diagnostics = result.Diagnostics.OrderByDescending(x => x.Severity).ThenBy(x => x.Code).ToList();
            this.AreDiagnosticsShown = true;

            this.Loading = false;

            if (result.AssemblyBytes != null && result.AssemblyBytes.Length > 0)
            {
                await this.JsRuntime.InvokeVoidAsync("window.App.readFile", result.AssemblyBytes);

                // TODO: Add error page in iframe
                await this.JsRuntime.InvokeVoidAsync("window.App.reloadIFrame", "user-page-window");
            }
        }

        public async Task Save()
        {
            var content = await this.CodeEditor.GetCode();

            var snippetId = await this.SnippetsService.SaveSnippetAsync(content);

            var urlBuilder = new UriBuilder(this.NavigationManager.BaseUri) { Path = $"repl/{snippetId}" };

            var url = urlBuilder.Uri.ToString();
            await this.JsRuntime.InvokeVoidAsync("window.App.changeDisplayUrl", url);
        }

        [JSInvokable]
        public async Task OnCompileEvent()
        {
            await this.Compile();
            this.StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JsRuntime.InvokeVoidAsync(
                    "window.App.initRepl",
                    "user-code-editor-container",
                    "user-page-window-container",
                    "user-code-editor",
                    this.dotNetInstance);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.SnippetId))
            {
                this.SnippetContent = await this.SnippetsService.GetSnippetContentAsync(this.SnippetId);
            }

            await base.OnInitializedAsync();
        }
    }
}
