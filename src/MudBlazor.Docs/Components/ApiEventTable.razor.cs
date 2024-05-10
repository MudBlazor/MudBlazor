﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Components;

/// <summary>
/// Represents a table which displays properties for a documented type.
/// </summary>
public partial class ApiEventTable
{
    /// <summary>
    /// The type to display properties for.
    /// </summary>
    [Parameter]
    public DocumentedType Type { get; set; }
}

