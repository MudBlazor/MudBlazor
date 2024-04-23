using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
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
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        protected virtual async Task OnPaste(ClipboardEventArgs args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // do nothing
            return;
        }

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public ElementReference ElementReference { get; private set; }
        private ElementReference _elementReference1;

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

        public override ValueTask BlurAsync()
        {
            return ElementReference.MudBlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return ElementReference.MudSelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return ElementReference.MudSelectRangeAsync(pos1, pos2);
        }

        /// <summary>
        /// Invokes the callback when the Up arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnIncrement { get; set; }

        /// <summary>
        /// Invokes the callback when the Down arrow button is clicked when the input is set to <see cref="InputType.Number"/>.
        /// Note: use the optimized control <see cref="MudNumericField{T}"/> if you need to deal with numbers.
        /// </summary>
        [Parameter] public EventCallback OnDecrement { get; set; }

        /// <summary>
        /// Hides the spin buttons for <see cref="MudNumericField{T}"/>
        /// </summary>
        [Parameter] public bool HideSpinButtons { get; set; } = true;

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Mouse wheel event for input.
        /// </summary>
        [Parameter] public EventCallback<WheelEventArgs> OnMouseWheel { get; set; }

        /// <summary>
        /// Custom clear icon.
        /// </summary>
        [Parameter] public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Custom numeric up icon.
        /// </summary>
        [Parameter] public string NumericUpIcon { get; set; } = Icons.Material.Filled.KeyboardArrowUp;

        /// <summary>
        /// Custom numeric down icon.
        /// </summary>
        [Parameter] public string NumericDownIcon { get; set; } = Icons.Material.Filled.KeyboardArrowDown;

        /// <summary>
        /// If true the input element will grow automatically with the text.
        /// </summary>
        [Parameter] public bool AutoGrow { get; set; }

        /// <summary>
        /// If AutoGrow is set to true, the input element will not grow bigger than MaxLines lines. If MaxLines is set to 0
        /// or less, the property will be ignored.
        /// </summary>
        [Parameter] public int MaxLines { get; set; }

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
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        public async ValueTask DisposeAsync()
        {
            if (AutoGrow && IsJSRuntimeAvailable)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInputAutoGrow.destroy", ElementReference);
            }
        }
    }

    public class MudInputString : MudInput<string> { }
}
