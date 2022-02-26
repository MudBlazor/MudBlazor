using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Charts;
using MudBlazor.Charts.SVG.Models;
using MudBlazor.Components.Chart;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract class MudChartBase : MudComponentBase
    {
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public double[] InputData { get; set; } = Array.Empty<double>();

        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] InputLabels { get; set; } = Array.Empty<string>();

        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] XAxisLabels { get; set; } = Array.Empty<string>();

        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<ChartSeries> ChartSeries { get; set; } = new();

        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public ChartOptions ChartOptions { get; set; } = new();

        /// <summary>
        /// RenderFragment for customization inside the chart's svg.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public RenderFragment CustomGraphics { get; set; }
        
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public RenderFragment<(string Label, double Value)> TooltipTemplate { get; set; }
        
        private readonly RenderFragment<(string Label, double Value)> _defaultToolTip = args => builder =>
        {
            builder.OpenComponent<MudChartTooltipContent>(0);
            builder.AddAttribute(1, nameof(MudChartTooltipContent.Label), args.Label);
            builder.AddAttribute(2, nameof(MudChartTooltipContent.Value), ToS(args.Value));

            builder.CloseComponent();
        };
        
        public bool TooltipVisible { get; private set; }

        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public bool ToolTipEnabled { get; set; } = true;

        public int HoverIndex { get; private set; }
        
        private int _tooltipX;
        private int _tooltipY;
        private int _lastHoverRenderIndex = -1;

        /// <summary>
        /// This methods is called when the mouse is over the chart item, and renders the tooltip.
        /// </summary>
        /// <returns></returns>
        private RenderFragment RenderTooltip()
        {
            var data = InputData[HoverIndex];
            var label = InputLabels[HoverIndex];

            //Default tooltip
            return builder =>
            {
                builder.OpenComponent<MudChartTooltip>(0);
                builder.AddAttribute(1, nameof(MudChartTooltip.X), _tooltipX);
                builder.AddAttribute(2, nameof(MudChartTooltip.Y), _tooltipY);
                builder.AddAttribute(3, nameof(MudChartTooltip.ChildContent), TooltipTemplate != null ? TooltipTemplate((label, data)) : _defaultToolTip((label, data)));

                builder.CloseComponent();
            };
        }

        protected RenderFragment _tooltip;

        protected void OnMouseHover(MouseEventArgs args, SvgPath svgPath)
        {
            //Set the index of the item that is hovered
            HoverIndex = svgPath.Index;
            //Set the tooltip position and make it visible
            TooltipVisible = true;
            //This is only needed for the tooltip not derp around on the first render
            _tooltipX = (int)args.ClientX;
            _tooltipY = (int)args.ClientY;

            //Only render the tooltip if the index has changed
            if (_lastHoverRenderIndex == HoverIndex)
            {
                //return;
            }

            //Updates the last rendered index
            _lastHoverRenderIndex = HoverIndex;
            //Render the tooltip
            _tooltip = RenderTooltip();
        }
        
        protected void OnMouseOut(MouseEventArgs args, SvgPath svgPath)
        {
            //Hide the tooltip
            TooltipVisible = false;
        }
        
        protected void OnMouseMove(MouseEventArgs args, SvgPath svgPath)
        {
            var x = (int)args.ClientX;
            var y = (int)args.ClientY;
            
            //So we dont render anything new if the position is the same
            if(_tooltipX == x && _tooltipY == y)
            {
                return;
            }
            
            _tooltipX = x;
            _tooltipY = y;
        }

        protected string Classname =>
        new CssBuilder("mud-chart")
           .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [CascadingParameter]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The Type of the chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public ChartType ChartType { get; set; }

        /// <summary>
        /// The Width of the chart, end with % or px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Width { get; set; } = "80%";

        /// <summary>
        /// The Height of the chart, end with % or px.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public string Height { get; set; } = "80%";

        /// <summary>
        /// The placement direction of the legend if used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public Position LegendPosition { get; set; } = Position.Bottom;

        private Position ConvertLegendPosition(Position position)
        {
            return position switch
            {
                Position.Start => RightToLeft ? Position.Right : Position.Left,
                Position.End => RightToLeft ? Position.Left : Position.Right,
                _ => position
            };
        }

        private int _selectedIndex;

        /// <summary>
        /// Selected index of a portion of the chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    SelectedIndexChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// Selected index of a portion of the chart.
        /// </summary>
        [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }

        /// <summary>
        /// Scales the input data to the range between 0 and 1
        /// </summary>
        protected double[] GetNormalizedData()
        {
            if (InputData == null)
                return Array.Empty<double>();
            var total = InputData.Sum();
            return InputData.Select(x => Math.Abs(x) / total).ToArray();
        }

        protected static string ToS(double d, string format = null)
        {
            if (string.IsNullOrEmpty(format))
                return d.ToString(CultureInfo.InvariantCulture);

            return d.ToString(format);
        }

    }

    public enum ChartType
    {
        Donut,
        Line,
        Pie,
        Bar
    }
}
