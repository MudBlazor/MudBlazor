using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A clickable link which can navigate to a URL.
/// </summary>
public partial class MudLink : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-typography mud-link")
            .AddClass($"mud-{Color.ToDescriptionString()}-text")
            .AddClass($"mud-link-underline-{Underline.ToDescriptionString()}")
            .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
            // When Href is empty, link's hover cursor is text "I beam" even when OnClick has a delegate.
            // To change this for more expected look change hover cursor to a pointer:
            .AddClass("cursor-pointer", Href == default && OnClick.HasDelegate && !Disabled)
            .AddClass("mud-link-disabled", Disabled)
            .AddClass(Class)
            .Build();

    private Dictionary<string, object?> Attributes
    {
        get
        {
            var attributes = new Dictionary<string, object?>();

            if (Disabled)
            {
                attributes["aria-disabled"] = "true";
            }
            else
            {
                attributes["href"] = Href;
                attributes["target"] = Target;
            }

            if (OnClick.HasDelegate)
            {
                attributes["role"] = "button";
            }

            // Apply user attributes last so they take precedence.
            foreach (var attribute in UserAttributes)
            {
                attributes[attribute.Key] = attribute.Value;
            }

            return attributes;
        }
    }

    /// <summary>
    /// The color of the link.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Primary"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Appearance)]
    public Color Color { get; set; } = MudGlobal.LinkDefaults.Color;

    /// <summary>
    /// The typography variant to use.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Typo.body1"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Appearance)]
    public Typo Typo { get; set; } = MudGlobal.LinkDefaults.Typo;

    /// <summary>
    /// Applies an underline to the link.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Underline.Hover"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Appearance)]
    public Underline Underline { get; set; } = MudGlobal.LinkDefaults.Underline;

    /// <summary>
    /// The URL to navigate to upon click.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Behavior)]
    public string? Href { get; set; }

    /// <summary>
    /// The browser frame to open this link when <see cref="Href"/> is specified.
    /// </summary>
    /// <remarks>
    /// Possible values include <c>_blank</c>, <c>_self</c>, <c>_parent</c>, <c>_top</c>, or a <i>frame name</i>. <br/>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Behavior)]
    public string? Target { get; set; }

    /// <summary>
    /// Prevents user interaction with this link.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Link.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// The content within this component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Link.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Occurs when this link has been clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected async Task OnClickHandler(MouseEventArgs ev)
    {
        if (Disabled)
        {
            return;
        }

        await OnClick.InvokeAsync(ev);
    }
}
