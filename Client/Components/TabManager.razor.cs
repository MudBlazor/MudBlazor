namespace BlazorRepl.Client.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Components.Models;
    using BlazorRepl.Client.Services;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.JSInterop;

    public partial class TabManager : IDisposable
    {
        private const int DefaultActiveIndex = 0;
        private const string EnterKey = "Enter";

        private bool tabCreating;
        private bool shouldFocusNewTabInput;
        private string newTab;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public IList<string> Tabs { get; set; }

        [Parameter]
        public EventCallback<string> OnTabActivate { get; set; }

        [Parameter]
        public EventCallback<string> OnTabClose { get; set; }

        [Parameter]
        public EventCallback<string> OnTabCreate { get; set; }

        [CascadingParameter]
        public PageNotifications PageNotificationsComponent { get; set; }

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
                await this.ActivateTabAsync(DefaultActiveIndex);
            }
        }

        public void InitTabCreating()
        {
            this.tabCreating = true;
            this.shouldFocusNewTabInput = true;
        }

        public async Task CreateTabAsync()
        {
            if (string.IsNullOrWhiteSpace(this.newTab))
            {
                this.tabCreating = false;
                this.newTab = null;
                return;
            }

            // TODO: Abstract to not use "code file" stuff
            var normalizedTab = CodeFilesHelper.NormalizeCodeFilePath(this.newTab, out var error);
            if (!string.IsNullOrWhiteSpace(error))
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, error);
                return;
            }

            Console.WriteLine("normalized");
            this.Tabs.Add(normalizedTab);

            this.newTab = null;
            this.tabCreating = false;
            var newTabIndex = this.Tabs.Count - 1;

            await this.OnTabCreate.InvokeAsync(normalizedTab);

            await this.ActivateTabAsync(newTabIndex);
        }

        public Task OnKeyDownAsync(KeyboardEventArgs eventArgs)
        {
            if (eventArgs.Key == EnterKey)
            {
                return this.CreateTabAsync();
            }

            return Task.CompletedTask;
        }

        public void Dispose() => _ = this.JsRuntime.InvokeAsync<string>("App.TabManager.dispose");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await this.JsRuntime.InvokeVoidAsync("App.TabManager.init");
            }

            if (this.shouldFocusNewTabInput)
            {
                await this.JsRuntime.InvokeVoidAsync("App.focusElement", "#new-tab-input");

                this.shouldFocusNewTabInput = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
