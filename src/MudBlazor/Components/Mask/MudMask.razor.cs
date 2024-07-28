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
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMask : MudBaseInput<string>
    {
        public MudMask()
        {
            TextUpdateSuppression = false;
        }

        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", when: () => Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => Underline && Variant != Variant.Outlined)
                .AddClass("mud-shrink",
                    when: () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start ||
                                !string.IsNullOrWhiteSpace(Placeholder))
                .AddClass("mud-disabled", GetDisabledState())
                .AddClass("mud-input-error", HasErrors)
                .AddClass("mud-ltr", GetInputType() == InputType.Email || GetInputType() == InputType.Telephone)
                .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
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
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .AddClass(Class)
                .Build();

        protected string ClearButtonClassname =>
            new CssBuilder()
                // .AddClass("me-n1", Adornment == Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End)
                // .AddClass("me-6", Adornment != Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End)
                .Build();


        private ElementReference _elementReference;
        private ElementReference _elementReference1;
        private IJsEvent _jsEvent;
        private IKeyInterceptor _keyInterceptor;

        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

        [Inject] private IJsEventFactory _jsEventFactory { get; set; }
        [Inject] private IJsApiService _jsApiService { get; set; }

        private string _elementId = "mask_" + Guid.NewGuid().ToString().Substring(0, 8);

        private IMask _mask = new PatternMask("** **-** **");

        /// <summary>
        /// ChildContent will only be displayed if InputType.Hidden and if its not null. Required for Select
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Provide a masking object. Built-in masks are PatternMask, MultiMask, RegexMask and BlockMask
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask Mask
        {
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

        private bool _showClearable;

        private void UpdateClearable(object value)
        {
            var showClearable = Clearable && !string.IsNullOrWhiteSpace(Text);
            if (_showClearable != showClearable)
                _showClearable = showClearable;
        }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Custom clear icon when <see cref="Clearable"/> is enabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        protected override async Task OnInitializedAsync()
        {
            if (Text != Mask.Text)
                await SetTextAsync(Mask.Text, updateValue: false);
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsEvent = _jsEventFactory.Create();

                await _jsEvent.Connect(_elementId,
                    new JsEventOptions
                    {
                        //EnableLogging = true,
                        TargetClass = "mud-input-slot",
                        TagName = "INPUT"
                    });
                _jsEvent.CaretPositionChanged += OnCaretPositionChanged;
                _jsEvent.Paste += OnPaste;
                _jsEvent.Select += OnSelect;

                _keyInterceptor = _keyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys =
                    {
                        new KeyOptions
                        {
                            Key = " ", PreventDown = "key+none"
                        }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key = "ArrowUp", PreventDown = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key = "ArrowDown", PreventDown = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key = "PageUp", PreventDown = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key = "PageDown", PreventDown = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key = @"/^.$/", PreventDown = "key+none|key+shift" },
                        new KeyOptions { Key = "/./", SubscribeDown = true },
                        new KeyOptions { Key = "Backspace", PreventDown = "key+none" },
                        new KeyOptions { Key = "Delete", PreventDown = "key+none" },
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDownInternally;
            }

            if (_isFocused && Mask.Selection == null)
                await SetCaretPositionAsync(Mask.CaretPos, _selection, render: false);
            await base.OnAfterRenderAsync(firstRender);
        }

        private async void HandleKeyDownInternally(KeyboardEventArgs args)
        {
            await HandleKeyDown(args);
        }

        protected internal async Task HandleKeyDown(KeyboardEventArgs e)
        {
            try
            {
                if ((e.CtrlKey && e.Key != "Backspace") || e.AltKey || GetReadOnlyState())
                    return;
                switch (e.Key)
                {
                    case "Backspace":
                        if (e.CtrlKey)
                        {
                            Mask.Clear();
                            await UpdateAsync();
                            return;
                        }
                        Mask.Backspace();
                        await UpdateAsync();
                        return;
                    case "Delete":
                        Mask.Delete();
                        await UpdateAsync();
                        return;
                }

                if (ValidCharacterRegularExpression().IsMatch(e.Key))
                {
                    Mask.Insert(e.Key);
                    await UpdateAsync();
                }
            }
            finally
            {
                // call user callback
                await OnKeyDown.InvokeAsync(e);
            }
        }

        private bool _updating;

        private async Task UpdateAsync()
        {
            var caret = Mask.CaretPos;
            var selection = Mask.Selection;
            var text = Mask.Text;
            var cleanText = Mask.GetCleanText();
            _updating = true;
            try
            {
                await base.SetTextAsync(text, updateValue: false);
                if (Clearable)
                    UpdateClearable(Text);
                var v = Converter.Get(cleanText);
                Value = v;
                await ValueChanged.InvokeAsync(v);
                await SetCaretPositionAsync(caret, selection);
            }
            finally
            {
                _updating = false;
            }
        }

        internal async Task HandleClearButtonAsync(MouseEventArgs e)
        {
            Mask.Clear();
            await UpdateAsync();
            await _elementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            // allow this only via changes from the outside
            if (_updating)
                return;
            var text = Converter.Set(Value);
            var cleanText = Mask.GetCleanText();
            if (cleanText == text || string.IsNullOrEmpty(cleanText) && string.IsNullOrEmpty(text))
                return;
            var maskText = Mask.Text;
            Mask.SetText(text);
            if (maskText == Mask.Text)
                return; // no change, stop update loop
            await UpdateAsync();
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            // allow this only via changes from the outside
            if (_updating)
                return;
            var text = Text;
            if (Mask.Text == text)
                return;
            var maskText = Mask.Text;
            Mask.SetText(text);
            if (maskText == Mask.Text)
                return; // no change, stop update loop
            await UpdateAsync();
        }

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null
            ? string.Empty
            : (Counter == 0
                ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}")
                : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Clear the text field. 
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            Mask.Clear();
            return UpdateAsync();
        }

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.MudSelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.MudSelectRangeAsync(pos1, pos2);
        }

        internal void OnCopy()
        {
            var text = Text;
            if (Mask.Selection != null)
            {
                (_, text, _) = BaseMask.SplitSelection(text, Mask.Selection.Value);
            }
            _jsApiService.CopyToClipboardAsync(text);
        }

        internal async void OnPaste(string text)
        {
            if (text == null || GetReadOnlyState())
                return;
            Mask.Insert(text);
            await UpdateAsync();
        }

        public void OnSelect(int start, int end)
        {
            Mask.Selection = _selection = (start, end);
        }

        internal void OnFocused(FocusEventArgs obj)
        {
            _isFocused = true;
        }

        protected internal override async Task OnBlurredAsync(FocusEventArgs obj)
        {
            await base.OnBlurredAsync(obj);
            _isFocused = false;
        }

        private int _caret;
        private (int, int)? _selection;

        private async Task SetCaretPositionAsync(int caret, (int, int)? selection = null, bool render = true)
        {
            if (!_isFocused)
                return;
            _caret = caret;
            if (caret == 0)
                _caret = 0;
            _selection = selection;
            if (selection == null)
            {
                await _elementReference.MudSelectRangeAsync(caret, caret);
            }
            else
            {
                var sel = selection.Value;
                await _elementReference.MudSelectRangeAsync(sel.Item1, sel.Item2);
            }
        }

        // from JS event     
        internal void OnCaretPositionChanged(int pos)
        {
            if (Mask.Selection != null)
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
        }

        private void SetMask(IMask other)
        {
            if (other == null)
            {
                // warn the user that the mask parameter is missing
                _mask = new PatternMask("null ********");
                return;
            }

            if (_mask.GetType() == other.GetType())
            {
                // update mask while retaining current state
                _mask.UpdateFrom(other);
                return;
            }

            // swap masks while retaining text
            // note: this is required for `BaseMask` instances other than `PatternMask` to work as expected
            other.SetText(Text);
            _mask = other;
        }

        private async void OnCut(ClipboardEventArgs obj)
        {
            if (GetReadOnlyState())
                return;

            if (_selection != null)
                Mask.Delete();
            await UpdateAsync();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDownInternally;
                }

                if (IsJSRuntimeAvailable)
                {
                    _jsEvent?.Dispose();
                    _keyInterceptor?.Dispose();
                }
            }
        }

        [GeneratedRegex(@"^.$")]
        private static partial Regex ValidCharacterRegularExpression();
    }
}
