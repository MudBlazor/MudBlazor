// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Abstractions;

#nullable enable
public interface IClassProcessor
{
    /// <summary>
    /// Processes the specified class name and returns a result based on the input.
    /// </summary>
    /// <param name="classname">The name of the class to process. Can be <see langword="null"/> or empty.</param>
    /// <returns>A processed string result based on the input class name, or <see langword="null"/> if the input is <see
    /// langword="null"/>.</returns>
    string? Process(string? classname);
}

