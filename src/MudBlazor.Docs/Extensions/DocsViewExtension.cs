using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skclusive.Core.Component;
using Skclusive.Markdown.Component;
using Skclusive.Script.DomHelpers;
using MudBlazor.Dialog;

namespace MudBlazor.Docs.Extensions
{
    public static class DocsViewExtension
    {
        public static void TryAddDocsViewServices(this IServiceCollection services)
        {
            services.AddMudBlazorDialog();
            //services.AddToaster();
            services.AddToaster(config =>
            {
                config.PositionClass = Defaults.Classes.Position.BottomLeft;
                config.ToastTitleClass = $"{Defaults.Classes.ToastTitle} {Defaults.Classes.TextPosition.Left}";
                config.ToastMessageClass = $"{Defaults.Classes.ToastMessage} {Defaults.Classes.TextPosition.Left}";

                config.PreventDuplicates = false;
                config.NewestOnTop = false;
                config.MaximumOpacity = 100;
                config.ShowProgressBar = false;
                config.VisibleStateDuration = 10000;
                config.HideTransitionDuration = 500;
                config.ShowTransitionDuration = 500;
            });
            services.TryAddMarkdownServices();
            services.TryAddDomHelpersServices(new CoreConfigBuilder().Build());
        }
    }
}
