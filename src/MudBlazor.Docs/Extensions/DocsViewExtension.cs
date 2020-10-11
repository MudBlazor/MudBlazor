using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Dialog;

namespace MudBlazor.Docs.Extensions
{
    public static class DocsViewExtension
    {
        public static void TryAddDocsViewServices(this IServiceCollection services)
        {
            services.AddMudBlazorDialog();
            services.AddMudBlazorSnackbar(config =>
            {
                config.PositionClass = Defaults.Classes.Position.BottomLeft;

                config.PreventDuplicates = false;
                config.NewestOnTop = false;
                config.VisibleStateDuration = 10000;
                config.HideTransitionDuration = 500;
                config.ShowTransitionDuration = 500;
            });
        }
    }
}
