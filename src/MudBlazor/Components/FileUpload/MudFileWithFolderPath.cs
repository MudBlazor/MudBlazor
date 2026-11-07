namespace MudBlazor;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// A file from a folder selection, paired with its path relative to the selected folder.
/// </summary>
public class MudFileWithFolderPath
{
    /// <summary>
    /// The selected file.
    /// </summary>
    public IBrowserFile File { get; set; }

    /// <summary>
    /// The file's path relative to the selected folder, including the folder name, such as <c>Photos/2024/beach.jpg</c>.
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MudFileWithFolderPath"/> class.
    /// </summary>
    /// <param name="file">The selected file.</param>
    /// <param name="relativePath">The file's path relative to the selected folder.</param>
    public MudFileWithFolderPath(IBrowserFile file, string relativePath)
    {
        File = file;
        RelativePath = relativePath;
    }
}
