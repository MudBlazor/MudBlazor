namespace MudBlazor;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
/// Represents a validated file payload paired with its browser-reported relative directory path structure.
/// </summary>
public class MudFileWithFolderPath
{
    /// <summary>
    /// Gets the native browser file information metadata.
    /// </summary>
    public IBrowserFile File { get; set; }

    /// <summary>
    /// Gets the relative filesystem directory structure hierarchy path extracted from webkitRelativePath.
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MudFileWithFolderPath"/> class.
    /// </summary>
    /// <param name="file">The uploaded file instance mapping target.</param>
    /// <param name="relativePath">The webkit relative directory string metadata path structural array.</param>
    public MudFileWithFolderPath(IBrowserFile file, string relativePath)
    {
        File = file;
        RelativePath = relativePath;
    }
}
