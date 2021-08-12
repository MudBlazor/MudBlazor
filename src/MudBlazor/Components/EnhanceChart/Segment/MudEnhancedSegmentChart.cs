using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    record MudEnhancedSegmentChartSnapShot(Double StartAngle, Double Padding);

    public abstract class MudEnhancedSegmentChart<TChart, TPoint, TSegementRepresentation> : MudEnhancedChartBase, ICollection<TPoint>, ISnapshot<MudEnhancedSegmentChartSnapShot>
        where TPoint : MudEnhancedSegementChartDataPoint<TChart, TPoint, TSegementRepresentation>
        where TSegementRepresentation : SvgSegementRepresentation
        where TChart : MudEnhancedSegmentChart<TChart, TPoint, TSegementRepresentation>
    {
        #region Fields

        private List<TPoint> _points = new();
        private Dictionary<String, String> _oldSegementPaths = new();
        protected List<TSegementRepresentation> Segments { get; init; } = new();

        #endregion

        #region Properties

        /// <summary>
        /// The data for the legend
        /// </summary>
        public override ChartLegendInfo LegendInfo =>
        new ChartLegendInfo(new[] { new DataPointBasedChartLegendInfoGroup(
            _points.Select(y => new ChartLegendInfoPoint(y.Label, y.FillColor, y.IsEnabled, y))) });


        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public Double Padding { get; set; } = 2.0;
        [Parameter] public Double StartAngle { get; set; } = -90;

        [Parameter] public Action<TChart> BeforeCreatingInstructionCallBack { get; set; }

        internal void UpdatedPoint(TPoint point, Boolean triggerAnimation)
        {
            if (TriggerAnimation == false)
            {
                TriggerAnimation = triggerAnimation;
            }

            CreateDrawingInstruction();
        }

        #endregion

        public MudEnhancedSegmentChart()
        {
            base.AnimationIsEnabled = false;
        }

        #region Methods

        #region Tooltips and legend

        internal void AddTooltip(SegementChartToolTipInfo tooltipInfo)
        {
            Chart?.UpdateTooltip(new[] { tooltipInfo });
        }

        internal void RemoveTooltip()
        {
            Chart?.UpdateTooltip(Array.Empty<SegementChartToolTipInfo>());
        }

        #endregion

        #endregion

        protected internal void SetSeriesAsExclusivelyActive(IDataPoint inputPoint)
        {
            foreach (var point in _points)
            {
                if (inputPoint == point)
                {
                    point.SetAsActive();
                }
                else
                {
                    point.SetAsInactive();
                }
            }

            SoftRedraw();
        }

        protected internal void SetAllSeriesAsActive()
        {
            foreach (var point in _points)
            {
                point.SetAsActive();
            }

            SoftRedraw();
        }

        #region ICollection member

        public int Count => _points.Count;
        public bool IsReadOnly => false;

        public void Add(TPoint item)
        {
            _points.Add(item);
            InvokeLegendChanged();
            TriggerAnimation = true;
            CreateDrawingInstruction();
        }

        public void Clear()
        {
            _points.Clear();
            InvokeLegendChanged();
            CreateDrawingInstruction();
        }


        public bool Remove(TPoint item)
        {
            Boolean result = _points.Remove(item);
            if (result == true)
            {
                InvokeLegendChanged();
                TriggerAnimation = true;
                CreateDrawingInstruction();
            }

            return result;
        }

        public bool Contains(TPoint item) => _points.Contains(item);
        public void CopyTo(TPoint[] array, int arrayIndex) => _points.CopyTo(array, arrayIndex);

        public IEnumerator<TPoint> GetEnumerator() => _points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();

        protected abstract String GetPathForNewElement(TPoint item, Double radius, Double startAngle, Double minAngle);
        protected abstract TSegementRepresentation GetSegementRepresentation(TPoint point, Double radius, Double segementLength, Double startAngle);

        protected override void CreateDrawingInstruction()
        {
            BeforeCreatingInstructionCallBack?.Invoke((TChart)this);

            if (_points.Any() == false) { return; }

            Segments.Clear();

            var relevantPoints = _points.Where(x => x.IsEnabled == true);

            Double total = relevantPoints.Sum(x => x.Value);

            Double radius = 50 - Padding;

            Double currentAngle = StartAngle;

            foreach (var item in relevantPoints)
            {
                Double segmentLength = (item.Value / total) * 360;
                String id = $"{item.Id}";

                TSegementRepresentation representation = GetSegementRepresentation(item, radius, relevantPoints.Count() > 1 ? segmentLength : 359.95, currentAngle);

                representation.OldPath = AnimationIsEnabled == true ? (_oldSegementPaths.ContainsKey(id) ? _oldSegementPaths[id] : GetPathForNewElement(item, radius, currentAngle, 0.1)) : String.Empty;
                representation.Id = id;

                Segments.Add(representation);

                currentAngle -= segmentLength;
            }

            RaiseUpdateStateFlag();
        }

        protected override void PreserveCurrentChartStates()
        {
            _oldSegementPaths.Clear();

            foreach (var item in Segments)
            {
                _oldSegementPaths.Add(item.Id, item.GetPathValue());
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            ISnapshot<MudEnhancedSegmentChartSnapShot> _this = this;

            if (_this.SnapshotHasChanged(true) == true)
            {
                CreateDrawingInstruction();
            }
        }

        MudEnhancedSegmentChartSnapShot ISnapshot<MudEnhancedSegmentChartSnapShot>.OldSnapshotValue { get; set; }
        MudEnhancedSegmentChartSnapShot ISnapshot<MudEnhancedSegmentChartSnapShot>.CreateSnapShot() => new MudEnhancedSegmentChartSnapShot(StartAngle, Padding);

        #endregion

    }
}
