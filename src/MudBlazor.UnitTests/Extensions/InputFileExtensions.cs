// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.UnitTests;

public static class InputFileExtensions
{
    public static async Task ClearFilesAsync(this IRenderedComponent<InputFile> inputFileComponent)
    {
        ArgumentNullException.ThrowIfNull(inputFileComponent);

        var args = new InputFileChangeEventArgs(Array.Empty<IBrowserFile>());
        await inputFileComponent.InvokeAsync(() => inputFileComponent.Instance.OnChange.InvokeAsync(args));
    }
}
