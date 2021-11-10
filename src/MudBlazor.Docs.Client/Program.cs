using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http;
using Blazor.Analytics;
using MudBlazor.Docs.Client;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<GitHubApiClient>();
builder.Services.TryAddDocsViewServices();
builder.Services.AddGoogleAnalytics("G-PRYNCB61NV");

await builder.Build().RunAsync();
