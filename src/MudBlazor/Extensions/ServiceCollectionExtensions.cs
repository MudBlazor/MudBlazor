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

        /// <summary>
        /// Adds a singleton <see cref="IToaster"/> instance to the DI <see cref="IServiceCollection"/> with the specified <see cref="ToasterConfiguration"/>
        /// </summary>
        public static IServiceCollection AddToaster(this IServiceCollection services, ToasterConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            services.TryAddScoped<IToaster>(builder => new Toaster(configuration));
            return services;
        }

        /// <summary>
        /// Adds a singleton <see cref="IToaster"/> instance to the DI <see cref="IServiceCollection"/> with the default <see cref="ToasterConfiguration"/>
        /// </summary>
        public static IServiceCollection AddToaster(this IServiceCollection services)
        {
            return AddToaster(services, new ToasterConfiguration());
        }

        /// <summary>
        /// Adds a singleton <see cref="IToaster"/> instance to the DI <see cref="IServiceCollection"/> with an action for configuring the default <see cref="ToasterConfiguration"/>
        /// </summary>
        public static IServiceCollection AddToaster(this IServiceCollection services, Action<ToasterConfiguration> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new ToasterConfiguration();
            configure(options);

            return AddToaster(services, options);
        }
    }
}
