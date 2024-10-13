// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace MudBlazor.UnitTests.Dummy;

[JsonSerializable(typeof(MudTheme))]
internal sealed partial class MudThemeSerializerContext : JsonSerializerContext;
