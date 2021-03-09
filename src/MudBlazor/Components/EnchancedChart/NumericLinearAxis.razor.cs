// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public enum ScalingType
    {
        Auto,
        Manuel,
    }

    record NumericLinearAxisSnapshot(Double Min, Double Max, Boolean ShowMinorTick, Boolean ShowMajorTick, ScalingType ScalingType);

    public partial class NumericLinearAxis : ComponentBase, IYAxis, IDisposable, ISnapshot<NumericLinearAxisSnapshot>
    {
        private Guid _id = new Guid();
        public Guid Id => _id;

        private Tick _minorTick;
        private Tick _majorTick;

        [CascadingParameter] public MudEnchancedBarChart Chart { get; set; }

        [Parameter] public RenderFragment MajorTick { get; set; } = DefaultMajorTickFragment;

        [Parameter] public RenderFragment MinorTick { get; set; } = DefaultMinorTickFragment;

        [Parameter] public Double Min { get; set; }
        [Parameter] public Double Max { get; set; }

        [Parameter] public Boolean ShowMinorTicks { get; set; } = false;
        [Parameter] public Boolean ShowMajorTicks { get; set; } = true;

        [Parameter] public ScalingType ScalingType { get; set; } = ScalingType.Auto;

        public Boolean ScalesAutomatically => ScalingType == ScalingType.Auto;


        private static void DefaultMinorTickFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(Tick));
            builder.AddAttribute(2, nameof(Tick.Mode), TickMode.Relative);
            builder.AddAttribute(3, nameof(Tick.Value), 20.0);
            builder.CloseComponent();
        }

        private static void DefaultMajorTickFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(Tick));
            builder.AddAttribute(2, nameof(Tick.Mode), TickMode.Relative);
            builder.AddAttribute(3, nameof(Tick.Value), 10.0);
            builder.CloseComponent();
        }

        public void TickUpdated(Tick tick)
        {
            if (tick.IsMajorTick == true)
            {
                if (_majorTick == null)
                {
                    _majorTick = tick;
                }

                Chart.MajorTickChanged(this, tick);
            }
            else
            {
                if (_minorTick == null)
                {
                    _minorTick = tick;
                }

                Chart.MinorTickChanged(this, tick);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Chart == null)
            {
                throw new ArgumentException("a axes needs to be placed inside a Chart");
            }

            if (Chart.Contains(this) == false)
            {
                Chart.Add(this);
                ISnapshot<NumericLinearAxisSnapshot> _this = this;
                _this.CreateSnapshot();
            }
            else
            {
                ISnapshot<NumericLinearAxisSnapshot> _this = this;
                if(_this.SnapshotHasChanged(true) == true)
                {
                    Chart.AxesUpdated(this);
                }
            }
        }

        public void Dispose()
        {
            Chart?.Remove(this);
        }

        NumericLinearAxisSnapshot ISnapshot<NumericLinearAxisSnapshot>.OldSnapshotValue { get; set; }
        NumericLinearAxisSnapshot ISnapshot<NumericLinearAxisSnapshot>.CreateSnapShot() => new NumericLinearAxisSnapshot(Min, Max, ShowMinorTicks, ShowMajorTicks, ScalingType);
    }
}
