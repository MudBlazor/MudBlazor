using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public class TickOverview
    {
        public Double Min { get; set; } = 0.0;
        public Double Max { get; set; } = Double.MinValue;
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
        String GridLineColor { get; }
        String GridLineCssClass { get; }
    }

    public interface IYAxis
    {
        void TickUpdated(MudEnhancedTick tick);
        Double Min { get; }
        Double Max { get; }
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
        void ProcessDataSet(IEnumerable<IDataSeries> set);
    }
}
