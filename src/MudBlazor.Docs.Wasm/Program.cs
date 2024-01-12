using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

using System;
using System.Net.Http;
using MudBlazor.Docs.Wasm;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.Notifications;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var js = (IJSInProcessRuntime)builder.Services.BuildServiceProvider().GetRequiredService<IJSRuntime>();
var preRender = js.Invoke<string>("getPreRender");
if (preRender != "True")
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.TryAddDocsViewServices();

var build = builder.Build();

var notificationService = build.Services.GetService<INotificationService>();
if (notificationService is InMemoryNotificationService inMemoryService)
{
    inMemoryService.Preload();
}

await build.RunAsync();
