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

        private BaseMask _mask = new SimpleMask("aaaa-0000");
        
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public BaseMask Mask { 
            get => _mask;
            set => SetMask(value);
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
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page
                        // new KeyOptions { Key="ArrowLeft", PreventDown = "key+none" }, // implement selection
                        // new KeyOptions { Key="ArrowRight", PreventDown = "key+none" },  // implement selection
                        // new KeyOptions { Key="Home", PreventDown = "key+none" },  // implement selection
                        // new KeyOptions { Key="End", PreventDown = "key+none" },  // implement selection
                        new KeyOptions { Key="PageUp", PreventDown = "key+none" },
                        new KeyOptions { Key="PageDown", PreventDown = "key+none" },
                        // new KeyOptions { Key="Escape" },
                        new KeyOptions { Key=@"/^.$/", PreventDown = "key+none|key+shift" },
                        //new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        //new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="/./", SubscribeDown = true },
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key="Delete", PreventDown = "key+none" },
                        //new KeyOptions { Key="Shift", PreventDown = "key+none" },
                    },
                });
                _keyInterceptor.KeyDown += e => HandleKeyDown(e).AndForget();
            }
            if (_isFocused && Mask.Selection == null)
                 SetCaretPosition(_caret, _selection, render:false);
            await base.OnAfterRenderAsync(firstRender);
        }
           
        protected internal async Task HandleKeyDown(KeyboardEventArgs e)
        {
            try
            {
                if (e.CtrlKey || e.AltKey)
                    return;
                Console.WriteLine($"HandleKeyDown: '{e.Key}'");
                switch (e.Key)
                {
                    case "Backspace":
                        Mask.Backspace();
                        await Update();
                        return;
                    case "Delete":
                        Mask.Delete();
                        await Update();
                        return;
                    // case "ArrowLeft":
                    //     if (e.ShiftKey)
                    //         ChangeSelection(-1);
                    //     else
                    //         Mask.CaretPos -= 1;
                    //     SetCaretPosition(Mask.CaretPos, Mask.Selection);
                    //     return;
                    // case "ArrowRight":
                    //     if (e.ShiftKey)
                    //         ChangeSelection(1);
                    //     else
                    //         Mask.CaretPos += 1;
                    //     SetCaretPosition(Mask.CaretPos, Mask.Selection);
                    //     return;
                }
                if (Regex.IsMatch(e.Key, @"^.$"))
                {
                    Mask.Insert(e.Key);
                    Console.WriteLine(Mask.ToString());
                    await Update();
                }
            }
            finally
            {
                // call user callback
                await OnKeyDown.InvokeAsync(e);
            }
        }
        
        // private void ChangeSelection(int direction)
        // {
        //     var sel = Mask.Selection ?? (Mask.CaretPos, Mask.CaretPos);
        //     if (direction < 0)
        //         Mask.Selection = (sel.Item1 + direction, sel.Item2);
        //     else
        //         Mask.Selection = (sel.Item1, sel.Item2+direction);
        // }

        private bool _updating;
        
        private async Task Update()
        {
            var caret = Mask.CaretPos;
            var selection = Mask.Selection;
            var text = Mask.Text;
            var cleanText = Mask.CleanText;
            _updating = true;
            try
            {
                await base.SetTextAsync(text, updateValue: false);
                var v=Converter.Get(cleanText);
                Console.WriteLine("####### Value: " + v);
                Value = v;
                await ValueChanged.InvokeAsync(v);
                //await base.SetValueAsync(Converter.Get(cleanText), updateText: false);
                SetCaretPosition(caret, selection);
            }
            finally
            {
                _updating = false;
            }
        }

        private async void HandleClearButton(MouseEventArgs e)
        {
            Mask.Clear();
            await Update();
            await OnClearButtonClick.InvokeAsync(e);
        }
        
        // private Task SetValue(string text)
        // {
        //     return SetValueAsync(Converter.Get(text), updateText: false);
        // }

        protected override async Task SetTextAsync(string text, bool updateValue = true)
        {
            if (_updating)
                return;
            Console.WriteLine($"SetTextAsync: '{text}' updateValue:{updateValue}");
            if (Mask.Text != text)
            {
                Console.WriteLine("########### SETTING Mask Text");
                Mask.SetText(text);
            }
            await base.SetTextAsync(text, updateValue);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            await base.SetValueAsync(value, updateText: false);
            //return Task.CompletedTask;
        }

        protected override Task UpdateTextPropertyAsync(bool updateValue)
        {
            // this causes problems with cursor:
           
            // // allow this only fia changes from the outside
            // if (_settingText)
            //     return;
            // var text = Converter.Set(Value);
            // if (Mask.Text==text)
            //      return;
            // Mask.SetText(text);
            // await Update();
            
            return Task.CompletedTask;
        }

        protected override Task UpdateValuePropertyAsync(bool updateText)
        {
            return Task.CompletedTask;
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

        private int _caret;
        private (int, int)? _selection;

        public void SetCaretPosition(int caret, (int, int)? selection, bool render=true)
        {
            if (!_isFocused)
                return;
            _caret = caret;
            if (caret == 0)
                _caret = 0;
            _selection = selection;
            if (selection == null)
            {
                Console.WriteLine("#Setting Caret Position: " + caret);
                _elementReference.SelectRangeAsync(caret, caret).AndForget();
            }
            else
            {
                var sel = selection.Value;
                Console.WriteLine($"#Setting Selection: ({sel.Item1}..{sel.Item2})");
                _elementReference.SelectRangeAsync(sel.Item1, sel.Item2).AndForget();
            }
            if(render)
                StateHasChanged();
        }
        
        // from JS event     
        internal void OnCaretPositionChanged(int pos)
        {
            if (Mask.Selection!=null)
            {
                // do not clear selection if pos change is at selection border
                var sel = Mask.Selection.Value;
                if (pos == sel.Item1 || pos == sel.Item2)
                    return;
            }
            if (pos == Mask.CaretPos)
                return;
            Mask.Selection = null;
            Mask.CaretPos = pos;
            Console.WriteLine($"On Caret position change: '{Mask}' ({pos})");
        }

        private void SetMask(BaseMask other)
        {
            if (_mask == null || other == null || _mask?.GetType() != other?.GetType())
            {
                _mask = other;
                if (_mask == null)
                    _mask = new SimpleMask("aaaa-0000"); // maybe have some kind of NullMask with Text "No mask configured"?
                return;
            } 
            // set new mask properties without loosing state
            _mask.UpdateFrom(other);
        }
    }
}
