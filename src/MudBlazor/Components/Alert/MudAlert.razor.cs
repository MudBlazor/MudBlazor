using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAlert : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-alert")
          .AddClass($"mud-alert-{Variant.ToDescriptionString()}-{Severity.ToDescriptionString()}")
          .AddClass($"mud-dense", Dense)
          .AddClass($"mud-square", Square)
          .AddClass($"mud-elevation-{Elevation.ToString()}")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
        /// </summary>
        [Parameter] public int Elevation { set; get; } = 0;

        /// <summary>
        /// If true, rounded corners are disabled.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, compact padding will be used.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, no alert icon will be used.
        /// </summary>
        [Parameter] public bool NoIcon { get; set; }

        /// <summary>
        /// The severity of the alert. This defines the color and icon used.
        /// </summary>
        [Parameter] public Severity Severity { get; set; } = Severity.Normal;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom icon, leave unset to use the standard icon which depends on the Severity
        /// </summary>
        [Parameter] public string Icon { get; set; }

        protected string _icon;

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
                    Severity.Normal => Outlined.EventNoteOutlined,
                    Severity.Info => Outlined.InfoOutlined,
                    Severity.Success => Icons.Custom.AlertSuccess,
                    Severity.Warning => Outlined.ReportProblemOutlined,
                    Severity.Error => Filled.ErrorOutline
                };
            }
            }

        /// <summary>
        /// Raised when the alert is clicked
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        private void OnClicked(MouseEventArgs ev)
        {
            OnClick.InvokeAsync(ev);
        }
    }
}
   

