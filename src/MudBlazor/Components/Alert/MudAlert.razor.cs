using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Represents an alert used to display an important message which is statically embedded in the page content.
    /// </summary>
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
        /// <param name="contentAlignment">
        /// A <see cref="HorizontalAlignment"/> value.  The alignment to adjust.
        /// </param>
        /// <returns>
        /// A <see cref="HorizontalAlignment"/> value.  The adjusted alignment.
        /// </returns>
        private HorizontalAlignment ConvertHorizontalAlignment(HorizontalAlignment contentAlignment)
        {
            return contentAlignment switch
            {
                HorizontalAlignment.Right => RightToLeft ? HorizontalAlignment.Start : HorizontalAlignment.End,
                HorizontalAlignment.Left => RightToLeft ? HorizontalAlignment.End : HorizontalAlignment.Start,
                _ => contentAlignment
            };
        }

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Gets or sets the position of the text to the start (Left in LTR and right in RTL).
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="HorizontalAlignment.Left"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public HorizontalAlignment ContentAlignment { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Occurs when the close button has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MudAlert> CloseIconClicked { get; set; }

        /// <summary>
        /// Gets or sets the icon used for the close button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Close"/>. This icon is only displayed when the <see cref="ShowCloseIcon"/> property is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// Gets or sets whether a close icon is displayed.
        /// </summary>
        /// <remarks>
        /// To customize which icon is displayed for the close icon, set the <see cref="CloseIcon"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public bool ShowCloseIcon { get; set; }

        /// <summary>
        /// Gets or sets the size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// Gets or sets whether rounded corners are disabled.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// Gets or sets whether compact padding will be used.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Gets or sets whether no icon is displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  To customize the icon, use the <see cref="Icon"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool NoIcon { get; set; }

        /// <summary>
        /// Gets or sets the severity of the alert.
        /// </summary>
        /// <remarks>
        /// The severity determines the color and icon used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public Severity Severity { get; set; } = Severity.Normal;

        /// <summary>
        /// Gets or sets the display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text" />. The variant changes the appearance of the alert, such as <c>Text</c>, <c>Outlined</c>, or <c>Filled</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Gets or sets the content within the alert.
        /// </summary>
        /// <remarks>
        /// This property allows for custom content to displayed inside of the alert, but it is not required.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the icon displayed for this alert.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, the custom icon will be displayed.  Otherwise, the icon will depend on the <see cref="Severity"/> property.
        /// </remarks>
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
            if (CloseIconClicked.HasDelegate)
            {
                await CloseIconClicked.InvokeAsync(this);
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage] //If we can check this exception can include the coverage again
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

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
        /// <param name="mouseEventArgs">
        /// A <see cref="MouseEventArgs"/> object.  The mouse coordinates related to this click.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> object.
        /// </returns>
        internal async Task OnClickHandler(MouseEventArgs mouseEventArgs)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(mouseEventArgs);
            }
        }

        /// <summary>
        /// Occurs when the alert has been clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
    }
}
