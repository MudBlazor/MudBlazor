namespace BlazorRepl.Client.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class CodeEditor
    {
        private const string EditorId = "user-code-editor";

        private bool shouldReInitEditor;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string DefaultCode { get; set; }

        public ValueTask<string> GetCodeAsync() => this.JsRuntime.InvokeAsync<string>("window.App.getEditorValue");

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(this.DefaultCode), out var parameterValue))
            {
                this.shouldReInitEditor = this.DefaultCode != parameterValue;
            }

            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || this.shouldReInitEditor)
            {
                this.shouldReInitEditor = false;

                await this.JsRuntime.InvokeVoidAsync("App.initEditor", EditorId, this.DefaultCode);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
