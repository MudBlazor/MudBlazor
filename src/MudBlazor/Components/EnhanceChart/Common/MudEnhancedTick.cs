using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    record TickSnapShot(MudColor Color, Double Thickness, Double Value, String LineCssClass, TickMode Mode);

    public record TickInfo(Boolean ShowGridLines, Double GridLineThickness, MudColor GridLineColor, String GridLineCssClass) : ITick;

    [DoNotGenerateAutomaticTest]
    public class MudEnhancedTick : ComponentBase, ISnapshot<TickSnapShot>, IDisposable
    {
        [CascadingParameter] public IYAxis Axis { get; set; }
        [CascadingParameter(Name = "IsMajorTick")] public Boolean IsMajorTick { get; set; }

        [Parameter] public MudColor Color { get; set; } = "#808080";
        [Parameter] public Double Thickness { get; set; } = 0.5;
        [Parameter] public String LineCssClass { get; set; } = String.Empty;
        [Parameter] public Double Value { get; set; }
        [Parameter] public TickMode Mode { get; set; } = TickMode.Absolute;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //if (Axis == null)
            //{
            //    throw new ArgumentException("a tick needs to be placed inside an axe");
            //}

            if (Axis == null)
            {
                return;
            }

            ISnapshot<TickSnapShot> _this = this;

            if (_this.SnapshotHasChanged(true))
            {
                Axis?.TickUpdated(this);
            }
        }

        TickSnapShot ISnapshot<TickSnapShot>.OldSnapshotValue { get; set; }
        TickSnapShot ISnapshot<TickSnapShot>.CreateSnapShot() => new TickSnapShot((String)Color, Thickness, Value, LineCssClass, Mode);

        internal ITick GetTickInfo(bool showMajorTicks) => new TickInfo(showMajorTicks, Thickness, (String)Color, LineCssClass);

        public void Dispose()
        {
            Axis?.RemoveTick(IsMajorTick);
        }
    }
}
