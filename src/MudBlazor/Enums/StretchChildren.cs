using System.ComponentModel;

namespace MudBlazor;

public enum StretchChildren
{
    [Description("none")]
    None,
    [Description("first-child")]
    FirstChild,
    [Description("last-child")]
    LastChild,
    [Description("all-children")]
    AllChildren,
}
