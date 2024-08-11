// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ErrorEventArgs = Microsoft.AspNetCore.Components.Web.ErrorEventArgs;

namespace MudBlazor
{
    [JsonSerializable(typeof(EventArgs))]
    [JsonSerializable(typeof(ChangeEventArgs))]
    [JsonSerializable(typeof(ClipboardEventArgs))]
    [JsonSerializable(typeof(DragEventArgs))]
    [JsonSerializable(typeof(ErrorEventArgs))]
    [JsonSerializable(typeof(FocusEventArgs))]
    [JsonSerializable(typeof(KeyboardEventArgs))]
    [JsonSerializable(typeof(MouseEventArgs))]
    [JsonSerializable(typeof(PointerEventArgs))]
    [JsonSerializable(typeof(ProgressEventArgs))]
    [JsonSerializable(typeof(TouchEventArgs))]
    [JsonSerializable(typeof(WheelEventArgs))]
    internal sealed partial class WebEventJsonContext : JsonSerializerContext
    {
    }
}
