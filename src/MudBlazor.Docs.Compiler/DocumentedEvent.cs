// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.Docs.Compiler;

public class DocumentedEvent
{
    public string Key { get; set; }
    public string Name { get; set; }
    public Type Type { get; set; }
    public string Summary { get; set; }
    public string Remarks { get; set; }
}
