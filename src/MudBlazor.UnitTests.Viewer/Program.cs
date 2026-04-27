using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Examples.Data;
using MudBlazor.Services;
using MudBlazor.UnitTests.Shared.Search;

namespace MudBlazor.UnitTests;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddMudServices();
        builder.Services.AddSingleton<IPeriodicTableService, PeriodicTableService>();
        builder.Services.AddSingleton<IFuzzySearchService, FuzzySearchService>();

        return builder.Build().RunAsync();
    }
}
