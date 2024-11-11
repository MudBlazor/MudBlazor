// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Pages.Api;

/// <summary>
/// A page which displays all <see cref="MudGlobal"/> properties.
/// </summary>
public partial class Globals
{
    /// <summary>
    /// The types which reference <see cref="MudGlobal"/> settings.
    /// </summary>
    private List<DocumentedType> TypesWithGlobals { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        // Find the types which have global settings
        TypesWithGlobals = [.. ApiDocumentation.Types.Where(pair => pair.Value.GlobalSettings.Count > 0).Select(pair => pair.Value).OrderBy(type => type.NameFriendly)];
        base.OnInitialized();
    }
}
