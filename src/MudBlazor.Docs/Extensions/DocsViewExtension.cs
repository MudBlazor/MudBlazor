using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.Services;

namespace MudBlazor.Docs.Extensions
{
    public static class DocsViewExtension
    {
        public static void TryAddDocsViewServices(this IServiceCollection services)
        {
            services.AddMudBlazorResizeListener();
            services.AddMudBlazorDialog();
            services.AddMudBlazorSnackbar(config =>
            {
                config.PositionClass = Defaults.Classes.Position.BottomLeft;

                config.PreventDuplicates = false;
                config.NewestOnTop = false;
                config.ShowCloseIcon = true;
                config.VisibleStateDuration = 10000;
                config.HideTransitionDuration = 500;
                config.ShowTransitionDuration = 500;
                config.SnackbarVariant = Variant.Filled;
            });
            services.AddSingleton<IApiLinkService, ApiLinkService>();
            services.AddSingleton<IMenuService,MenuService>();
            services.AddScoped<IDocsNavigationService,DocsNavigationService>();
            
        }
    }
}
