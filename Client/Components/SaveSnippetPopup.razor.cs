namespace BlazorRepl.Client.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Components.Models;
    using BlazorRepl.Client.Services;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class SaveSnippetPopup : IDisposable
    {
        private DotNetObjectReference<SaveSnippetPopup> dotNetInstance;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public SnippetsService SnippetsService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        public PageNotifications PageNotificationsComponent { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter]
        public string InvokerId { get; set; }

        [Parameter]
        public IEnumerable<CodeFile> CodeFiles { get; set; } = Enumerable.Empty<CodeFile>();

        [Parameter]
        public Func<Task> UpdateActiveCodeFileContentFunc { get; set; }

        public bool Loading { get; set; }

        public string SnippetLink { get; set; }

        public bool SnippetLinkCopied { get; set; }

        public string VisibleClass => this.Visible ? "show" : string.Empty;

        public string CopyButtonIcon => this.SnippetLinkCopied ? "icon-check" : "icon-copy";

        public string DisplayStyle => this.Visible ? string.Empty : "display: none;";

        public async Task CopyLinkToClipboardAsync()
        {
            await this.JsRuntime.InvokeVoidAsync("App.copyToClipboard", this.SnippetLink);
            this.SnippetLinkCopied = true;
        }

        public async Task SaveAsync()
        {
            this.Loading = true;

            try
            {
                await (this.UpdateActiveCodeFileContentFunc?.Invoke() ?? Task.CompletedTask);

                var snippetId = await this.SnippetsService.SaveSnippetAsync(this.CodeFiles);

                var urlBuilder = new UriBuilder(this.NavigationManager.BaseUri) { Path = $"repl/{snippetId}" };
                var url = urlBuilder.Uri.ToString();
                this.SnippetLink = url;

                await this.JsRuntime.InvokeVoidAsync("App.changeDisplayUrl", url);
            }
            catch (InvalidOperationException ex)
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, content: ex.Message);
            }
            catch (Exception)
            {
                this.PageNotificationsComponent.AddNotification(
                    NotificationType.Error,
                    content: "Error while saving snippet. Please try again later.");
            }
            finally
            {
                this.Loading = false;
            }
        }

        [JSInvokable]
        public Task CloseAsync() => this.CloseInternalAsync();

        public void Dispose()
        {
            this.dotNetInstance?.Dispose();

            _ = this.JsRuntime.InvokeVoidAsync("App.SaveSnippetPopup.dispose");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JsRuntime.InvokeVoidAsync(
                    "App.SaveSnippetPopup.init",
                    "save-snippet-popup",
                    this.InvokerId,
                    this.dotNetInstance);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private Task CloseInternalAsync()
        {
            this.Visible = false;
            this.SnippetLink = null;
            this.SnippetLinkCopied = false;
            return this.VisibleChanged.InvokeAsync(this.Visible);
        }
    }
}
