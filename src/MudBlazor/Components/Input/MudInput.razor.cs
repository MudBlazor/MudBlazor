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

        protected Task OnInput(ChangeEventArgs args)
        {
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
            return JSRuntime.InvokeVoidAsync("elementReference.focus", _elementReference);
        }

        public override ValueTask SelectAsync()
        {
            return JSRuntime.InvokeVoidAsync("mbSelectHelper.select", _elementReference);
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return JSRuntime.InvokeVoidAsync("mbSelectHelper.selectRange", _elementReference, pos1, pos2);
        }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }


        /// <summary>
        /// Invokes the callback when the Up arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnIncrement { get; set; }

        /// <summary>
        /// Invokes the callback when the Down arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnDecrement { get; set; }

    }

    public class MudInputString : MudInput<string> { }
}
