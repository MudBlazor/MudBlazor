namespace MudBlazor
{
    public class BreadcrumbItem
    {
        public string Text { get; }
        public string Href { get; }
        public bool Disabled { get; }

        public BreadcrumbItem(string text, string href, bool disabled = false)
        {
            Text = text;
            Disabled = disabled;
            Href = href;
        }
    }
}
