// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using BytexDigital.Blazor.Components.CookieConsent;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor.State;

namespace MudBlazor.Docs.Pages.Consent.Modal;

#nullable enable
public partial class MudCookieConsentSettingsCategory : MudComponentBase
{
    private readonly IParameterState<bool> _selectedState;

    [Inject]
    protected IOptions<CookieConsentOptions> Options { get; set; } = null!;

    [Inject]
    public CookieConsentLocalizer Localizer { get; set; } = null!;

    [Parameter]
    public CookieCategory Category { get; set; } = new();

    [Parameter]
    public bool Selected { get; set; }

    [Parameter]
    public EventCallback<bool> SelectedChanged { get; set; }

    private bool Collapsed { get; set; } = true;

    public MudCookieConsentSettingsCategory()
    {
        _selectedState = RegisterParameter(nameof(Selected), () => Selected, () => SelectedChanged);
    }

    private async Task OnChangedAsync(ChangeEventArgs args)
    {
        if (args.Value is bool newValue)
        {
            await _selectedState.SetValueAsync(newValue);
        }
    }
}
