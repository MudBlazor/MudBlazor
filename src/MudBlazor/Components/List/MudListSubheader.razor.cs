using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A header displayed as part of a <see cref="MudList{T}"/>.
/// </summary>
/// <remarks>
/// Typically used to describe a list.
/// </remarks>
/// <seealso cref="MudList{T}"/>
/// <seealso cref="MudListItem{T}"/>
public partial class MudListSubheader : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-list-subheader")
            .AddClass("mud-list-subheader-gutters", Gutters)
            .AddClass("mud-list-subheader-inset", Inset)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The content within this header.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Applies left and right padding to all list items.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool Gutters { get; set; } = true;

    /// <summary>
    /// Applies an indent to this header.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.List.Appearance)]
    public bool Inset { get; set; }
}
