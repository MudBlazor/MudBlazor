using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Analytics;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace MudBlazor.Docs.Client
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<GitHubApiClient>();
            builder.Services.TryAddDocsViewServices();
            builder.Services.AddHeadElementHelper();
            builder.Services.AddGoogleAnalytics("G-PRYNCB61NV");

            return builder.Build().RunAsync();
        }
    }
}
