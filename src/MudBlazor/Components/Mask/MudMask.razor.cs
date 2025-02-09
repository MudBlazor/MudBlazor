// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A text input which conforms user input to a specific format while typing.
    /// <remarks>
    /// Note that MudMask is recommended to be used in WASM projects only because it has known problems
    /// in BSS, especially with high network latency.
    /// </remarks>
    /// </summary>
    public partial class MudMask : MudBaseInput<string>
    {
        private int _caret;
        private bool _updating;
        private IJsEvent? _jsEvent;
        private bool _showClearable;
        private (int, int)? _selection;
        private ElementReference _elementReference;
        private ElementReference _elementReference1;
        private IMask _mask = new PatternMask("** **-** **");
        private string _elementId = Identifier.Create("mask");

        protected string Classname =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass($"mud-input-adorned-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}", () => Margin != Margin.None)
                .AddClass("mud-input-underline", () => Underline && Variant != Variant.Outlined)
                .AddClass("mud-shrink", () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(Placeholder) || ShrinkLabel)
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
                .AddClass($"mud-input-root-margin-{Margin.ToDescriptionString()}", () => Margin != Margin.None)
                .AddClass(Class)
                .Build();

        protected string AdornmentClassname =>
            new CssBuilder()
                .AddClass($"mud-input-adornment-{Adornment.ToDescriptionString()}", Adornment != Adornment.None)
                .AddClass($"mud-text", !string.IsNullOrEmpty(AdornmentText))
                .AddClass($"mud-input-root-filled-shrink", Variant == Variant.Filled)
                .AddClass(Class)
                .Build();

        protected string ClearButtonClassname =>
            new CssBuilder("mud-input-clear-button")
                // .AddClass("me-n1", Adornment == Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End)
                // .AddClass("me-6", Adornment != Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End)
                .Build();

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [Inject]
        private IJsEventFactory JsEventFactory { get; set; } = null!;

        [Inject]
        private IJsApiService JsApiService { get; set; } = null!;

        /// <summary>
        /// The content within this input.
        /// </summary>
        /// <remarks>
        /// Only displays when <see cref="InputType"/> is <see cref="InputType.Hidden"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The mask to apply to text values.
        /// </summary>
        /// <remarks>
        /// Typically set to common masks such as <see cref="PatternMask"/>, <see cref="MultiMask"/>, <see cref="RegexMask"/>, and <see cref="BlockMask"/>.
        /// When set, some properties will be ignored such as <see cref="MudInput{T}.MaxLines"/>, <see cref="MudInput{T}.AutoGrow"/>, and <see cref="MudInput{T}.HideSpinButtons"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask Mask
        {
            get => _mask;
            set => SetMask(value);
        }

        /// <summary>
        /// The type of the underlying input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Shows the clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// The icon displayed when <see cref="Clearable" /> is <c>true</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        public MudMask()
        {
            TextUpdateSuppression = false;
        }

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
                _jsEvent = JsEventFactory.Create();

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

                var options = new KeyInterceptorOptions(
                    "mud-input-slot",
                    [
                        // prevent scrolling page, toggle open/close
                        new(" ", preventDown: "key+none"),
                        // prevent scrolling page, instead increment
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead decrement
                        new("ArrowDown", preventDown: "key+none"),
                        // prevent scrolling page
                        new("PageUp", preventDown: "key+none"),
                        // prevent scrolling page
                        new("PageDown", preventDown: "key+none"),
                        // prevent input of all other characters except allowed, like [0-9.,-+]
                        new(@"/^.$/", preventDown: "key+none|key+shift"),
                        // subscribe to all key down events
                        new("/./", subscribeDown: true),
                        // prevent backspace key
                        new("Backspace", preventDown: "key+none"),
                        // prevent delete key
                        new("Delete", preventDown: "key+none")
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDown);
            }

            if (_isFocused && Mask.Selection == null)
                await SetCaretPositionAsync(Mask.CaretPos, _selection, render: false);
            await base.OnAfterRenderAsync(firstRender);
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

        private void UpdateClearable()
        {
            var showClearable = Clearable && !string.IsNullOrWhiteSpace(Text);

            if (_showClearable != showClearable)
            {
                _showClearable = showClearable;
                StateHasChanged();
            }
        }

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
                    UpdateClearable();
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
            if (string.IsNullOrEmpty(cleanText) && string.IsNullOrEmpty(text))
                return;

            if (cleanText != text)
            {
                var maskText = Mask.Text;
                Mask.SetText(text);
                if (maskText == Mask.Text)
                    return;
            }

            if (Text != Mask.Text)
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

        private string GetCounterText() => Counter switch
        {
            null => string.Empty,
            0 => string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}",
            _ => (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"
        };

        private bool ShowClearButton()
        {
            if (SubscribeToParentForm)
                return _showClearable && !GetReadOnlyState() && !GetDisabledState();
            return _showClearable && !GetDisabledState();
        }

        /// <summary>
        /// Clears the text and value for this input.
        /// </summary>
        public Task Clear()
        {
            Mask.Clear();
            return UpdateAsync();
        }

        /// <summary>
        /// Sets the cursor to this input.
        /// </summary>
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <summary>
        /// Selects the text in this input.
        /// </summary>
        public override ValueTask SelectAsync()
        {
            return _elementReference.MudSelectAsync();
        }

        /// <summary>
        /// Selects a range of characters in this input.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
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

            JsApiService.CopyToClipboardAsync(text);
        }

        internal async void OnPaste(string? text)
        {
            if (text == null || GetReadOnlyState())
                return;

            Mask.Insert(text);
            await UpdateAsync();
        }

        /// <summary>
        /// Occurs when the selected characters have changed.
        /// </summary>
        /// <param name="start">The index of the first selected character.</param>
        /// <param name="end">The index of the last selected character.</param>
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

        private void SetMask(IMask? other)
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

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
                if (_jsEvent is not null)
                {
                    _jsEvent.CaretPositionChanged -= OnCaretPositionChanged;
                    _jsEvent.Paste -= OnPaste;
                    _jsEvent.Select -= OnSelect;
                    await _jsEvent.DisposeAsync();
                }
            }
        }

        [GeneratedRegex(@"^.$")]
        private static partial Regex ValidCharacterRegularExpression();
    }
}
