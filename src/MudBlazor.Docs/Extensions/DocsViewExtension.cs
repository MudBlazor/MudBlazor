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
            services.TryAddMarkdownServices();
            services.TryAddDomHelpersServices(new CoreConfigBuilder().Build());
        }
    }
}
