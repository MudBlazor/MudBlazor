namespace BlazorRepl.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Services;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddTransient<SnippetsService>();
            builder.Services.AddSingleton(new ComponentCompilationService());

            builder.Services.AddSingleton(serviceProvider =>
            {
                var snippetsOptions = new SnippetsOptions();

                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                configuration.GetSection("Snippets").Bind(snippetsOptions);

                return snippetsOptions;
            });

            await builder.Build().RunAsync();
        }
    }
}
