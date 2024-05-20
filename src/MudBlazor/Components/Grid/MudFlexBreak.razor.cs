// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudFlexBreak : MudComponentBase
{
    /// <summary>
    /// Class names separated by spaces.
    /// </summary>
    protected string Classname =>
        new CssBuilder("mud-flex-break")
            .AddClass(Class)
            .Build();
}
