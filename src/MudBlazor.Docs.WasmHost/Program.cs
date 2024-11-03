using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;
using MudBlazor.Docs.WasmHost.Prerender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpContextAccessor();


//adding client specific service for prerendering. This service are not used by the WASM app, but for prerending it. Thefore they are different
builder.Services.AddScoped(sp =>
{
    var context = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var client = new HttpClient { BaseAddress = new Uri($"{context?.Request.Scheme}://{context?.Request.Host}{context?.Request.PathBase}") };

    return client;
});
builder.Services.TryAddDocsViewServices();
//set the capacity max so that content is not queue. Again this is for prerending to serve the entire page back to crawler
builder.Services.AddScoped<IRenderQueueService>(s => new RenderQueueService { Capacity = int.MaxValue });

builder.Services.AddSingleton<ICrawlerIdentifier>(new FileBasedCrawlerIdentifier("CrawlerInfo.json"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UsePrerenderMiddleware();

app.MapRazorPages();
app.MapControllers();

app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var notificationService = scope.ServiceProvider.GetService<INotificationService>();
    if (notificationService is InMemoryNotificationService inMemoryService)
    {
        inMemoryService.Preload();
    }

    var crawlerIdentifier = scope.ServiceProvider.GetRequiredService<ICrawlerIdentifier>();
    await crawlerIdentifier.Initialize();
}

app.Run();
