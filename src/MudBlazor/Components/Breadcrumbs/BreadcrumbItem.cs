using System.Diagnostics;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Represents a portion of a list of breadcrumbs.
    /// </summary>
    [DebuggerDisplay("Text={Text}, Href={Href}, Disabled={Disabled}")]
    public class BreadcrumbItem
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The URL to navigate to when clicked.
        /// </summary>
        public string? Href { get; }

        /// <summary>
        /// Prevents this item from being clicked.
        /// </summary>
        public bool Disabled { get; }

        /// <summary>
        /// The custom icon for this item.
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
}
