using System.ComponentModel;

namespace MudBlazor.Docs.Services;

public enum NavigationSection
{
    [Description("unspecified")]
    Unspecified = 0,
    [Description("api")]
    Api,
    [Description("components")]
    Components,
    [Description("features")]
    Features,
    [Description("customization")]
    Customization,
    [Description("utilities")]
    Utilities
}
