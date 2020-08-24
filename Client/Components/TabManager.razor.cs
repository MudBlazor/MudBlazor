namespace BlazorRepl.Client.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class TabManager : IDisposable
    {
        private const int DefaultActiveIndex = 0;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public IList<string> Tabs { get; set; }

        [Parameter]
        public EventCallback<string> OnTabActivate { get; set; }

        [Parameter]
        public EventCallback<string> OnTabClose { get; set; }

        public int ActiveIndex { get; set; } = DefaultActiveIndex;

        public Task ActivateTabAsync(int activeIndex)
        {
            if (activeIndex < 0 || activeIndex >= this.Tabs.Count)
            {
                return Task.CompletedTask;
            }

            this.ActiveIndex = activeIndex;

            return this.OnTabActivate.InvokeAsync(this.Tabs[activeIndex]);
        }

        public async Task CloseTabAsync(int index)
        {
            if (index < 0 || index >= this.Tabs.Count)
            {
                return;
            }

            if (index == DefaultActiveIndex)
            {
                return;
            }

            var tab = this.Tabs[index];
            this.Tabs.RemoveAt(index);

            await this.OnTabClose.InvokeAsync(tab);

            if (index == this.ActiveIndex)
            {
                this.ActiveIndex = DefaultActiveIndex;
                await this.OnTabActivate.InvokeAsync(this.Tabs[this.ActiveIndex]);
            }
        }

        public void Dispose() => _ = this.JsRuntime.InvokeAsync<string>("App.TabManager.dispose");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await this.JsRuntime.InvokeVoidAsync("App.TabManager.init");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
