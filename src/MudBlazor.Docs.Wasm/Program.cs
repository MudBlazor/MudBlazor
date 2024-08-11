using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services.Notifications;
using MudBlazor.Docs.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// We use javascript to extract the data-prerender attribute which we created in the _Host.cshtml razor page.
// There is no other easy way to pass data into the wasm entry point since (args) is always null.
// Reference https://github.com/dotnet/aspnetcore/issues/24461
// We have to do this because the following code should only run when not prerendering.
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
