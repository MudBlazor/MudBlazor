using System.ComponentModel;

namespace MudBlazor
{
    public enum Typo
    {
        [Description("inherit")]
        Inherit,
        [Description("h1")]
        H1,
        [Description("h2")]
        H2,
        [Description("h3")]
        H3,
        [Description("h4")]
        H4,
        [Description("h5")]
        H5,
        [Description("h6")]
        H6,
        [Description("subtitle1")]
        Subtitle1,
        [Description("subtitle2")]
        Subtitle2,
        [Description("body1")]
        Body1,
        [Description("body2")]
        Body2,
        [Description("button")]
        Button,
        [Description("caption")]
        Caption,
        [Description("overline")]
        Overline
    }
}
