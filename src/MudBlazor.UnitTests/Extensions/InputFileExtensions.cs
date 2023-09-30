// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.UnitTests;

public static class InputFileExtensions
{
    public static void ClearFiles(this IRenderedComponent<InputFile> inputFileComponent)
    {
        if (inputFileComponent == null)
            throw new ArgumentNullException(nameof(inputFileComponent));

        var args = new InputFileChangeEventArgs(Array.Empty<IBrowserFile>());
        var uploadTask = inputFileComponent.InvokeAsync(() => inputFileComponent.Instance.OnChange.InvokeAsync(args));
        if (!uploadTask.IsCompleted)
        {
            uploadTask.GetAwaiter().GetResult();
        }
    }
}
