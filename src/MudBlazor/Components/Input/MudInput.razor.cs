using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudInput<T> : MudBaseInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input")
             .AddClass($"mud-input-{Variant.ToDescriptionString()}")
             .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
             .AddClass("mud-input-underline", when: () => DisableUnderLine == false && Variant != Variant.Outlined)
             .AddClass("mud-shrink", when: () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start)
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

        private string _text;
        private Converter<T> _converter = new DefaultConverter<T>();

        [Parameter] public string Text
        {
            get => _text;
            set
            {
                if (_text == value)
                    return;
                // update loop protection!
                if (_settingText) 
                    return;
                _settingText=true;
                try
                {
                    _text = value;
                    StringValueChanged(value);
                    TextChanged.InvokeAsync(value);
                }
                finally
                {
                    _settingText=false;
                }
            }
        }
        
        [Parameter] public EventCallback<string> TextChanged { get; set; }
        
        /// <summary>
        /// Text change hook for descendants  
        /// </summary>
        /// <param name="value"></param>
        protected virtual void StringValueChanged(string text)
        {
            Value=Converter.Get(text);
        }

        protected override void GenericValueChanged(T value)
        {
            Text = Converter.Set(value);
        }

        private bool _settingText;
        bool _settingValue;

        [Parameter]
        public Converter<T> Converter
        {
            get => _converter;
            set
            {
                if (_converter == value)
                    return;
                _converter = value;
                Text = Converter.Set(Value);
            }
        }
    }
}
