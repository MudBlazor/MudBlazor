// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor;

/// <summary>
/// A file from a folder selection, paired with its path relative to the selected folder.
/// </summary>
public sealed class MudFileWithFolderPath
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="file">The selected file.</param>
    /// <param name="relativePath">The file's path relative to the selected folder.</param>
    public MudFileWithFolderPath(IBrowserFile file, string relativePath)
    {
        File = file;
        RelativePath = relativePath;
    }

    /// <summary>
    /// The selected file.
    /// </summary>
    public IBrowserFile File { get; }

    /// <summary>
    /// The file's path relative to the selected folder, including the folder name, such as <c>Photos/2024/beach.jpg</c>.
    /// </summary>
    /// <remarks>
    /// Empty when the browser didn't provide the path.
    /// </remarks>
    public string RelativePath { get; }
}
