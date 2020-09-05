namespace BlazorRepl.Client.Components
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class CodeEditor : IDisposable
    {
        private const string EditorId = "user-code-editor";

        private bool shouldReInitEditor;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string Code { get; set; }

        public ValueTask<string> GetCodeAsync() => this.JsRuntime.InvokeAsync<string>("App.CodeEditor.getValue");

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(this.Code), out var parameterValue))
            {
                this.shouldReInitEditor = this.Code != parameterValue;
            }

            return base.SetParametersAsync(parameters);
        }

        public void Dispose() => _ = this.JsRuntime.InvokeAsync<string>("App.CodeEditor.dispose");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JsRuntime.InvokeVoidAsync("App.CodeEditor.init", EditorId, this.Code);
            }
            else if (this.shouldReInitEditor)
            {
                this.shouldReInitEditor = false;

                await this.JsRuntime.InvokeVoidAsync("App.CodeEditor.setValue", this.Code);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
