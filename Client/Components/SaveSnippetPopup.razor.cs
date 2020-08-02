namespace BlazorRepl.Client.Components
{
    using System;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Services;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class SaveSnippetPopup : IDisposable
    {
        private DotNetObjectReference<SaveSnippetPopup> dotNetInstance;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public SnippetsService SnippetsService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter]
        public string InvokerId { get; set; }

        [Parameter]
        public CodeEditor CodeEditor { get; set; }

        public bool Loading { get; set; }

        public string VisibleClass => this.Visible ? "show" : string.Empty;

        public string DisplayStyle => this.Visible ? string.Empty : "display: none;";

        public async Task SaveAsync()
        {
            if (this.CodeEditor == null)
            {
                throw new InvalidOperationException(
                    $"Cannot use save snippet popup without specified {nameof(this.CodeEditor)} parameter.");
            }

            this.Loading = true;

            try
            {
                var content = await this.CodeEditor.GetCodeAsync();

                var snippetId = await this.SnippetsService.SaveSnippetAsync(content);

                var urlBuilder = new UriBuilder(this.NavigationManager.BaseUri) { Path = $"repl/{snippetId}" };
                var url = urlBuilder.Uri.ToString();
                await this.JsRuntime.InvokeVoidAsync("App.changeDisplayUrl", url);
            }
            finally
            {
                this.Loading = false;
            }

            await this.CloseInternalAsync();
        }

        [JSInvokable]
        public Task CloseAsync() => this.CloseInternalAsync();

        public void Dispose()
        {
            this.dotNetInstance?.Dispose();
            _ = this.JsRuntime.InvokeVoidAsync("App.SaveSnippetPopup.dispose");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JsRuntime.InvokeVoidAsync(
                    "App.SaveSnippetPopup.init",
                    "save-snippet-popup",
                    this.InvokerId,
                    this.dotNetInstance);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private Task CloseInternalAsync()
        {
            this.Visible = false;
            return this.VisibleChanged.InvokeAsync(this.Visible);
        }
    }
}
