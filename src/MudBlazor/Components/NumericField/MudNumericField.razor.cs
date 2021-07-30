using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        private MudInput<string> _elementReference;

        private Converter<string> _inputConverter;

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            await base.SetValueAsync(value, valueChanged || updateText);
        }

        protected override void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            if (RuntimeLocation.IsServerSide)
                _key++; // this forces a re-render on the inner input to display the value correctly
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
            return (Num.To<T>(value), false);
        }


        private long _key = 0;
        private bool _keyDownPreventDefault;

        /// <summary>
        /// Overrides KeyDown event, intercepts Arrow Up/Down and uses them to add/substract the value manually by the step value.
        /// Relying on the browser mean the steps are each integer multiple from <see cref="Min"/> up until <see cref="Max"/>.
        /// This align the behaviour with the spinner buttons.
        /// </summary>
        /// <remarks>https://try.mudblazor.com/snippet/QamlkdvmBtrsuEtb</remarks>
        protected async Task InterceptKeydown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;

            if (obj.Type == "keydown") //KeyDown or repeat, blazor never fires InvokeKeyPress
            {
                switch (obj.Key)
                {
                    case "ArrowUp":
                        if (RuntimeLocation.IsServerSide)
                        {
                            if (!Immediate)
                            {
                                _key++;
                                await Task.Delay(1);
                                await Increment();
                                await Task.Delay(1);
                                _ = FocusAsync();
                            }
                            else
                                await Increment();
                        }
                        else
                        {
                            await Increment();
                            _elementReference.ForceRender(true);
                        }

                        return;

                    case "ArrowDown":
                        if (RuntimeLocation.IsServerSide)
                        {
                            if (!Immediate)
                            {
                                _key++;
                                await Task.Delay(1);
                                await Decrement();
                                await Task.Delay(1);
                                _ = FocusAsync();
                            }
                            else
                                await Decrement();
                        }
                        else
                        {
                            await Decrement();
                            _elementReference.ForceRender(true);
                        }
                        return;
                    // various navigation keys
                    case "ArrowLeft":
                    case "ArrowRight":
                    case "Tab":
                    case "Backspace":
                    case "Delete":
                        break;

                    //copy/paste
                    case "v":
                    case "c":
                        if (obj.CtrlKey is false)
                        {
                            _keyDownPreventDefault = true;
                            return;
                        }

                        break;

                    default:
                        var acceptableKeyTypes = new Regex("^[0-9,.-]$");
                        var isMatch = acceptableKeyTypes.Match(obj.Key).Success;
                        if (isMatch is false)
                        {
                            _keyDownPreventDefault = true;
                            return;
                        }
                        break;
                }
            }

            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        protected void InterceptKeyUp(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            switch (obj.Key)
            {
                case "ArrowUp":
                    if (RuntimeLocation.IsServerSide)
                        _elementReference?.ForceRender(forceTextUpdate: true);
                    break;

                case "ArrowDown":
                    if (RuntimeLocation.IsServerSide)
                        _elementReference?.ForceRender(forceTextUpdate: true);
                    break;
            }

            _keyDownPreventDefault = false;
            StateHasChanged();
            OnKeyUp.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter]
        public string Placeholder { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        //Tracks if Min has a value.
        private bool _minHasValue = false;

        //default value for the type
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

        //Tracks if Max has a value.
        private bool _maxHasValue = false;

        //default value for the type
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

        //Tracks if Max has a value.
        private bool _stepHasValue = false;

        //default value for the type, it's useful for decimal type to avoid constant evaluation
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
        /// Defaults to [0-9,\.\-+]*
        /// To get a numerical keyboard on safari, use the pattern. The default pattern should achieve numerical keyboard.
        /// </summary>
        [Parameter]
        public override string Pattern { get; set; } = @"[0-9,\.\-+]*";

        protected string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var pattern = Pattern;
            var cleanedText = "";
            foreach (Match m in Regex.Matches(text, pattern))
            {
                cleanedText += m.Captures[0].Value;
            }

            cleanedText = cleanedText[0] + cleanedText.Substring(1).Replace("-", string.Empty);

            return cleanedText;
        }

    }
}
