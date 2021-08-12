
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public record BarChartXAxisSnapshot(Boolean ShowGridLines, XAxisPlacement Placement, String LabelCssClass, Double Height, Double Margin, String GridLineCssClass, Double GridLineThickness, String GridLineColor);

    [DoNotGenerateAutomaticTest]
    public class MudEnhancedBarChartXAxis : ComponentBase, ISnapshot<BarChartXAxisSnapshot>
    {
        /// <summary>
        /// the parent bar chart
        /// </summary>
        [CascadingParameter] public MudEnhancedBarChart Chart { get; set; }

        /// <summary>
        /// Display a line after each label. Default is true
        /// </summary>
        [Parameter] public Boolean ShowGridLines { get; set; } = true;

        /// <summary>
        /// Set the placement of the x axis. Set the Placement to XAxisPlacement.None to let the axis disappear. Default is XAxisPlacement.Bottom
        /// </summary>
        [Parameter] public XAxisPlacement Placement { get; set; } = XAxisPlacement.Bottom;

        /// <summary>
        /// Set an addtional class that is applied to each label. This value is added to the class attribute, so you can add multiple css classes here like "class1 class2"
        /// </summary>
        [Parameter] public String LabelCssClass { get; set; } = String.Empty;

        /// <summary>
        /// The labels of the axis. Can be any value and should have the same amount as the datapoints
        /// </summary>
        [Parameter] public IList<String> Labels { get; set; } = new List<String>();

        /// <summary>
        /// The heigth of the axis. The value is the relative size. A value of  5.0 would indicate that the the axis will take 5% of the available space
        /// </summary>
        [Parameter] public Double Height { get; set; } = 5.0;

        /// <summary>
        /// The margin between the labels and the axis in the chart
        /// </summary>
        [Parameter] public Double Margin { get; set; } = 3.0;

        /// <summary>
        /// Set an addtional class that is applied to gridline label. This value is added to the class attribute, so you can add multiple css classes here like "class1 class2"
        /// </summary>
        [Parameter] public String GridLineCssClass { get; set; } = String.Empty;

        /// <summary>
        /// Set the thickness of the grid lines. This property has only an effect if ShowGridLines is set to true. The value is the relative size of the line. A value of 0.5 indicating that each line should take 0.5 per cent of the availabe space
        /// </summary>
        [Parameter] public Double GridLineThickness { get; set; } = 0.5;

        /// <summary>
        /// The color that is applied to the a gridline. This property has only an effect if ShowGridLines is set to true.
        /// </summary>
        [Parameter] public MudColor GridLineColor { get; set; } = "#808080";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //if (Chart == null)
            //{
            //    throw new ArgumentException("Axes needs to be placed inside a Chart");
            //}

            if(Chart == null)
            {
                return;
            }

            ISnapshot<BarChartXAxisSnapshot> _this = this;

            if (_this.SnapshotHasChanged(true) == true)
            {
                Chart.XAxesUpdated(this);
            }
        }

        BarChartXAxisSnapshot ISnapshot<BarChartXAxisSnapshot>.OldSnapshotValue { get; set; }
        BarChartXAxisSnapshot ISnapshot<BarChartXAxisSnapshot>.CreateSnapShot() => new BarChartXAxisSnapshot(ShowGridLines, Placement, LabelCssClass, Height, Margin, GridLineCssClass, GridLineThickness, (String)GridLineColor);
    }
}
