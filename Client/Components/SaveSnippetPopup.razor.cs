namespace BlazorRepl.Client.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class SaveSnippetPopup
    {
        // TODO: Dispose
        private DotNetObjectReference<SaveSnippetPopup> dotNetInstance;

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter]
        public string InvokerId { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public string VisibleClass => this.Visible ? "show" : string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JSRuntime.InvokeVoidAsync(
                    "window.App.initSaveSnippetPopup",
                    "save-snippet-popup",
                    this.InvokerId,
                    this.dotNetInstance);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public async Task Close()
        {
            this.Visible = false;
            await this.VisibleChanged.InvokeAsync(this.Visible);
        }
    }
}
