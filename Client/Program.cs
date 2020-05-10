using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using BlazorFiddlePoC.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorFiddlePoC.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(new ComponentCompilationService());
            builder.Services.AddTelerikBlazor();
           //builder.Services.AddScoped<IJSRuntime>();



           await builder.Build().RunAsync();
        }
    }
}
