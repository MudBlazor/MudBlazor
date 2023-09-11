namespace MudBlazor
{
#nullable enable
    public class BreadcrumbItem
    {
        public string Text { get; }

        public string? Href { get; }

        public bool Disabled { get; }

        public string? Icon { get; }

        public BreadcrumbItem(string text, string? href, bool disabled = false, string? icon = null)
        {
            Text = text;
            Disabled = disabled;
            Href = href;
            Icon = icon;
        }
    }
}
