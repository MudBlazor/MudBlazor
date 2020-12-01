using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudInput<T> : MudBaseInput<T>
    {
        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)                
                .AddClass("mud-shrink", when: () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-input-error", HasErrors)
                .AddClass(Class)
                .Build();

        protected string InputClassname =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-root-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-root-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass(Class)
                .Build();

        protected string AdornmentClassname =>
            new CssBuilder("mud-input-adornment")
                .AddClass($"mud-input-adornment-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-text", !String.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .AddClass(Class)
                .Build();

        protected string InputTypeString => InputType.ToDescriptionString();

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private ElementReference elementReference;

        public override ValueTask FocusAsync()
        {
            //The context must be a WebElementReferenceContext otherwise the JSRuntime is not available otherwise we just return a completed task and pretend everything is ok
            return elementReference.Context is WebElementReferenceContext ? elementReference.FocusAsync() : ValueTask.CompletedTask;
        }
    }

    public class MudInputString : MudInput<string> { }
}