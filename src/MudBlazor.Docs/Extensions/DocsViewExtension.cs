using Blazor.Analytics;
using Blazored.LocalStorage;
using BytexDigital.Blazor.Components.CookieConsent;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Pages.Consent.Prompt;
using MudBlazor.Docs.Services;
using MudBlazor.Docs.Services.Notifications;
using MudBlazor.Docs.Services.UserPreferences;
using MudBlazor.Examples.Data;
using MudBlazor.Services;

namespace MudBlazor.Docs.Extensions
{
    public static class DocsViewExtension
    {
        public static void TryAddDocsViewServices(this IServiceCollection services)
        {
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;

                // we're currently planning on deprecating `PreventDuplicates`, at least to the end dev. however,
                // we may end up wanting to instead set it as internal because the docs project relies on it
                // to ensure that the Snackbar always allows duplicates. disabling the warning for now because
                // the project is set to treat warnings as errors.
#pragma warning disable 0618
                config.SnackbarConfiguration.PreventDuplicates = false;
#pragma warning restore 0618
            });

            services.AddScoped<IDocsJsApiService, DocsJsApiService>();
            services.AddSingleton<DiscordApiClient>();
            services.AddSingleton<NugetApiClient>();
            services.AddSingleton<GitHubApiClient>();
            services.AddSingleton<IApiLinkService, ApiLinkService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddScoped<IDocsNavigationService, DocsNavigationService>();
            services.AddBlazoredLocalStorage();
            services.AddScoped<IUserPreferencesService, UserPreferencesService>();
            services.AddScoped<INotificationService, InMemoryNotificationService>();
            services.AddSingleton<IPeriodicTableService, PeriodicTableService>();
            services.AddSingleton<IRenderQueueService, RenderQueueService>();
            services.AddScoped<LayoutService>();
            services.AddGoogleAnalytics("G-PRYNCB61NV");
            services.AddCookieConsent(options =>
            {
                options.ImportJsAutomatically = false;
                options.Revision = 1;
                options.PolicyUrl = "/mud/cookie-policy";

                // Replace default prompt. We don't use the modal.
                options.ConsentPromptVariant = new MudCookieConsentPromptVariant();

                options.Categories.Add(new CookieCategory()
                {
                    IsRequired = true,
                    TitleText = new()
                    {
                        ["en"] = "Azure Cloud Platform",
                    },
                    DescriptionText = new()
                    {
                        ["en"] = "It is used for load balancing to make sure the visitor page requests are routed to the same server in any browsing session.",
                    },
                    Services =
                    [
                        new CookieCategoryService
                        {
                            Identifier = "necessary",
                            PolicyUrl = "https://privacy.microsoft.com/en-us/privacystatementy",
                            TitleText = new()
                            {
                                ["en"] = "Azure App Service",
                            },
                            ShowPolicyText = new()
                            {
                                ["en"] = "Display policies",
                            },
                        }
                    ]
                });

                options.Categories.Add(new CookieCategory
                {
                    TitleText = new()
                    {
                        ["en"] = "Google Services",
                    },
                    DescriptionText = new()
                    {
                        ["en"] = "Allows the integration and usage of Google services.",
                    },
                    Identifier = "google",
                    IsPreselected = true,
                    Services =
                    [
                        new CookieCategoryService
                        {
                            Identifier = "google-analytics",
                            PolicyUrl = "https://policies.google.com/privacy",
                            TitleText = new()
                            {
                                ["en"] = "Google Analytics",
                            },
                            ShowPolicyText = new()
                            {
                                ["en"] = "Display policies",
                            }
                        }
                    ]
                });
            });
        }
    }
}
