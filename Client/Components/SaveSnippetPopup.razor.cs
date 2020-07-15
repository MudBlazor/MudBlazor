namespace BlazorRepl.Client.Components
{
    using System;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Services;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class SaveSnippetPopup
    {
        // TODO: Dispose
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

        public string VisibleClass => this.Visible ? "show" : string.Empty;

        public async Task SaveAsync()
        {
            if (this.CodeEditor == null)
            {
                throw new InvalidOperationException(
                    $"Cannot use save snippet popup without specified {nameof(this.CodeEditor)} parameter.");
            }

            var content = await this.CodeEditor.GetCode();

            var snippetId = await this.SnippetsService.SaveSnippetAsync(content);

            var urlBuilder = new UriBuilder(this.NavigationManager.BaseUri) { Path = $"repl/{snippetId}" };

            var url = urlBuilder.Uri.ToString();
            await this.JsRuntime.InvokeVoidAsync("window.App.changeDisplayUrl", url);

            await this.CloseInternalAsync();
        }

        [JSInvokable]
        public Task Close() => this.CloseInternalAsync();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JsRuntime.InvokeVoidAsync(
                    "window.App.initSaveSnippetPopup",
                    "save-snippet-popup",
                    this.InvokerId,
                    this.dotNetInstance);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task CloseInternalAsync()
        {
            this.Visible = false;
            await this.VisibleChanged.InvokeAsync(this.Visible);
        }
    }
}
