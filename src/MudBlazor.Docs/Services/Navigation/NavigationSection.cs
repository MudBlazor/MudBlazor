using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor.Docs.Services;

[EnumExtensions]
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
