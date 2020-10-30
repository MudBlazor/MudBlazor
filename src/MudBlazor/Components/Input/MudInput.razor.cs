using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudInput : MudBaseInput
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


        // todo: pull down into generic child like with TextField

        private string _value;
        private bool _settingValue;
        /// <summary>
        /// Fired when the Value property changes. 
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// The value of this input element. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public string Value
        {
            get => _value;
            set
            {
                if (object.Equals(value, _value))
                    return;
                if (_settingValue)
                    return;
                _settingValue = true;
                try
                {
                    _value = value;
                    //GenericValueChanged(value);
                    //ValidateValue(value);
                    ValueChanged.InvokeAsync(value);
                }
                finally
                {
                    _settingValue = false;
                }
            }
        }
    }
}
