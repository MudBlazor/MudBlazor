// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMaskField<T> : MudBaseInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

        private PlaceholderMask _mask;
        private string _rawValue;

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }
        [Inject] private IJsEvent _jsEvent { get; set; }
        [Inject] private IJsApiService _jsApiService { get; set; }

        private string _elementId = "maskfield_" + Guid.NewGuid().ToString().Substring(0, 8);

        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public PlaceholderMask Mask { 
            get => _mask;
            set
            {
                Unbind(_mask);
                _mask = value;
                Bind(_mask);
            }
        }

        private void Bind(PlaceholderMask impl)
        {
            if (impl == null)
                return;
            impl.SetCaretPosition = SetCaretPosition;
            impl.SetTextAsync = SetText;
            impl.SetValueAsync = SetValue;
        }

        private void Unbind(PlaceholderMask impl)
        {
            if (impl == null)
                return;
            impl.SetCaretPosition = null;
            impl.SetTextAsync = null;
            impl.SetValueAsync = null;
        }

        private Task SetText(string text)
        {
            var rawValue = _mask.GetRawValueFromText(text) ?? "";
            var textToSet = rawValue.Length > 0 && !string.IsNullOrEmpty(Placeholder) ? text : null;
            return SetTextAsync(textToSet, updateValue: false);
        }

        private Task SetValue(string text)
        {
            return SetValueAsync(Converter.Get(text), updateText: false);
        }

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }


        protected override async Task SetTextAsync(string text, bool updateValue = true)
        {
            //Console.WriteLine($"SetTextAsync: '{text}' updateValue:{updateValue}");
            _mask.Text = text;
            await base.SetTextAsync(text, updateValue);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            //Console.WriteLine($"SetValueAsync: '{value}' updateText:{updateText}");
            _rawValue = Converter.Set(value);
            if (updateText)
            {
                ////If value changed by user from outside, create dictionary from _rawValue scratch, if there is a problem with value changing, probably is here
                //if (_rawValue != null && Converter.Set(Value) != _rawValue)
                //    _maskedText.SetRawValueDictionary(_rawValue);
                await _mask.ImplementMask(null);
            }
            // never update text directly. we already did it
            await base.SetValueAsync(value, updateText: false);
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            if (_mask.GetRawValueFromText(Text) == Converter.Set(Value))
                return;
            await _mask.ImplementMask(null);
        }

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            return SetTextAsync(null, updateValue: true);
        }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _jsEvent.Connect(_elementId, new JsEventOptions
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    TagName = "INPUT"
                });
                _jsEvent.CaretPositionChanged += OnCaretPositionChanged;
                _jsEvent.Copy += OnCopy;
                _jsEvent.Paste += OnPaste;
                _jsEvent.Select += OnSelect;
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page,
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page,
                        new KeyOptions { Key="ArrowLeft", PreventDown = "key+none" },
                        new KeyOptions { Key="ArrowRight", PreventDown = "key+none" },
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="PageUp", PreventDown = "key+none" },
                        new KeyOptions { Key="PageDown", PreventDown = "key+none" },
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key=@"/^(\w|\d)$/", PreventDown = "key+none|key+shift" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key="Delete", PreventDown = "key+none" },
                        new KeyOptions { Key="Shift", PreventDown = "key+none" },
                        new KeyOptions { Key="[/-+_-*()%&]'/", PreventDown = "key+none" },
                    },
                });
            }
            if (_isFocused && _mask.Selection == null)
                await _elementReference.SelectRangeAsync(_mask.CaretPosition, _mask.CaretPosition);
            await base.OnAfterRenderAsync(firstRender);
        }

        internal void OnCaretPositionChanged(int pos)
        {
            _mask.CaretPosition = pos;
            _mask.Selection = null;
            //Console.WriteLine($"Caret position: {pos}");
        }

        internal void OnCopy()
        {
            var text = Value?.ToString();
            _jsApiService.CopyToClipboardAsync(text);
            //Console.WriteLine($"Copy: {text}");
        }

        internal async void OnPaste(string text)
        {
            ////Console.WriteLine($"Paste: {text}");
            if (Text == null || _mask == null)
                return;
            _mask.Paste(text, _mask.CaretPosition);
            await SetValueAsync(Converter.Get(_mask.GetRawValueFromDictionary()), false);
        }

        public void OnSelect(int start, int end)
        {
            _mask.Selection = (start, end);
            //Console.WriteLine($"Select: {_selection.Value.Item1}-{_selection.Value.Item2}");
        }

        internal async void OnFocused(FocusEventArgs obj)
        {
            _isFocused = true;
            if (!string.IsNullOrEmpty(Converter.Set(Value)) || string.IsNullOrEmpty(Placeholder))
            {
                //This delay let click event fires first to set caret position proper (for only first click)
                await Task.Delay(1);
                await _mask.ImplementMask(null);
                SetCaretPosition(_mask.FindFirstCaretLocation());
            }
        }

        protected internal override void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            if (string.IsNullOrEmpty(_rawValue))
            {
                SetTextAsync("", updateValue: false).AndForget();
            }
            _isFocused = false;
        }

        public string GetRawValue()
        {
            if (_rawValue == null)
            {
                return "";
            }
            return _rawValue;
        }

        public void SetCaretPosition(int caretPosition)
        {
            _mask.CaretPosition = caretPosition;
            if (!_isFocused)
                return;
            _elementReference.SelectRangeAsync(caretPosition, caretPosition).AndForget();
            StateHasChanged();
        }

        // TODO: remove, can be replaced by setting this.Value
        //This is used for some tests, not has a production aim.
        internal async Task SetBothValueAndText(T value)
        {
            await SetValueAsync(value, true);
        }

        protected internal void HandleKeyDown(KeyboardEventArgs obj)
        {
            _mask.HandleKeyDown(obj.Key, obj.ShiftKey, obj.CtrlKey);
            OnKeyDown.InvokeAsync(obj).AndForget();
        }
    }
}
