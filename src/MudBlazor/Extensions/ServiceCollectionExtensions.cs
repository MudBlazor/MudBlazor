using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Dialog;

namespace MudBlazor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMudBlazorDialog(this IServiceCollection services)
        {
            return services.AddScoped<IDialogService, DialogService>();
        }

        public static IServiceCollection AddToaster(this IServiceCollection services, SnackbarConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            services.TryAddScoped<ISnackbar>(builder => new Snackbars(configuration));
            return services;
        }

        public static IServiceCollection AddSnackbar(this IServiceCollection services)
        {
            return AddSnackbar(services, new SnackbarConfiguration());
        }

        public static IServiceCollection AddSnackbar(this IServiceCollection services, Action<SnackbarConfiguration> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new SnackbarConfiguration();
            configure(options);

            return AddSnackbar(services, options);
        }
    }
}
