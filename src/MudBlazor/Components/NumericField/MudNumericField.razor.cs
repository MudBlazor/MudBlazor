using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNumericField<T> : MudDebouncedInput<T>
    {
        public MudNumericField() : base()
        {
            Validation = new Func<T, Task<bool>>(ValidateInput);
            _inputConverter = new NumericBoundariesConverter<T>((val) => ConstrainBoundaries(val).value)
            {
                FilterFunc = CleanText,
                Culture = CultureInfo.CurrentUICulture,
            };

            #region parameters default depending on T

            //sbyte
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
            {
                _minDefault = (T)(object)sbyte.MinValue;
                _maxDefault = (T)(object)sbyte.MaxValue;
                _stepDefault = (T)(object)(sbyte)1;
            }
            // byte
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
            {
                _minDefault = (T)(object)byte.MinValue;
                _maxDefault = (T)(object)byte.MaxValue;
                _stepDefault = (T)(object)(byte)1;
            }
            // short
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
            {
                _minDefault = (T)(object)short.MinValue;
                _maxDefault = (T)(object)short.MaxValue;
                _stepDefault = (T)(object)(short)1;
            }
            // ushort
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
            {
                _minDefault = (T)(object)ushort.MinValue;
                _maxDefault = (T)(object)ushort.MaxValue;
                _stepDefault = (T)(object)(ushort)1;
            }
            // int
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                _minDefault = (T)(object)int.MinValue;
                _maxDefault = (T)(object)int.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // uint
            else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
            {
                _minDefault = (T)(object)uint.MinValue;
                _maxDefault = (T)(object)uint.MaxValue;
                _stepDefault = (T)(object)1u;
            }
            // long
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                _minDefault = (T)(object)long.MinValue;
                _maxDefault = (T)(object)long.MaxValue;
                _stepDefault = (T)(object)1L;
            }
            // ulong
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
            {
                _minDefault = (T)(object)ulong.MinValue;
                _maxDefault = (T)(object)ulong.MaxValue;
                _stepDefault = (T)(object)1ul;
            }
            // float
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
            {
                _minDefault = (T)(object)float.MinValue;
                _maxDefault = (T)(object)float.MaxValue;
                _stepDefault = (T)(object)1.0f;
            }
            // double
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
            {
                _minDefault = (T)(object)double.MinValue;
                _maxDefault = (T)(object)double.MaxValue;
                _stepDefault = (T)(object)1.0;
            }
            // decimal
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
            {
                _minDefault = (T)(object)decimal.MinValue;
                _maxDefault = (T)(object)decimal.MaxValue;
                _stepDefault = (T)(object)1M;
            }

            #endregion parameters default depending on T
        }

        protected override bool SetCulture(CultureInfo value)
        {
            var changed = base.SetCulture(value);
            _inputConverter.Culture = value;
            return changed;
        }

        protected string Classname =>
            new CssBuilder("mud-input-input-control mud-input-number-control " +
                           (HideSpinButtons ? "mud-input-nospin" : "mud-input-showspin"))
                .AddClass(Class)
                .Build();


        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }

        private string _elementId = "numericField_" + Guid.NewGuid().ToString().Substring(0, 8);

        private MudInput<string> _elementReference;

        private Converter<string> _inputConverter;

        [ExcludeFromCodeCoverage]
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        [ExcludeFromCodeCoverage]
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        [ExcludeFromCodeCoverage]
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        protected override Task SetValueAsync(T value, bool updateText = true)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            return base.SetValueAsync(value, valueChanged || updateText);
        }

        protected override async void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            await UpdateTextPropertyAsync(false);
        }

        protected async Task<bool> ValidateInput(T value)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            if (valueChanged)
                await SetValueAsync(value, true);
            return true; //Don't show errors
        }

        /// <summary>
        /// Decrements or increments depending on factor
        /// </summary>
        /// <param name="factor">Multiplication factor (1 or -1) will be applied to the step</param>
        private async Task Change(double factor = 1)
        {
            var value = Num.To<T>(Num.From(Value) + Num.From(Step) * factor);
            await SetValueAsync(ConstrainBoundaries(value).value);
            _elementReference.SetText(Text).AndForget();
        }

        /// <summary>
        /// Adds a Step to the Value
        /// </summary>
        public Task Increment() => Change(factor: 1);


        /// <summary>
        /// Substracts a Step from the Value
        /// </summary>
        public Task Decrement() => Change(factor: -1);

        /// <summary>
        /// Checks if the value respects the boundaries set for this instance.
        /// </summary>
        /// <param name="v">Value to check.</param>
        /// <returns>Returns a valid value and if it has been changed.</returns>
        protected (T value, bool changed) ConstrainBoundaries(T v)
        {
            var value = Num.From(v);
            var max = Num.From(Max);
            var min = Num.From(Min);
            //check if Max/Min has value, if not use MaxValue/MinValue for that data type
            if (value > max)
                return (Max, true);
            else if (value < min)
                return (Min, true);
            //return T default (null) when there is no value
            else if (v == null)
            {
                return (default(T), true);
            }

            return (Num.To<T>(value), false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead increment
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead decrement
                        new KeyOptions { Key="Dead", PreventDown = "key+any" }, // prevent dead keys like ^ ` ´ etc
                        new KeyOptions { Key="/^(?!"+(Pattern ?? "[0-9]").TrimEnd('*')+").$/", PreventDown = "key+none|key+shift|key+alt" }, // prevent input of all other characters except allowed, like [0-9.,-+]
                    },
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task HandleKeydown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            switch (obj.Key)
            {
                case "ArrowUp":
                    await Increment();
                    break;
                case "ArrowDown":
                    await Decrement();
                    break;
            }
            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        protected Task HandleKeyUp(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return Task.CompletedTask;
            OnKeyUp.InvokeAsync(obj).AndForget();
            return Task.CompletedTask;
        }

        protected async Task OnMouseWheel(WheelEventArgs obj)
        {
            if (!obj.ShiftKey)
                return;
            if (obj.DeltaY < 0)
            {
                if (InvertMouseWheel == false)
                    await Increment();
                else
                    await Decrement();
            }
            else if (obj.DeltaY > 0)
            {
                if (InvertMouseWheel == false)
                    await Decrement();
                else
                    await Increment();
            }
        }

        private bool _minHasValue = false;

        /// <summary>
        /// Reverts mouse wheel up and down events, if true.
        /// </summary>
        [Parameter]
        public bool InvertMouseWheel { get; set; } = false;

        private T _minDefault;

        private T _min;

        /// <summary>
        /// The minimum value for the input.
        /// </summary>
        [Parameter]
        public T Min
        {
            get => _minHasValue ? _min : _minDefault;
            set
            {
                _minHasValue = value != null;
                _min = value;
            }
        }

        private bool _maxHasValue = false;
        private T _maxDefault;
        private T _max;

        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter]
        public T Max
        {
            get => _maxHasValue ? _max : _maxDefault;
            set
            {
                _maxHasValue = value != null;
                _max = value;
            }
        }

        private bool _stepHasValue = false;
        private T _stepDefault;
        private T _step;

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter]
        public T Step
        {
            get => _stepHasValue ? _step : _stepDefault;
            set
            {
                _stepHasValue = value != null;
                _step = value;
            }
        }

        /// <summary>
        /// Hides the spin buttons, the user can still change value with keyboard arrows and manual update.
        /// </summary>
        [Parameter]
        public bool HideSpinButtons { get; set; }

        /// <summary>
        ///  Hints at the type of data that might be entered by the user while editing the input.
        ///  Defaults to numeric
        /// </summary>
        [Parameter]
        public override InputMode InputMode { get; set; } = InputMode.numeric;

        /// <summary>
        /// The pattern attribute, when specified, is a regular expression which the input's value must match in order for the value to pass constraint validation. It must be a valid JavaScript regular expression
        /// Defaults to [0-9,.\-]
        /// To get a numerical keyboard on safari, use the pattern. The default pattern should achieve numerical keyboard.
        ///
        /// Note: this pattern is also used to prevent all input except numbers and allowed characters. So for instance to allow only numbers, no signs and no commas you might change it to to [0-9.]
        /// </summary>
        [Parameter]
        public override string Pattern { get; set; } = @"[0-9,.\-]";

        protected string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var pattern = Pattern + "*";
            var cleanedText = "";
            foreach (Match m in Regex.Matches(text, pattern))
            {
                cleanedText += m.Captures[0].Value;
            }

            cleanedText = cleanedText[0] + cleanedText.Substring(1).Replace("-", string.Empty);

            return cleanedText;
        }

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        private Task OnInputValueChanged(string text)
        {
            return SetTextAsync(text);
        }

        //avoids the format to use scientific notation for large or small number in floating points types, while covering all options
        //https://stackoverflow.com/questions/1546113/double-to-string-conversion-without-scientific-notation
        private const string TagFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";

        private string FormatParam(T value)
        {
            if (value is IFormattable f)
                return f.ToString(TagFormat, CultureInfo.InvariantCulture.NumberFormat);
            else
                return null;
        }
    }
}
