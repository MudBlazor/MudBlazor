using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudInput<T> : MudBaseInput<T>
    {
        protected string Classname => MudInputCssHelper.GetClassname(this,
            () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(Placeholder));

        protected string InputClassname => MudInputCssHelper.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelper.GetAdornmentClassname(this);

        protected string InputTypeString => InputType.ToDescriptionString();

        protected override bool ShouldRender()
        {
            if (Immediate && _isFocused && !_showClearableRenderUpdate)
                return false;
            _showClearableRenderUpdate = false;
            return true;
        }

        protected Task OnInput(ChangeEventArgs args)
        {
            _isFocused = true;
            return Immediate ? SetTextAsync(args?.Value as string) : Task.CompletedTask;
        }

        protected Task OnChange(ChangeEventArgs args)
        {
            return Immediate ? Task.CompletedTask : SetTextAsync(args?.Value as string);
        }

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private ElementReference _elementReference;

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.MudSelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.MudSelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        private bool _showClearable;

        private bool _showClearableRenderUpdate;

        private void UpdateClearable(object value)
        {
            var showClearable = Clearable && ((value is string stringValue && !string.IsNullOrWhiteSpace(stringValue)) || (value is not string && value is not null));
            if (_showClearable != showClearable)
            {
                _showClearable = showClearable;
                _showClearableRenderUpdate = true;
            }
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            await base.UpdateTextPropertyAsync(updateValue);
            UpdateClearable(Text);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            await base.UpdateValuePropertyAsync(updateText);
            UpdateClearable(Value);
        }

        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, true);
            await OnClearButtonClick.InvokeAsync(e);
        }
    }

    public class MudInputString : MudInput<string> { }
}
