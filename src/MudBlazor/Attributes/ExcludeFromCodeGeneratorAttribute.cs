// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

/// <summary>
/// Attribute to exclude enums from source code generator.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
internal sealed class ExcludeFromCodeGeneratorAttribute: Attribute
{
    
}
