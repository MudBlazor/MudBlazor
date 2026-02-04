using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// An input for collecting text values.
    /// </summary>
    /// <typeparam name="T">The type of object managed by this input.</typeparam>
    public partial class MudTextField<T> : MudDebouncedInput<T>
    {
        private IMask? _mask;
        private MudMask? _maskReference;

        protected string Classname =>
            new CssBuilder("mud-input-input-control")
                .AddClass($"mud-input-sizing-{Sizing.ToStringFast(true)}")
                .AddClass(Class)
                .Build();

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        /// <summary>
        /// The reference to the underlying <see cref="MudInput{T}"/> component.
        /// </summary>
        public MudInput<string>? InputReference { get; private set; }

        /// <summary>
        /// The type of input collected by this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.  Represents a valid HTML5 input type.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        /// <summary>
        /// Shows a button to clear this input's value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; }

        /// <summary>
        /// The icon to display when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Occurs when the clear button is clicked.
        /// </summary>
        /// <remarks>
        /// When clicked, the <see cref="MudBaseInput{T}.Text"/> and <see cref="MudBaseInput{T}.Value"/> properties are reset.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// The mask to apply to text values.
        /// </summary>
        /// <remarks>
        /// Typically set to common masks such as <see cref="PatternMask"/>, <see cref="MultiMask"/>, <see cref="RegexMask"/>, and <see cref="BlockMask"/>.
        /// When set, some properties will be ignored such as <see cref="MudInput{T}.MaxLines"/>, <see cref="MudInput{T}.Sizing"/>, and <see cref="MudInput{T}.HideSpinButtons"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask? Mask
        {
            get => _maskReference?.Mask ?? _mask; // this might look strange, but it is absolutely necessary due to how MudMask works.
            set => _mask = value;
        }

        /// <summary>
        /// Defines the resizing behavior of this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputSizing.Fixed"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public InputSizing Sizing { get; set; } = InputSizing.Fixed;

        /// <summary>
        /// The maximum vertical lines to display when <see cref="Sizing"/> is <see cref="InputSizing.Auto"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  When <c>0</c>. this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public int MaxLines { get; set; }

        [MemberNotNullWhen(false, nameof(InputReference))]
        [MemberNotNullWhen(true, nameof(_mask), nameof(Mask), nameof(_maskReference))]
        private bool HasMask => _mask is not null;

        /// <inheritdoc />
        public override ValueTask FocusAsync()
        {
            if (!HasMask)
            {
                return InputReference.FocusAsync();
            }

            return _maskReference.FocusAsync();
        }

        /// <inheritdoc />
        public override ValueTask BlurAsync()
        {
            if (!HasMask)
            {
                return InputReference.BlurAsync();
            }

            return _maskReference.BlurAsync();
        }

        /// <inheritdoc />
        public override ValueTask SelectAsync()
        {
            if (!HasMask)
            {
                return InputReference.SelectAsync();
            }

            return _maskReference.SelectAsync();
        }

        /// <inheritdoc />
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            if (!HasMask)
            {
                return InputReference.SelectRangeAsync(pos1, pos2);
            }

            return _maskReference.SelectRangeAsync(pos1, pos2);
        }

        /// <inheritdoc />
        protected override async Task ResetValueAsync()
        {
            if (!HasMask)
            {
                await InputReference.ResetAsync();
            }
            else
            {
                await _maskReference.ResetAsync();
            }

            await base.ResetValueAsync();
        }

        /// <summary>
        /// Clears the <see cref="MudBaseInput{T}.Text"/> and sets <see cref="MudBaseInput{T}.Value"/> to <c>default(T)</c>.
        /// </summary>
        public Task ClearAsync()
        {
            if (!HasMask)
            {
                return InputReference.SetText(null);
            }

            return _maskReference.Clear();
        }

        /// <summary>
        /// Sets the <see cref="MudBaseInput{T}.Text"/> to the specified value.
        /// </summary>
        /// <param name="text">The new text value to use.</param>
        public async Task SetTextAsync(string? text)
        {
            if (!HasMask)
            {
                await InputReference.SetText(text);
                return;
            }

            await _maskReference.Clear();
            await _maskReference.OnPasteAsync(text);
        }

        /// <summary>
        /// Returns the current caret position.
        /// </summary>
        /// <remarks>
        /// Returns the text length if called and this field hasn't been focused yet.
        /// Returns <c>-1</c> if called before this component has been rendered.
        /// </remarks>
        public async Task<int> GetCurrentCaretPositionAsync()
        {
            if (IsJSRuntimeAvailable && InputReference != null)
            {
                return await JsRuntime.InvokeAsync<int>("mudInput.getCaretPosition", InputReference.ElementReference);
            }

            return -1;
        }

        /// <summary>
        /// Inserts the given text at the given caret position.
        /// </summary>
        /// <param name="text">The text to insert.</param>
        /// <param name="position">The position to insert the text at. Set to <c>0</c> to insert the text before and to <c>int.MaxValue</c> after the existing text.</param>
        /// <remarks>
        /// If <c>position</c> is greater than the current text length, the text will be inserted at the end.<br/>
        /// If <c>position</c> is less than <c>0</c>, the text will be inserted at the beginning.<br/>
        /// Note that this function doesn't support <see cref="MudMask"/>.
        /// </remarks>
        public async Task InsertTextAsync(string text, int position = int.MaxValue)
        {
            if (HasMask)
            {
                throw new InvalidOperationException("Cannot insert text into masked input.");
            }

            if (IsJSRuntimeAvailable && InputReference != null)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.insertAtPosition", InputReference.ElementReference, text, position);
            }
        }

        /// <summary>
        /// Inserts the given text at the current caret position.
        /// </summary>
        /// <param name="text">The text to insert.</param>
        public async Task InsertTextAtCurrentCaretPositionAsync(string text)
        {
            if (!HasMask && IsJSRuntimeAvailable && InputReference != null)
            {
                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.insertAtCurrentCaretPosition", InputReference.ElementReference, text);
                return;
            }

            if (HasMask)
            {
                await _maskReference.OnPasteAsync(text);
            }
        }

        /// <inheritdoc />
        protected override Task SetValueAndUpdateTextAsync(T? value, bool updateText = true, bool force = false)
        {
            if (HasMask)
            {
                var textValue = ConvertSet(value);
                _mask.SetText(textValue);
                textValue = Mask.GetCleanText();
                value = ConvertGet(textValue);
            }

            return base.SetValueAndUpdateTextAsync(value, updateText, force);
        }

        /// <inheritdoc />
        protected override Task SetTextAndUpdateValueAsync(string? text, bool updateValue = true)
        {
            if (HasMask)
            {
                _mask.SetText(text);
                text = _mask.Text;
            }

            return base.SetTextAndUpdateValueAsync(text, updateValue);
        }

        /// <inheritdoc />
        protected internal override InputType GetInputType() => InputType;

        private bool ShowClearButton()
        {
            if (SubscribeToParentForm)
                return Clearable && !GetReadOnlyState() && !GetDisabledState();
            return Clearable && !GetDisabledState();
        }

        private Task OnMaskedValueChangedAsync(string s) => SetTextAndUpdateValueAsync(s);

        private string GetCounterText() => Counter switch
        {
            null => string.Empty,
            0 => string.IsNullOrEmpty(ReadText) ? "0" : $"{ReadText.Length}",
            _ => (string.IsNullOrEmpty(ReadText) ? "0" : $"{ReadText.Length}") + $" / {Counter}"
        };

        protected async Task HandleContainerClickAsync()
        {
            if (!_isFocused && IsJSRuntimeAvailable && InputReference != null)
            {
                await InputReference.FocusAsync();
            }
        }
    }
}
