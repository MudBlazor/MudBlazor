namespace BlazorRepl.Client.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class CodeEditor
    {
        private const string EditorId = "user-code-editor";

        private bool shouldReinitEditor = false;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string DefaultCode { get; set; }

        public ValueTask<string> GetCode() => this.JsRuntime.InvokeAsync<string>("window.App.getEditorValue");

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(this.DefaultCode), out var parameterValue))
            {
                this.shouldReinitEditor = this.DefaultCode != parameterValue;
            }

            System.Console.WriteLine(this.shouldReinitEditor);

            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || this.shouldReinitEditor)
            {
                this.shouldReinitEditor = false;
                await this.JsRuntime.InvokeVoidAsync("App.initEditor", EditorId, this.DefaultCode);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
