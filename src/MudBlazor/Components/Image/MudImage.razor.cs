// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

public partial class MudImage : MudComponentBase
{
    /// <summary>
    /// Specifies the path to the image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string Src { get; set; }
    
    /// <summary>
    /// Specifies an alternate text for the image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string Alt { get; set; }
    
    /// <summary>
    /// Specifies the height of the image in px.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Image.Appearance)]
    public string Height { get; set; }

    /// <summary>
    /// Specifies the width of the image in px.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Image.Appearance)]
    public string Width { get; set; }
}
