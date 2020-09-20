using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Dialog;

namespace MudBlazor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMudBlazorDialog(this IServiceCollection services)
        {
            return services.AddScoped<IDialogService, DialogService>();
        }
    }
}
