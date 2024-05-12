// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Components;

#nullable enable

/// <summary>
/// Represents the summary or remarks of an object, with linking.
/// </summary>
public partial class ApiText
{
    /// <summary>
    /// The XML documentation text to parse.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Text { get; set; } = "";

    /// <summary>
    /// The MudText and MudLink components for this text.
    /// </summary>
    //public List<DynamicComponent> TextParts { get; private set; } = [];

    protected override void OnParametersSet()
    {
        // Are there any XML "<see cref" links?
        //TextParts = (builder) => { builder.OpenComponent(0, counter.GetType()); builder.CloseComponent(); };

        base.OnParametersSet();
    }
}
