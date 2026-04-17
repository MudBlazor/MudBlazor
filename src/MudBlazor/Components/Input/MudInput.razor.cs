using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A component for collecting an input value.
    /// </summary>
    /// <typeparam name="T">The type of object managed by this input.</typeparam>
    public partial class MudInput<T> : MudBaseInput<T>
    {
        private string? _internalText;
        private string? _oldText = null;
        private bool _shouldInitSizing;
        private bool _shouldUpdateSizingParams;
        private bool _shouldAdjustSizingAfterRender;
        private ElementReference _elementReference1;
        private readonly Lazy<DotNetObjectReference<MudInput<T>>> _dotNetReferenceLazy;

        [DynamicDependency(nameof(CallOnBlurredAsync))]
        public MudInput()
        {
            _dotNetReferenceLazy = new Lazy<DotNetObjectReference<MudInput<T>>>(DotNetObjectReference.Create(this));
        }

        protected string Classname =>
            new CssBuilder(
                    MudInputCssHelper.GetClassname(this,
                        () => HasNativeHtmlPlaceholder() ||
                              !string.IsNullOrEmpty(ReadText) ||
                              Adornment == Adornment.Start ||
                              !string.IsNullOrWhiteSpace(Placeholder) ||
                              ShrinkLabel))
                .AddClass($"mud-input-sizing-{Sizing.ToStringFast(true)}")
                .Build();

        protected string InputClassname => MudInputCssHelper.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelper.GetAdornmentClassname(this);

        protected string ClearButtonClassname =>
            new CssBuilder("mud-input-clear-button")
                .AddClass("me-n1", Adornment == Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End && HideSpinButtons)
                .AddClass("me-6", Adornment != Adornment.End && HideSpinButtons == false)
                .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End && HideSpinButtons)
                .AddClass("mud-no-activator")
                .Build();

        protected internal override InputType GetInputType() => InputType;

        protected string InputTypeString => InputType.ToStringFast(true);

        /// <summary>
        /// The type of input collected by this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.  Represents a valid HTML5 input type.
        /// </remarks>
        [Parameter]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// The content within this input component.
        /// </summary>
        /// <remarks>
        /// Will only display if <see cref="InputType"/> is <see cref="InputType.Hidden"/>.
        /// </remarks>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The reference to the HTML element for this component.
        /// </summary>
        public ElementReference ElementReference { get; private set; }

        /// <summary>
        /// Occurs when the <c>Up</c> arrow button is clicked.
        /// </summary>
        /// <remarks>
        /// Only occurs when <see cref="InputType"/> is <see cref="InputType.Number"/>.  For numeric inputs, use the <see cref="MudNumericField{T}"/> component.
        /// </remarks>
        [Parameter]
        public EventCallback OnIncrement { get; set; }

        /// <summary>
        /// Occurs when the <c>Down</c> arrow button is clicked.
        /// </summary>
        /// <remarks>
        /// Only occurs when <see cref="InputType"/> is <see cref="InputType.Number"/>.  For numeric inputs, use the <see cref="MudNumericField{T}"/> component.
        /// </remarks>
        [Parameter]
        public EventCallback OnDecrement { get; set; }

        /// <summary>
        /// For <see cref="MudNumericField{T}"/>, hides the spin buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool HideSpinButtons { get; set; } = true;

        /// <summary>
        /// Shows a button to clear this input's value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        /// <remarks>
        /// When clicked, the <see cref="MudBaseInput{T}.Text"/> and <see cref="MudBaseInput{T}.Value"/> properties are reset.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Occurs when a mouse wheel event is raised.
        /// </summary>
        [Parameter]
        public EventCallback<WheelEventArgs> OnMouseWheel { get; set; }

        /// <summary>
        /// The icon to display when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// The icon to display for the <c>Up</c> arrow button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.KeyboardArrowUp"/>.
        /// </remarks>
        [Parameter]
        public string NumericUpIcon { get; set; } = Icons.Material.Filled.KeyboardArrowUp;

        /// <summary>
        /// The icon to display for the <c>Down</c> arrow button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.KeyboardArrowDown"/>.
        /// </remarks>
        [Parameter]
        public string NumericDownIcon { get; set; } = Icons.Material.Filled.KeyboardArrowDown;

        /// <summary>
        /// Defines the resizing behavior of this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputSizing.Fixed"/>.
        /// </remarks>
        [Parameter]
        public InputSizing Sizing { get; set; } = InputSizing.Fixed;

        /// <summary>
        /// The maximum vertical lines to display when <see cref="Sizing"/> is <see cref="InputSizing.Auto"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  When <c>0</c>. this property is ignored.
        /// </remarks>
        [Parameter]
        public int MaxLines { get; set; }

        /// <summary>
        /// Indicates whether the input should use a textarea element for dynamic sizing.
        /// </summary>
        private bool ShouldUseTextArea => Sizing != InputSizing.Fixed || Lines > 1;

        private Task OnInputOrOnChangeAsync(string? input) => Immediate ? OnInput(input) : OnChange(input);

        protected async Task OnInput(string? args)
        {
            _isFocused = true;
            _internalText = args;
            await OnInternalInputChanged.InvokeAsync(args);
            await SetTextAndUpdateValueAsync(args);
        }

        protected async Task OnChange(string? args)
        {
            _internalText = args;
            await OnInternalInputChanged.InvokeAsync(args);
            await SetTextAndUpdateValueAsync(args);
        }

        /// <summary>
        /// Paste hook for descendants.
        /// </summary>
        protected virtual Task OnPaste(ClipboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async ValueTask FocusAsync()
        {
            try
            {
                if (InputType == InputType.Hidden && ChildContent != null)
                    await _elementReference1.FocusAsync();
                else
                    await ElementReference.FocusAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($@"MudInput.FocusAsync: {e.Message}");
            }
        }

        /// <inheritdoc />
        public override ValueTask BlurAsync()
        {
            return ElementReference.MudBlurAsync();
        }

        /// <inheritdoc />
        public override ValueTask SelectAsync()
        {
            return ElementReference.MudSelectAsync();
        }

        /// <inheritdoc />
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return ElementReference.MudSelectRangeAsync(pos1, pos2);
        }

        private Size GetButtonSize() => Margin == Margin.Dense ? Size.Small : Size.Medium;

        /// <summary>
        /// Determine whether to show the clear button when Clearable==true.
        /// Of course the clear button won't show up if the text field is empty
        /// </summary>
        private bool ShowClearButton()
        {
            if (GetDisabledState())
            {
                return false;
            }

            if (!Clearable)
            {
                return false;
            }

            // If this is a standalone input it will not be clearable when read-only
            if (SubscribeToParentForm && GetReadOnlyState())
            {
                return false;
            }

            if (ReadValue is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            return ReadValue is not string and not null;
        }

        protected virtual async Task HandleClearButtonAsync(MouseEventArgs e)
        {
            await SetTextAndUpdateValueAsync(string.Empty, updateValue: true);
            await ElementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        protected virtual async Task HandleSpinButtonPointerDownAsync()
        {
            await ElementReference.FocusAsync();
        }

        private readonly record struct AutoSizingVisualState(
            Variant Variant,
            Margin Margin,
            Typo Typo,
            Adornment Adornment,
            string? Class,
            string? Style,
            bool Disabled);

        private AutoSizingVisualState CaptureAutoSizingVisualState()
            => new(Variant, Margin, Typo, Adornment, Class, Style, GetDisabledState());

        private void ResetAutoSizingFlags()
        {
            _shouldInitSizing = false;
            _shouldUpdateSizingParams = false;
            _shouldAdjustSizingAfterRender = false;
        }

        private void SyncAutoSizingTextSnapshot()
        {
            _oldText = _internalText;
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            // Visual/style-affecting changes.
            var oldVisualState = CaptureAutoSizingVisualState();

            // Handled separately because they drive different lifecycle actions.
            var oldLines = Lines;
            var oldMaxLines = MaxLines;
            var oldSizing = Sizing;

            await base.SetParametersAsync(parameters);

            var newSizing = Sizing;
            var hasAutoSizingVisualChange = oldVisualState != CaptureAutoSizingVisualState();
            var hasAutoSizingParameterChange = oldLines != Lines || oldMaxLines != MaxLines || oldSizing != newSizing;

            // Always update internal text (TextUpdateSuppression removed)
            _internalText = ReadText;

            if (oldSizing == InputSizing.Fixed && newSizing != InputSizing.Fixed)
            {
                _shouldInitSizing = true;
            }

            if (newSizing != InputSizing.Fixed && !_shouldInitSizing && hasAutoSizingVisualChange)
            {
                // Re-measure after style/class-related updates because runtime classes and computed styles can affect textarea metrics.
                _shouldAdjustSizingAfterRender = true;
            }

            if (oldSizing != InputSizing.Fixed && newSizing == InputSizing.Fixed)
            {
                // Disable dynamic sizing.
                ResetAutoSizingFlags();
                if (IsJSRuntimeAvailable)
                {
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputSizing.destroy", ElementReference);
                }
            }
            else if (newSizing != InputSizing.Fixed && !_shouldInitSizing && hasAutoSizingParameterChange)
            {
                // Defer until OnAfterRender so measurements use the latest DOM/classes.
                _shouldUpdateSizingParams = true;
            }
        }

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Sizing != InputSizing.Fixed)
            {
                if (firstRender || _shouldInitSizing)
                {
                    ResetAutoSizingFlags();
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputSizing.init", ElementReference, MaxLines);
                    SyncAutoSizingTextSnapshot();
                }
                else if (_shouldUpdateSizingParams)
                {
                    _shouldUpdateSizingParams = false;
                    _shouldAdjustSizingAfterRender = false;
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputSizing.updateParams", ElementReference, MaxLines);
                    SyncAutoSizingTextSnapshot();
                }
                else if (_shouldAdjustSizingAfterRender || _oldText != _internalText)
                {
                    _shouldAdjustSizingAfterRender = false;
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputSizing.adjustHeight", ElementReference);
                    SyncAutoSizingTextSnapshot();
                }
            }
            if (firstRender)
            {
                // add onblur event through javascript which will trigger CallOnBlurredAsync
                // must do in javascript or it won't detect ios Keyboard button - limitation of Blazor/React/other frameworks of the DOM
                await ElementReference.MudAttachBlurEventWithJS(_dotNetReferenceLazy.Value);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Set the <see cref="MudBaseInput{T}.Text"/> to the specified value.
        /// </summary>
        /// <param name="text">The new value.</param>
        public Task SetText(string? text)
        {
            return SetText(text, updateValue: true);
        }

        internal Task SetText(string? text, bool updateValue)
        {
            _internalText = text;
            return SetTextAndUpdateValueAsync(text, updateValue);
        }

        // Certain HTML5 inputs (dates and color) have a native placeholder
        private bool HasNativeHtmlPlaceholder()
        {
            return GetInputType()
                is InputType.Color
                or InputType.Date
                or InputType.DateTimeLocal
                or InputType.Month
                or InputType.Time
                or InputType.Week;
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            if (IsJSRuntimeAvailable)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudElementRef.removeOnBlurEvent", ElementReference);
                if (Sizing != InputSizing.Fixed)
                {
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputSizing.destroy", ElementReference);
                }
            }

            if (_dotNetReferenceLazy.IsValueCreated)
            {
                _dotNetReferenceLazy.Value.Dispose();
            }

            await base.DisposeAsyncCore();
        }

        [JSInvokable]
        public async Task CallOnBlurredAsync()
        {
            // If onblurred already fired then cancel
            if (!_isFocused)
                return;

            await OnBlurredAsync(new FocusEventArgs { Type = "jsBlur.OnBlur" });
        }
    }

    /// <summary>
    /// An input component for collecting alphanumeric values.
    /// </summary>
    public class MudInputString : MudInput<string> { }
}
