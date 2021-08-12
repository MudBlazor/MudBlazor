
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudBlazor.EnhanceChart
{
    public abstract class MudEnhancedChartBase : ComponentBase
    {
        private Boolean _triggerAnimation = false;

        protected Boolean TriggerAnimation
        {
            get => _triggerAnimation;
            set
            {
                if (value != _triggerAnimation)
                {
                    _triggerAnimation = value;
                }
            }
        }
        protected Boolean IsRendered { get; private set; }

        private Boolean _updateStateFlag = false;

        protected void RemoveUpdateStateFlag() => _updateStateFlag = false;
        protected void RaiseUpdateStateFlag() => _updateStateFlag = true;

        /// <summary>
        /// The "parent" Chart of this BarChart
        /// </summary>
        [CascadingParameter] public MudEnhancedChart Chart { get; set; }

        /// <summary>
        /// If this value is true, the bars will have a transition animation when the value changes
        /// </summary>
        [Parameter] public Boolean AnimationIsEnabled { get; set; } = true;

        /// <summary>
        /// Uniuque Id of a chart use to trigger animation and other internal functions.
        /// </summary>
        [Parameter] public Guid Id { get; set; } = Guid.NewGuid();

        protected abstract void PreserveCurrentChartStates();

        [Inject]
        public IJSRuntime _jsruntime { get; set; }

        /// <summary>
        /// Callback for changes about the legend. This is invoked for instance if a new series is added
        /// </summary>
        [Parameter] public EventCallback<ChartLegendInfo> LegendInfoChanged { get; set; }

        /// <summary>
        /// The data for the legend
        /// </summary>
        public abstract ChartLegendInfo LegendInfo { get; }

        protected virtual void InvokeLegendChanged()
        {
            var info = LegendInfo;
            LegendInfoChanged.InvokeAsync(info);
            Chart?.UpdateLegend(info);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender == true)
            {
                IsRendered = true;
            }

            if (_updateStateFlag == true)
            {
                StateHasChanged();
                return;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (AnimationIsEnabled == false) { return; }
            if (TriggerAnimation == false) { return; }

            try
            {
                TriggerAnimation = false;
                PreserveCurrentChartStates();
                await _jsruntime.InvokeVoidAsync("mudEnhancedChartHelper.triggerAnimation", Id);
            }
            catch (Exception)
            {
            }
        }

        protected internal void SoftRedraw()
        {
            StateHasChanged();
        }

        public void ForceRedraw()
        {
            TriggerAnimation = true;
            CreateDrawingInstruction();
        }

        protected abstract void CreateDrawingInstruction();
    }
}
