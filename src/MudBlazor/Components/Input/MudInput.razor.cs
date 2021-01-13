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

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private ElementReference _elementReference;

        public override ValueTask FocusAsync()
        {
            //The context must be a WebElementReferenceContext otherwise the JSRuntime is not available otherwise we just return a completed task and pretend everything is ok
            return _elementReference.Context is WebElementReferenceContext ? _elementReference.FocusAsync() : ValueTask.CompletedTask;
        }

        public override ValueTask SelectAsnyc()
        {
            return _elementReference.Context is WebElementReferenceContext ? JSRuntime.InvokeVoidAsync("mbSelectHelper.select", _elementReference) : ValueTask.CompletedTask;
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.Context is WebElementReferenceContext ? JSRuntime.InvokeVoidAsync("mbSelectHelper.selectRange", _elementReference, pos1, pos2) : ValueTask.CompletedTask;
        }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }
    }

    public class MudInputString : MudInput<string> { }
}
