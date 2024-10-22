﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        private IMask? _mask = null;
        private MudMask? _maskReference = null;

        protected string Classname =>
           new CssBuilder("mud-input-input-control")
               .AddClass(Class)
               .Build();

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
        public bool Clearable { get; set; } = false;

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
        /// When set, some properties will be ignored such as <see cref="MudInput{T}.MaxLines"/>, <see cref="MudInput{T}.AutoGrow"/>, and <see cref="MudInput{T}.HideSpinButtons"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask? Mask
        {
            get => _maskReference?.Mask ?? _mask; // this might look strange, but it is absolutely necessary due to how MudMask works.
            set => _mask = value;
        }

        /// <summary>
        /// Stretches this input vertically to accommodate the <see cref="MudBaseInput{T}.Text"/> value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool AutoGrow { get; set; }

        /// <summary>
        /// The maximum vertical lines to display when <see cref="AutoGrow"/> is <c>true</c>.
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
        public Task Clear()
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
        public async Task SetText(string text)
        {
            if (!HasMask)
            {
                await InputReference.SetText(text);
                return;
            }

            await _maskReference.Clear();
            _maskReference.OnPaste(text);
        }

        protected override Task SetValueAsync(T? value, bool updateText = true, bool force = false)
        {
            if (HasMask)
            {
                var textValue = Converter.Set(value);
                _mask.SetText(textValue);
                textValue = Mask.GetCleanText();
                value = Converter.Get(textValue);
            }

            return base.SetValueAsync(value, updateText, force);
        }

        protected override Task SetTextAsync(string? text, bool updateValue = true)
        {
            if (HasMask)
            {
                _mask.SetText(text);
                text = _mask.Text;
            }

            return base.SetTextAsync(text, updateValue);
        }

        internal override InputType GetInputType() => InputType;

        private bool GetClearable()
        {
            if (SubscribeToParentForm)
                return Clearable && !GetReadOnlyState() && !GetDisabledState();
            return Clearable && !GetDisabledState();
        }

        private Task OnMaskedValueChanged(string s) => SetTextAsync(s);

        private string GetCounterText() => Counter switch
        {
            null => string.Empty,
            0 => (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}"),
            _ => (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"
        };
    }
}
