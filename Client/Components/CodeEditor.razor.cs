namespace BlazorRepl.Client.Components
{
    using System;
    using System.Threading.Tasks;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class CodeEditor : IDisposable
    {
        private const string EditorId = "user-code-editor";

        private bool hasCodeChanged;
        private bool? isFirstCodeChange;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public string Code { get; set; }

        public ValueTask<string> GetCodeAsync() => this.JsRuntime.InvokeAsync<string>("App.CodeEditor.getValue");

        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(this.Code), out var parameterValue))
            {
                this.hasCodeChanged = this.Code != parameterValue;
                if (this.hasCodeChanged)
                {
                    this.isFirstCodeChange = !this.isFirstCodeChange.HasValue;
                }
            }

            return base.SetParametersAsync(parameters);
        }

        public void Dispose() => _ = this.JsRuntime.InvokeAsync<string>("App.CodeEditor.dispose");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (this.isFirstCodeChange == true)
            {
                await this.JsRuntime.InvokeVoidAsync(
                    "App.CodeEditor.init",
                    EditorId,
                    this.Code ?? CoreConstants.MainComponentDefaultFileContent);
            }
            else if (this.isFirstCodeChange == false)
            {
                await this.JsRuntime.InvokeVoidAsync("App.CodeEditor.setValue", this.Code);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
