using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System;

namespace MudBlazor
{
    public partial class MudInput : MudBaseInputText
    {
        protected string Classname =>
       new CssBuilder("mud-input")
         .AddClass($"mud-input-{Variant.ToDescriptionString()}")
         .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
         .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
         .AddClass("mud-shrink", when: () => !string.IsNullOrEmpty(Value) || Adornment == Adornment.Start)
         .AddClass("mud-disabled", Disabled)
         .AddClass("mud-error", Error)
         .AddClass(Class)
       .Build();

        protected string InputClassname =>
       new CssBuilder("mud-input-root")
         .AddClass($"mud-input-root-{Variant.ToDescriptionString()}")
         .AddClass($"mud-input-root-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
         .AddClass(Class)
       .Build();

        protected string AdornmentClassname =>
       new CssBuilder("mud-input-adornment")
         .AddClass($"mud-input-adornment-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
         .AddClass($"mud-text", !String.IsNullOrEmpty(AdornmentText))
         .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
         .AddClass(Class)
       .Build();

        protected string _InputType => new CssBuilder().AddClass(InputType.ToDescriptionString()).Build();

        [Parameter]
        public EventCallback<ChangeEventArgs> OnInput { get; set; }
    }
}
