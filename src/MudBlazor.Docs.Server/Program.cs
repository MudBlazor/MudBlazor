using System;
using System.Net.Http;
using Blazor.Analytics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;
using MudBlazor.Examples.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IPeriodicTableService, PeriodicTableService>();
builder.Services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.Configuration["ApiBase"]) });
builder.Services.AddScoped<GitHubApiClient>();
builder.Services.TryAddDocsViewServices();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddGoogleAnalytics("G-PRYNCB61NV");
if (builder.Configuration["Azure:SignalR:Enabled"] == "true")
{
    builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);
}

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

// serve the wasm site and finish the pipeline
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/wasm"), wasm =>
{
    wasm.UseBlazorFrameworkFiles("/wasm");
    wasm.UseStaticFiles("/wasm");
    wasm.UseRouting();
    wasm.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapFallbackToFile("wasm/{*path:nonfile}", "wasm/index.html");
    });
});

// only reach here if path does not start /wasm
app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
