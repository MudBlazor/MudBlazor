// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public enum XAxisPlacement
    {
        Bottom = 1,
        Top = 2,
        None = 3,
    }

    public record BarChartXAxisSnapshot(Boolean ShowGridLines, XAxisPlacement Placement, String LabelCssClass, Double Height, Double Margin);

    public class BarChartXAxis : ComponentBase, ISnapshot<BarChartXAxisSnapshot>
    {
        [CascadingParameter] public MudEnchancedBarChart Chart { get; set; }

        [Parameter] public Boolean ShowGridLines { get; set; } = true;
        [Parameter] public XAxisPlacement Placement { get; set; } = XAxisPlacement.Bottom;
        [Parameter] public String LabelCssClass { get; set; } = String.Empty;
        [Parameter] public IList<String> Labels { get; set; } = new List<String>();
        [Parameter] public Double Height { get; set; } = 5.0;
        [Parameter] public Double Margin { get; set; } = 3.0;


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Chart == null)
            {
                throw new ArgumentException("Axes needs to be placed inside a Chart");
            }

            ISnapshot<BarChartXAxisSnapshot> _this = this;

            if (_this.SnapshotHasChanged(true) == true)
            {
                Chart.XAxesUpdated(this);
            }
        }

        BarChartXAxisSnapshot ISnapshot<BarChartXAxisSnapshot>.OldSnapshotValue { get; set; }
        BarChartXAxisSnapshot ISnapshot<BarChartXAxisSnapshot>.CreateSnapShot() => new BarChartXAxisSnapshot(ShowGridLines, Placement, LabelCssClass, Height, Margin);
    }
}
