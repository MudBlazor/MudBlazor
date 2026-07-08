using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Examples.DataGridState.Services;
using MudBlazor.Services;

namespace MudBlazor.Examples.DataGridState;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddMudServices();
        builder.Services.AddScoped<LocalStorageService>();
        builder.Services.AddScoped<PersonDataStore>();

        return builder.Build().RunAsync();
    }
}
