using System;
using System.Collections.Generic;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public class TickOverview
    {
        public Double Min { get; set; } = 0.0;
        public Double Max { get; set; } = 0.0;
        public Double Distance => Max - Min;
        public Double StartTickValue { get; set; } = 0.0;
        public Double MajorTickNumericValue { get; set; } = 0.0;
        public Double MinorTickNumericValue { get; set; } = 0.0;

        public Int32 MajorTickAmount { get; set; } = 2;
        public Int32 MinorTickAmount { get; set; } = 0;

        public Boolean HasValues { get; set; } = false;
    }

    public interface ITick
    {
        Boolean ShowGridLines { get; }
        Double GridLineThickness { get; }
        MudColor GridLineColor { get; }
        String GridLineCssClass { get; }
    }

    public interface IYAxis
    {
        void TickUpdated(MudEnhancedTick tick);
        Boolean ScalesAutomatically { get; }

        Double LabelSize { get; }
        Double Margin { get; }
        String LabelCssClass { get; }

        YAxisPlacement Placement { get; }

        Double MajorTickValue { get; }
        Double MinorTickValue { get; }

        ITick MajorTickInfo { get; }
        ITick MinorTickInfo { get; }

        Guid Id { get; }

        void RemoveTick(bool isMajorTick);
        void CalculateTicks();
        TickOverview GetTickInfo();
        void ProcessDataSet(IDataSet set);
        void ClearTickInfo();
    }
}
