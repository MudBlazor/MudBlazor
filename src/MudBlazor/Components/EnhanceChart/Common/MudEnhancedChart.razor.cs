using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedChart : MudComponentBase
    {
        private MudEnhancedChartLegendBase _legend;
        private ChartLegendInfo _legendInfo;
        private List<ChartToolTipInfo> _toolTipInfo = new();

        /// <summary>
        /// Setting the Chart that should be rendered inside this
        /// </summary>
        [Parameter] public RenderFragment Chart { get; set; }

        #region Legend

        /// <summary>
        /// If this value is true, a legend will be displayed. The placement can be controlled via LegendPosition  and LegendAlignment. Default value is true
        /// </summary>
        [Parameter] public Boolean ShowLegend { get; set; } = true;

        /// <summary>
        /// Set the value regarding where the legend should be place. This value has only an effect if ShowLegend is set to true. Default is Position.Right
        /// </summary>
        [Parameter] public Position LegendPosition { get; set; } = Position.Right;

        /// <summary>
        /// Set the value regarding where the legnend should be align. This value has only an effect if ShowLegend is set to true. This applies to horizontal and vertical variants. Default is Align.Center
        /// </summary>
        [Parameter] public Align LegendAlignment { get; set; } = Align.Center;

        /// <summary>
        /// The template used for rendering the legend. Only has an effect if ShowLegend is set to true.
        /// </summary>
        [Parameter] public RenderFragment<ChartLegendInfo> Legend { get; set; }

        #endregion

        #region Title

        /// <summary>
        /// If this value is true, a a title  will be displayed. The placement can be controlled via TitlePosition and TitleAlignment. The content can be controlled via the Ttile or TitleDrawer property. Default value is true.
        /// </summary>
        [Parameter] public Boolean ShowTitle { get; set; } = true;

        /// <summary>
        /// The position of the title of the chart. This value has only an effect if ShowTitle is set to true.  Default value is Position.Top
        /// </summary>
        [Parameter] public Position TitlePosition { get; set; } = Position.Top;

        /// <summary>
        /// Set the value regarding where the legnend should be align. This value has only an effect if ShowTitle is set to true. This applies to horizontal and vertical variants. Default is TitleAlignment.Center
        /// </summary>
        [Parameter] public Align TitleAlignment { get; set; } = Align.Center;

        /// <summary>
        /// The title of the chart. It is displayed as simple heading. For more complex content set the value of TitleDrawer
        /// </summary>
        [Parameter] public String Title { get; set; } = String.Empty;

        /// <summary>
        /// The template used for rendering the title. Change the value if you want more than just a headline. This value has only an effect if ShowTitle is set to true.
        /// </summary>
        [Parameter] public RenderFragment<String> TitleDrawer { get; set; } = DefaultTitleDrawerFragment;

        /// <summary>
        /// Template used to draw a the tooltip. You can set it to one of the predefined templates or create your own 
        /// </summary>
        [Parameter] public RenderFragment<IEnumerable<ChartToolTipInfo>> ToolTip { get; set; }

        public void UpdateLegend(ChartLegendInfo info)
        {
            _legendInfo = info;
            _legend?.Update(info);
        }

        public async void UpdateTooltip(IEnumerable<ChartToolTipInfo> info)
        {
            _toolTipInfo = new (info);
            await InvokeAsync(StateHasChanged);
        }

        protected internal void SetLegend(MudEnhancedChartLegendBase legend) => _legend = legend;

        private static RenderFragment DefaultTitleDrawerFragment(String context)
        {
            return builder =>
            {
                builder.OpenComponent(0, typeof(MudText));
                builder.AddAttribute(1, nameof(MudText.Typo), Typo.h4);
                builder.AddAttribute(2, nameof(MudText.ChildContent), (RenderFragment)((builder2) => {
                    builder2.AddContent(3, context);
                }));
                builder.CloseElement();
            };
        }

        #endregion

        #region View helper

        protected string Classname =>
        new CssBuilder("mud-enhanced-chart")
          .AddClass(Class)
          .AddClass("d-flex")
          .AddClass(GetFlexDirectionForChartContainer())
        .Build();

        protected String GetFlexDirectionForChartContainer() => TitlePosition switch
        {
            Position.Bottom or Position.Top => "flex-column",
            Position.Left or Position.Right => "flex-row",
            _ => String.Empty,
        };

        protected String GetFlexOrderForTitleContainer() => TitlePosition switch
        {
            Position.Bottom or Position.Right => "order-last",
            Position.Left or Position.Top => "order-first",
            _ => String.Empty,
        };

        protected String GetFlexDirectionForDrawerContainer() => LegendPosition switch
        {
            Position.Bottom or Position.Top => "flex-column",
            Position.Left or Position.Right => "flex-row",
            _ => String.Empty,
        };

        protected String GetFlexOrderForLegendContainer() => LegendPosition switch
        {
            Position.Bottom or Position.Right => "order-last",
            Position.Left or Position.Top => "order-first",
            _ => String.Empty,
        };

        protected virtual String GetAlignClassBasendOnPosition(Align align, Position position) => (align, position) switch
        {
            ((Align.Center or Align.Justify), (Position.Bottom or Position.Top)) => "justify-center",
            ((Align.Center or Align.Justify), (Position.Left or Position.Right)) => "align-center",

            (Align.Left, (Position.Bottom or Position.Top)) => "justify-start",
            (Align.Left, (Position.Left or Position.Right)) => "align-start",

            (Align.Right, (Position.Bottom or Position.Top)) => "justify-end",
            (Align.Right, (Position.Left or Position.Right)) => "align-end",

            _ => String.Empty,
        };

        protected virtual String GetAlignClassForLegendContainer() => GetAlignClassBasendOnPosition(LegendAlignment, LegendPosition);
        protected virtual String GetAlignClassForTitleContainer() => GetAlignClassBasendOnPosition(TitleAlignment, TitlePosition);

        #endregion
    }
}
