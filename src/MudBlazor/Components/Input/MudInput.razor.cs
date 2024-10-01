using System;
using System.Threading.Tasks;
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
    public partial class MudInput<T> : MudBaseInput<T>, IAsyncDisposable
    {
        protected string Classname =>
           new CssBuilder(
               MudInputCssHelper.GetClassname(this,
                   () => HasNativeHtmlPlaceholder() || !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(Placeholder) || ShrinkLabel))
            .AddClass("mud-input-auto-grow", when: () => AutoGrow)
            .Build();

        protected string InputClassname => MudInputCssHelper.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelper.GetAdornmentClassname(this);

        protected string ClearButtonClassname =>
                    new CssBuilder("mud-input-clear-button")
                    .AddClass("me-n1", Adornment == Adornment.End && HideSpinButtons == false)
                    .AddClass("mud-icon-button-edge-end", Adornment == Adornment.End && HideSpinButtons)
                    .AddClass("me-6", Adornment != Adornment.End && HideSpinButtons == false)
                    .AddClass("mud-icon-button-edge-margin-end", Adornment != Adornment.End && HideSpinButtons)
                    .Build();

        /// <summary>
        /// The type of input collected by this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.  Represents a valid HTML5 input type.
        /// </remarks>
        [Parameter]
        public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        protected string InputTypeString => InputType.ToDescriptionString();

        protected Task OnInput(ChangeEventArgs args)
        {
            if (!Immediate)
                return Task.CompletedTask;
            _isFocused = true;
            return SetTextAsync(args?.Value as string);
        }

        protected async Task OnChange(ChangeEventArgs args)
        {
            _internalText = args?.Value as string;
            await OnInternalInputChanged.InvokeAsync(args);
            if (!Immediate)
            {
                await SetTextAsync(args?.Value as string);
            }
        }

        /// <summary>
        /// Paste hook for descendants.
        /// </summary>
        protected virtual Task OnPaste(ClipboardEventArgs args)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// The content within this input component.
        /// </summary>
        /// <remarks>
        /// Will only display if <see cref="InputType"/> is <see cref="InputType.Hidden"/>.
        /// </remarks>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The reference to the HTML element for this component.
        /// </summary>
        public ElementReference ElementReference { get; private set; }

        private ElementReference _elementReference1;

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
                Console.WriteLine("MudInput.FocusAsync: " + e.Message);
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
        /// Stretches this input vertically to accommodate the <see cref="MudBaseInput{T}.Text"/> value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool AutoGrow { get; set; }

        /// <summary>
        /// The maximum vertical lines to display when <see cref="AutoGrow"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  When <c>0</c>. this property is ignored.
        /// </remarks>
        [Parameter]
        public int MaxLines { get; set; }

        private Size GetButtonSize() => Margin == Margin.Dense ? Size.Small : Size.Medium;

        /// <summary>
        /// If true, Clearable is true and there is a non null value (non-string for string values)
        /// </summary>
        private bool GetClearable() => Clearable && ((Value is string stringValue && !string.IsNullOrWhiteSpace(stringValue)) || (Value is not string && Value is not null));

        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, updateValue: true);
            await ElementReference.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        private string _oldText = null;
        private string _internalText;
        private bool _shouldInitAutoGrow;

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var oldLines = Lines;
            var oldMaxLines = MaxLines;
            var oldAutoGrow = AutoGrow;

            await base.SetParametersAsync(parameters);

            //if (!_isFocused || _forceTextUpdate)
            //    _internalText = Text;
            if (RuntimeLocation.IsServerSide && TextUpdateSuppression)
            {
                // Text update suppression, only in BSS (not in WASM).
                // This is a fix for #1012
                if (!_isFocused || _forceTextUpdate)
                    _internalText = Text;
            }
            else
            {
                // in WASM (or in BSS with TextUpdateSuppression==false) we always update
                _internalText = Text;
            }

            // Flag AutoGrow to be initialized on the next render.
            if (!oldAutoGrow && AutoGrow)
            {
                _shouldInitAutoGrow = true;
            }

            if (IsJSRuntimeAvailable)
            {
                if (oldAutoGrow && !AutoGrow)
                {
                    // Disable AutoGrow.
                    _shouldInitAutoGrow = false;
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.destroy", ElementReference);
                }
                else if (oldLines != Lines || oldMaxLines != MaxLines)
                {
                    if (AutoGrow && !_shouldInitAutoGrow)
                    {
                        // Update AutoGrow parameters (if it was already enabled).
                        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.updateParams", ElementReference, MaxLines);
                    }
                }
            }
        }

        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (AutoGrow)
            {
                if (firstRender || _shouldInitAutoGrow)
                {
                    _shouldInitAutoGrow = false;
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.initAutoGrow", ElementReference, MaxLines);
                    _oldText = _internalText;
                }
                else if (_oldText != _internalText)
                {
                    await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.adjustHeight", ElementReference);
                    _oldText = _internalText;
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Set the <see cref="MudBaseInput{T}.Text"/> to the specified value.
        /// </summary>
        /// <param name="text">The new value.</param>
        public Task SetText(string text)
        {
            _internalText = text;
            return SetTextAsync(text);
        }

        // Certain HTML5 inputs (dates and color) have a native placeholder
        private bool HasNativeHtmlPlaceholder()
        {
            return GetInputType() is InputType.Color or InputType.Date or InputType.DateTimeLocal or InputType.Month
                or InputType.Time or InputType.Week;
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (AutoGrow && IsJSRuntimeAvailable)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.destroy", ElementReference);
            }
        }
    }

    /// <summary>
    /// An input component for collecting alphanumeric values.
    /// </summary>
    public class MudInputString : MudInput<string> { }
}
