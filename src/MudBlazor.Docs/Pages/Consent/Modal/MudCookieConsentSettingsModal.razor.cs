// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace MudBlazor.Docs.Pages.Consent.Modal;

#nullable enable
public partial class MudCookieConsentSettingsModal : BytexDigital.Blazor.Components.CookieConsent.Dialogs.Settings.CookieConsentSettingsModalComponentBase
{
    [Inject]
    protected IOptions<CookieConsentOptions> Options { get; set; } = null!;

    [Inject]
    protected CookieConsentService CookieConsentService { get; set; } = null!;

    [Inject]
    public CookieConsentLocalizer Localizer { get; set; } = null!;

    public List<string> AcceptedCategories { get; set; } = new();

    public List<string> AcceptedServices { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var preferences = await CookieConsentService.GetPreferencesAsync();

        AcceptedCategories = preferences.AllowedCategories.ToList();
        AcceptedServices = preferences.AllowedServices.ToList();

        foreach (var category in Options.Value.Categories.Where(x => x.IsRequired))
        {
            AcceptedCategories.Add(category.Identifier);
            AcceptedServices.AddRange(category.Services.Select(x => x.Identifier));
        }

        if (!await CookieConsentService.IsCurrentRevisionAcceptedAsync())
        {
            // Everytime we have a new revision, we want to also add all preselected categories and services
            foreach (var category in Options.Value.Categories.Where(x => x.IsPreselected))
            {
                AcceptedCategories.Add(category.Identifier);
                AcceptedServices.AddRange(category.Services.Select(x => x.Identifier));
            }
        }

        // Cleanup
        AcceptedCategories = AcceptedCategories.Distinct().ToList();
        AcceptedServices = AcceptedServices.Distinct().ToList();
    }

    private async Task AllowAllAsync()
    {
        await CookieConsentService.SavePreferencesAcceptAllAsync();
        await OnClosePreferences.InvokeAsync(true);
    }

    private async Task AllowSelectedAsync()
    {
        await CookieConsentService.SavePreferencesAsync(new CookiePreferences
        {
            AcceptedRevision = Options.Value.Revision,
            AllowedCategories = AcceptedCategories.ToArray(),
            AllowedServices = AcceptedServices.ToArray()
        });

        await OnClosePreferences.InvokeAsync(true);
    }

    private void SelectedChanged(CookieCategory category, bool isAllowed)
    {
        if (isAllowed)
        {
            if (!AcceptedCategories.Contains(category.Identifier))
            {
                AcceptedCategories.Add(category.Identifier);
            }

            foreach (var service in category.Services.Where(service => !AcceptedServices.Contains(service.Identifier)))
            {
                AcceptedServices.Add(service.Identifier);
            }
        }
        else if (!isAllowed)
        {
            if (AcceptedCategories.Contains(category.Identifier))
            {
                AcceptedCategories.Remove(category.Identifier);
            }

            foreach (var service in category.Services.Where(service => AcceptedServices.Contains(service.Identifier)))
            {
                AcceptedServices.Remove(service.Identifier);
            }
        }
    }
}
