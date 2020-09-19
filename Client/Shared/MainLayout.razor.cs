namespace BlazorRepl.Client.Shared
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Components;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;

    public partial class MainLayout : IDisposable
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        public PageNotifications PageNotificationsComponent { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CompilationService.InitAsync(this.HttpClient);

            await base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.PageNotificationsComponent?.Dispose();
        }
    }
}
