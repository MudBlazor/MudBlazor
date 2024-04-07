// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using BytexDigital.Blazor.Components.CookieConsent;

namespace MudBlazor.Docs.Pages.Consent.Modal;

#nullable enable
public class MudCookieConsentDefaultSettingsModalVariant : CookieConsentPreferencesVariantBase
{
    public override Type ComponentType { get; set; } = typeof(MudCookieConsentSettingsModal);
}
