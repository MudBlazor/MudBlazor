namespace BlazorRepl.Client.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class CodeEditor
    {
        private const string EditorId = "user-code-editor";

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string DefaultCode { get; set; }

        public ValueTask<string> GetCode() => this.JsRuntime.InvokeAsync<string>("window.App.getEditorValue");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JsRuntime.InvokeVoidAsync("App.initEditor", EditorId, this.DefaultCode);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
