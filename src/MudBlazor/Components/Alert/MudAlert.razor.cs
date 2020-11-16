using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudAlert : ComponentBase
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
        /// User class names, separated by space.
        /// </summary>
        [Parameter] public string Class { set; get; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public string Icon;
        public string variant;
        protected override void OnInitialized()
        {
            Icon = Severity switch
            {
                Severity.Normal => Outlined.EventNoteOutlined,
                Severity.Info => Outlined.InfoOutlined,
                Severity.Success => Icons.Custom.AlertSuccess,
                Severity.Warning => Outlined.ReportProblemOutlined,
                Severity.Error => Filled.ErrorOutline
            };
        }
    }
}
   

