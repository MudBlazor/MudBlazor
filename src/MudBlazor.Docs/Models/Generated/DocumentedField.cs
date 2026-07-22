// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents a documented field.
/// </summary>
[DebuggerDisplay("({TypeName}) {Name}: {Summary}")]
public sealed class DocumentedField : DocumentedMember
{
}
