using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorRepl.Client.Components
{
    public partial class CodeEditor
    {
        private const string EditorId = "user-code-editor";

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public ValueTask<string> GetCode() => this.JsRuntime.InvokeAsync<string>("window.App.getEditorValue");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JsRuntime.InvokeVoidAsync("window.App.initEditor", EditorId);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
