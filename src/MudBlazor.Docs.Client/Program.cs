using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http;
using Blazor.Analytics;
using MudBlazor.Docs.Client;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<GitHubApiClient>();
builder.Services.TryAddDocsViewServices();
builder.Services.AddGoogleAnalytics("G-PRYNCB61NV");

var build = builder.Build();

var notificationService = build.Services.GetService<INotificationService>();
if (notificationService is InMemoryNotificationService inmemoryService)
{
    inmemoryService.Preload();
}


await build.RunAsync();
