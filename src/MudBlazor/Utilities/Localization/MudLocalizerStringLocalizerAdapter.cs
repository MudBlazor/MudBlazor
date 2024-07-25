// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace MudBlazor.Utilities.Localization;

#nullable enable
/// <summary>
/// Adapter class that wraps an <see cref="InternalMudLocalizer"/> to provide an implementation of <see cref="IStringLocalizer"/>.
/// </summary>
internal sealed class MudLocalizerStringLocalizerAdapter : IStringLocalizer
{
    private readonly InternalMudLocalizer _internalMudLocalizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="MudLocalizerStringLocalizerAdapter"/> class.
    /// </summary>
    /// <param name="internalMudLocalizer">The <see cref="InternalMudLocalizer"/> to wrap.</param>
    public MudLocalizerStringLocalizerAdapter(InternalMudLocalizer internalMudLocalizer)
    {
        _internalMudLocalizer = internalMudLocalizer;
    }

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Enumerable.Empty<LocalizedString>();

    /// <inheritdoc />
    public LocalizedString this[string name] => _internalMudLocalizer[name, []];

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments] => _internalMudLocalizer[name, arguments];
}
