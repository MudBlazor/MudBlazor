using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTextField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        public MudInput<string> InputReference { get; private set; }
        private MudMask _maskReference;

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        public override ValueTask FocusAsync()
        {
            if (_mask == null)
                return InputReference.FocusAsync();
            else
                return _maskReference.FocusAsync();
        }

        public override ValueTask BlurAsync()
        {
            if (_mask == null)
                return InputReference.BlurAsync();
            else
                return _maskReference.BlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            if (_mask == null)
                return InputReference.SelectAsync();
            else
                return _maskReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            if (_mask == null)
                return InputReference.SelectRangeAsync(pos1, pos2);
            else
                return _maskReference.SelectRangeAsync(pos1, pos2);
        }

        protected override async Task ResetValueAsync()
        {
            if (_mask == null)
                await InputReference.ResetAsync();
            else
                await _maskReference.ResetAsync();
            await base.ResetValueAsync();
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            if (_mask == null)
                return InputReference.SetText(null);
            else
                return _maskReference.Clear();
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SetText(string text)
        {
            if (_mask == null)
            {
                if (InputReference != null)
                    await InputReference.SetText(text);
                return;
            }
            await _maskReference.Clear();
            _maskReference.OnPaste(text);
        }

        private IMask _mask = null;

        /// <summary>
        /// Provide a masking object. Built-in masks are PatternMask, MultiMask, RegexMask and BlockMask
        /// Note: when Mask is set, TextField will ignore some properties such as Lines, Pattern or HideSpinButtons, OnKeyDown and OnKeyUp, etc.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IMask Mask
        {
            get => _maskReference?.Mask ?? _mask; // this might look strange, but it is absolutely necessary due to how MudMask works.
            set
            {
                _mask = value;
            }
        }

        protected override Task SetValueAsync(T value, bool updateText = true, bool force = false)
        {
            if (_mask != null)
            {
                var textValue = Converter.Set(value);
                _mask.SetText(textValue);
                textValue = Mask.GetCleanText();
                value = Converter.Get(textValue);
            }

            return base.SetValueAsync(value, updateText);
        }

        protected override Task SetTextAsync(string text, bool updateValue = true)
        {
            if (_mask != null)
            {
                _mask.SetText(text);
                text = _mask.Text;
            }
            return base.SetTextAsync(text, updateValue);
        }

        private async Task OnMaskedValueChanged(string s)
        {
            await SetTextAsync(s);
        }

        /// <summary>
        /// If true the input element will grow automatically with the text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool AutoGrow { get; set; }

        /// <summary>
        /// If AutoGrow is set to true, the input element will not grow bigger than MaxLines lines. If MaxLines is set to 0
        /// or less, the property will be ignored.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public int MaxLines { get; set; }
    }
}
