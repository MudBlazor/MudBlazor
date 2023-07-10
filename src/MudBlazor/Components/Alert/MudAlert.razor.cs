using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudAlert : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-alert")
          .AddClass($"mud-alert-{Variant.ToDescriptionString()}-{Severity.ToDescriptionString()}")
          .AddClass($"mud-dense", Dense)
          .AddClass($"mud-square", Square)
          .AddClass($"mud-elevation-{Elevation}")
          .AddClass(Class)
        .Build();

        protected string ClassPosition =>
        new CssBuilder("mud-alert-position")
            .AddClass($"justify-sm-{ConvertHorizontalAlignment(ContentAlignment).ToDescriptionString()}")
        .Build();

        private HorizontalAlignment ConvertHorizontalAlignment(HorizontalAlignment contentAlignment)
        {
            return contentAlignment switch
            {
                HorizontalAlignment.Right => RightToLeft ? HorizontalAlignment.Start : HorizontalAlignment.End,
                HorizontalAlignment.Left => RightToLeft ? HorizontalAlignment.End : HorizontalAlignment.Start,
                _ => contentAlignment
            };
        }

        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        /// <summary>
        /// Sets the position of the text to the start (Left in LTR and right in RTL).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public HorizontalAlignment ContentAlignment { get; set; } = HorizontalAlignment.Left;

        /// <summary>
        /// Sets the position of the text to the start (Left in LTR and right in RTL).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use ContentAlignment instead.", true)]
        [Parameter]
        public AlertTextPosition AlertTextPosition
        {
            get => (AlertTextPosition)ContentAlignment;
            set => ContentAlignment = (HorizontalAlignment)value;
        }

        /// <summary>
        /// The callback, when the close button has been clicked.
        /// </summary>
        [Parameter] public EventCallback<MudAlert> CloseIconClicked { get; set; }

        /// <summary>
        /// Define the icon used for the close button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// Sets if the alert shows a close icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public bool ShowCloseIcon { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// If true, rounded corners are disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, compact padding will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, no alert icon will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public bool NoIcon { get; set; }

        /// <summary>
        /// The severity of the alert. This defines the color and icon used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public Severity Severity { get; set; } = Severity.Normal;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Custom icon, leave unset to use the standard icon which depends on the Severity
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Alert.Appearance)]
        public string? Icon { get; set; }

        protected string? _icon;

        internal Task OnCloseIconClickAsync()
        {
            if (CloseIconClicked.HasDelegate)
            {
                return CloseIconClicked.InvokeAsync(this);
            }

            return Task.CompletedTask;
        }

        //If we can check this exception can include the coverage again
        [ExcludeFromCodeCoverage]
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
        /// Raised when the alert is clicked
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    }
}
