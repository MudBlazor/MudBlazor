using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MudBlazor
{
    public partial class MudSlider<T> : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-slider")
                .AddClass(Class)
                .Build();

        protected string _value;
        protected string _min = "0";
        protected string _max = "100";
        protected string _step = "1";

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter]
        public T Min
        {
            get => Converter.Get(_min);
            set => _min = Converter.Set(value);
        }

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        /// 
        [Parameter]
        public T Max
        {
            get => Converter.Get(_max);
            set => _max = Converter.Set(value);
        }

        /// <summary>
        /// How many steps the slider should take on each move.
        /// </summary>
        /// 
        [Parameter]
        public T Step
        {
            get => Converter.Get(_step);
            set => _step = Converter.Set(value);
        }

        /// <summary>
        /// If true, the slider will be disabled.
        /// </summary>
        /// 
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter] public Converter<T> Converter { get; set; } = new DefaultConverter<T>() { Culture = CultureInfo.InvariantCulture };

        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        public T Value
        {
            get => Converter.Get(_value);
            set
            {
                var d = Converter.Set(value);
                if (_value==d)
                    return;
                _value = d;
                ValueChanged.InvokeAsync(value);
            }
        }

        protected string Text
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                ValueChanged.InvokeAsync(Value);
            }
        }

        /// <summary>
        /// If true, the dragging the slider will update the Value immediately.
        /// If false, the Value is updated only on releasing the handle.
        /// </summary>
        [Parameter]
        public bool Immediate { get; set; } = true;

        //protected static string ToFpS(double value)
        //{
        //    var s = ToS(value);
        //    if (!s.Contains('.'))
        //        return s + ".0";
        //    return s;
        //}
    }
}
