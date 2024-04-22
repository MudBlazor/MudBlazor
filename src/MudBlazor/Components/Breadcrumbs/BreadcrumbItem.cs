namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a portion of a list of breadcrumbs.
/// </summary>
public class BreadcrumbItem
{
    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets or sets the URL to navigate to when clicked.
    /// </summary>
    public string? Href { get; }

    /// <summary>
    /// Gets or sets whether this item cannot be clicked.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Gets or sets any custom icon for this item.
    /// </summary>
    public string? Icon { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="text">The text to display for this item.</param>
    /// <param name="href">The URL to navigate to when this item is clicked.</param>
    /// <param name="disabled">Whether the item cannot be clicked.</param>
    /// <param name="icon">The custom icon to display for this item.</param>
    public BreadcrumbItem(string text, string? href, bool disabled = false, string? icon = null)
    {
        Text = text;
        Disabled = disabled;
        Href = href;
        Icon = icon;
    }
}
