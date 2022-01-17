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

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }
        [Inject] private IJsEvent _jsEvent { get; set; }
        [Inject] private IJsApiService _jsApiService { get; set; }

        private string _elementId = "maskfield_" + Guid.NewGuid().ToString().Substring(0, 8);

        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public BaseMask Mask { get; set; } = new SimpleMask("aaaa-0000");
        
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
                    EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page,
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page,
                        //new KeyOptions { Key="ArrowLeft", PreventDown = "key+none" },
                        //new KeyOptions { Key="ArrowRight", PreventDown = "key+none" },
                        //new KeyOptions { Key="Home", PreventDown = "key+none" },
                        //new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="PageUp", PreventDown = "key+none" },
                        new KeyOptions { Key="PageDown", PreventDown = "key+none" },
                        // new KeyOptions { Key="Escape" },
                        new KeyOptions { Key=@"/^.$/", PreventDown = "key+none|key+shift" },
                        //new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        //new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true },
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key="Delete", PreventDown = "key+none" },
                        // new KeyOptions { Key="Shift", PreventDown = "key+none" },
                        // new KeyOptions { Key="[/-+_-*()%&]'/", PreventDown = "key+none" },
                    },
                });
                _keyInterceptor.KeyDown += e => HandleKeyDown(e).AndForget();
            }
            // if (_isFocused && _mask.Selection == null)
            //     await _elementReference.SelectRangeAsync(_mask.CaretPosition, _mask.CaretPosition);
            await base.OnAfterRenderAsync(firstRender);
        }
           
        protected internal async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.CtrlKey || e.AltKey)
                return;
            Console.WriteLine($"HandleKeyDown: '{e.Key}'");
            switch (e.Key)
            {
                case "Backspace":
                    Mask.Backspace();
                    return;
                case "Delete":
                    Mask.Delete();
                    return;
                //case "ArrowLeft"
            }
            if (Regex.IsMatch(e.Key, @"^.$"))
                Mask.Insert(e.Key);
            Console.WriteLine(Mask.ToString());
            await Update();
            await OnKeyDown.InvokeAsync(e);
        }

        private async Task Update()
        {
            await SetTextAsync(Mask.Text, updateValue: false);
            SetCaretPosition(Mask.CaretPos);
        }

        private Task SetValue(string text)
        {
            return SetValueAsync(Converter.Get(text), updateText: false);
        }

        protected override async Task SetTextAsync(string text, bool updateValue = true)
        {
            Console.WriteLine($"SetTextAsync: '{text}' updateValue:{updateValue}");
            if (Mask.Text != text)
                Mask.SetText(text);
            await base.SetTextAsync(text, updateValue);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            //Console.WriteLine($"SetValueAsync: '{value}' updateText:{updateText}");
            // _rawValue = Converter.Set(value);
            // if (updateText)
            // {
            //     //If value changed by user from outside
            //     //if (_rawValue != null && Converter.Set(Value) != _rawValue)
            //     //    _maskedText.SetRawValueDictionary(_rawValue);
            //     await _mask.ImplementMask(null);
            // }
            // // never update text directly. we already did it
            await base.SetValueAsync(value, updateText: false);
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            var text = Converter.Set(Value);
            if (Mask.Text==text)
                 return;
            Mask.SetText(text);
            await Update();
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
     
        internal void OnCaretPositionChanged(int pos)
        {
            Mask.Selection = null;
            Mask.CaretPos = pos;
            Console.WriteLine($"Caret position: {Mask}");
        }

        internal void OnCopy()
        {
            //Console.WriteLine($"Copy: {text}");
            var text = Text;
            _jsApiService.CopyToClipboardAsync(text);
        }

        internal async void OnPaste(string text)
        {
            //Console.WriteLine($"Paste: {text}");
            if (Text == null)
                return;
            Mask.Insert(text);
            await Update();
            //await SetValueAsync(Converter.Get(_mask.GetRawValueFromDictionary()), false);
        }

        public void OnSelect(int start, int end)
        {
            Mask.Selection = (start, end);
            Console.WriteLine($"Select: {Mask}");
        }

        internal void OnFocused(FocusEventArgs obj)
        {
             _isFocused = true;
        //     if (!string.IsNullOrEmpty(Converter.Set(Value)) || string.IsNullOrEmpty(Placeholder))
        //     {
        //         //This delay let click event fires first to set caret position proper (for only first click)
        //         await Task.Delay(1);
        //         await _mask.ImplementMask(null);
        //         SetCaretPosition(_mask.FindFirstCaretLocation());
        //     }
        }

        protected internal override void OnBlurred(FocusEventArgs obj)
        {
            base.OnBlurred(obj);
            // if (string.IsNullOrEmpty(_rawValue))
            // {
            //     SetTextAsync("", updateValue: false).AndForget();
            // }
            _isFocused = false;
        }

        public void SetCaretPosition(int caretPosition)
        {
            if (!_isFocused)
                return;
            _elementReference.SelectRangeAsync(caretPosition, caretPosition).AndForget();
            StateHasChanged();
        }
    }
}
