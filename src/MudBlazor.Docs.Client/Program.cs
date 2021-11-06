using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Docs.Client;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using Blazor.Analytics;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<GitHubApiClient>();
builder.Services.TryAddDocsViewServices();
builder.Services.AddGoogleAnalytics("G-PRYNCB61NV");

await builder.Build().RunAsync();
