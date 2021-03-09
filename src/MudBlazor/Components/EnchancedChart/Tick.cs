using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public enum TickMode
    {
        Absolut,
        Relative,
    }

    record TickSnapShot(String Color, Double Thickness, Double Value, TickMode Mode);

    [DoNotGenerateAutomaticTest]
    public class Tick : ComponentBase, ISnapshot<TickSnapShot>
    {
        [CascadingParameter] public IYAxis Axe { get; set; }
        [CascadingParameter(Name = "IsMajorTick")] public Boolean IsMajorTick { get; set; }

        [Parameter] public String Color { get; set; }
        [Parameter] public Double Thickness { get; set; } = 1.0;
        [Parameter] public Double Value { get; set; }
        [Parameter] public TickMode Mode { get; set; } = TickMode.Absolut;


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Axe == null)
            {
                throw new ArgumentException("a tick needs to be placed inside an axe");
            }

            ISnapshot<TickSnapShot> _this = this;

            if (_this.SnapshotHasChanged(true))
            {
                Axe.TickUpdated(this);
            }
        }

        TickSnapShot ISnapshot<TickSnapShot>.OldSnapshotValue { get; set; }
        TickSnapShot ISnapshot<TickSnapShot>.CreateSnapShot() => new TickSnapShot(Color, Thickness, Value, Mode);
    }
}
