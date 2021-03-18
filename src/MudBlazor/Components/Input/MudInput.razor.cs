using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        //private DateTime _lastTyped; // timestamp to prohibit rendering while typing to prevent swallowed chars
        //private readonly TimeSpan _renderProtectionInterval = TimeSpan.FromMilliseconds(500);

        protected override bool ShouldRender()
        {
            //if (Immediate && _lastTyped.Add(_renderProtectionInterval) > DateTime.Now)
            if (Immediate && _isFocused)
                return false;
            return true;
        }

        protected Task OnInput(ChangeEventArgs args)
        {
            //_lastTyped = DateTime.Now;
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
    }

    public class MudInputString : MudInput<string> { }
}
