using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using static System.Net.WebRequestMethods;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents an alert used to display an important message which is statically embedded in the page content.
/// </summary>
/// <remarks><para>Alerts are useful for calling attention to the user in regards to warnings, errors,
/// informational messages, and more.  Alerts typically use the <see cref="Severity"/> property to control
/// the color and icon displayed, but a custom <see cref="Icon"/> and <see cref="CloseIcon"/> can be used, 
/// or no icon at all via the <see cref="NoIcon"/> and <see cref="ShowCloseIcon"/> properties.  You can 
/// also use the <see cref="Dense"/> property to take up less space, and <see cref="HorizontalAlignment"/>
/// to control the horizontal alignment of text.  The <see cref="Variant"/> property can be used to show
/// outlined, filled, or textual alerts.  The <see cref="Elevation"/> property can be used to make the alert
/// stand out with a drop shadow.</para>
/// <para>To notify the user with dynamic alerts which overlay the page, check out the 
/// <see href="https://mudblazor.com/components/snackbar"/> component.</para>
/// </remarks>
public partial class MudAlert : MudComponentBase
{
    protected string Classname => new CssBuilder("mud-alert")
      .AddClass($"mud-alert-{Variant.ToDescriptionString()}-{Severity.ToDescriptionString()}")
      .AddClass($"mud-dense", Dense)
      .AddClass($"mud-square", Square)
      .AddClass($"mud-elevation-{Elevation}")
      .AddClass(Class)
      .Build();

    protected string ClassPosition => new CssBuilder("mud-alert-position")
      .AddClass($"justify-sm-{ConvertHorizontalAlignment(ContentAlignment).ToDescriptionString()}")
      .Build();

    /// <summary>
    /// Gets the horizontal alignment to use based on the current right-to-left setting.
    /// </summary>
    /// <param name="contentAlignment">A <see cref="HorizontalAlignment"/> value.  The alignment to adjust.</param>
    /// <returns>A <see cref="HorizontalAlignment"/> value.  The adjusted alignment.</returns>
    private HorizontalAlignment ConvertHorizontalAlignment(HorizontalAlignment contentAlignment)
    {
        return contentAlignment switch
        {
            HorizontalAlignment.Right => RightToLeft ? HorizontalAlignment.Start : HorizontalAlignment.End,
            HorizontalAlignment.Left => RightToLeft ? HorizontalAlignment.End : HorizontalAlignment.Start,
            _ => contentAlignment
        };
    }

    /// <summary>
    /// Gets or sets a value indicating whether Right-to-Left (RTL) mode is enabled.
    /// </summary>
    /// <remarks>Defaults to false. When true, text will be displayed right-to-left.</remarks>
    [CascadingParameter(Name = "RightToLeft")]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// Gets or sets the position of the text to the start (Left in LTR and right in RTL).
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public HorizontalAlignment ContentAlignment { get; set; } = HorizontalAlignment.Left;

    /// <summary>
    /// Gets or sets the callback for when the close button has been clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MudAlert> CloseIconClicked { get; set; }

    /// <summary>
    /// Gets or sets the icon used for the close button.
    /// </summary>
    /// <remarks>Defaults to <see cref="Icons.Material.Filled.Close"/>. This icon is only 
    /// displayed when the <see cref="ShowCloseIcon"/> property is true.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

    /// <summary>
    /// Gets or sets a value indicating whether a close icon is displayed.
    /// </summary>
    /// <remarks>To customize which icon is displayed for the close icon, set the 
    /// <see cref="CloseIcon"/> property.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Behavior)]
    public bool ShowCloseIcon { get; set; }

    /// <summary>
    /// Gets or sets the size of the drop shadow.
    /// </summary>
    /// <remarks>Defaults to 0.  A higher number creates a heavier drop shadow.  Use a value 
    /// of 0 for no shadow.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public int Elevation { set; get; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether rounded corners are disabled.
    /// </summary>
    /// <remarks>Defaults to false.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public bool Square { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether compact padding will be used.
    /// </summary>
    /// <remarks>Defaults to false.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public bool Dense { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether no icon is displayed.
    /// </summary>
    /// <remarks>Defaults to false.  To customize the icon, use the <see cref="Icon"/> property.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public bool NoIcon { get; set; }

    /// <summary>
    /// Gets or sets the severity of the alert.
    /// </summary>
    /// <remarks>The severity determines the color and icon used.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Behavior)]
    public Severity Severity { get; set; } = Severity.Normal;

    /// <summary>
    /// Gets or sets the display variant to use.
    /// </summary>
    /// <remarks>Defaults to Variant.Text. The variant changes the appearance of the alert, such as Text, Outlined, or Filled.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public Variant Variant { get; set; } = Variant.Text;

    /// <summary>
    /// Gets or sets the content within the alert.
    /// </summary>
    /// <remarks>This property allows for custom content to displayed inside of the alert, but it is not required.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the icon displayed for this alert.
    /// </summary>
    /// <remarks>Defaults to null.  When set, the custom icon will be displayed.  Otherwise, the icon will depend on the <see cref="Severity"/> property.</remarks>
    [Parameter]
    [Category(CategoryTypes.Alert.Appearance)]
    public string? Icon { get; set; }

    protected string? _icon;

    /// <summary>
    /// Occurs when the close icon has been clicked.
    /// </summary>
    /// <returns>A <see cref="Task"/> object.</returns>
    internal async Task OnCloseIconClickAsync()
    {
        await CloseIconClicked.InvokeAsync(this);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage] //If we can check this exception can include the coverage again
    protected override void OnParametersSet()
    {
        if (!string.IsNullOrEmpty(Icon))
        {
            _icon = Icon;
        }
        else
        {
            _icon = Severity switch
            {
                Severity.Normal => Icons.Material.Outlined.EventNote,
                Severity.Info => Icons.Material.Outlined.Info,
                Severity.Success => Icons.Custom.Uncategorized.AlertSuccess,
                Severity.Warning => Icons.Material.Outlined.ReportProblem,
                Severity.Error => Icons.Material.Filled.ErrorOutline,
                _ => throw new ArgumentOutOfRangeException(nameof(Severity)),
            };
        }
    }

    /// <summary>
    /// Occurs when the alert has been clicked.
    /// </summary>
    /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> object.  The mouse coordinates related to this click.</param>
    /// <returns>A <see cref="Task"/> object.</returns>
    internal async Task OnClickHandler(MouseEventArgs mouseEventArgs)
    {
        await OnClick.InvokeAsync(mouseEventArgs);
    }

    /// <summary>
    /// Occurs when the alert has been clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}
