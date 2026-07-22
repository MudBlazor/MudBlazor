namespace MudBlazor.Docs.Models;

/// <summary>
/// A link to MudBlazor documentation.
/// </summary>
public class DocsLink
{
    /// <summary>
    /// The URL of the link.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// The name of the link.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The group this link belongs to.
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// The sort order of this link.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>.  Links are sorted by order, then by <see cref="Title"/>.
    /// </remarks>
    public int Order { get; set; }
}
