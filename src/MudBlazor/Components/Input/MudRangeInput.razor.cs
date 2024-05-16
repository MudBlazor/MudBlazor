using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudRangeInput<T> : MudBaseInput<Range<T>>
    {
        private string _textStart, _textEnd;

        public MudRangeInput()
        {
            Value = new Range<T>();
            Converter = new RangeConverter<T>();
        }

        protected string Classname => MudInputCssHelper.GetClassname(this,
            () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(PlaceholderStart) || !string.IsNullOrWhiteSpace(PlaceholderEnd));

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter] public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        protected string InputClassname => MudInputCssHelper.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelper.GetAdornmentClassname(this);

        /// <summary>
        /// The short hint displayed in the start input before the user enters a value.
        /// </summary>
        [Parameter] public string PlaceholderStart { get; set; }

        /// <summary>
        /// The short hint displayed in the end input before the user enters a value.
        /// </summary>
        [Parameter] public string PlaceholderEnd { get; set; }

        protected bool IsClearable() => Clearable && Value != null;

        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, updateValue: true);
            await _elementReferenceStart.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter] public bool Clearable { get; set; }

        protected string InputTypeString => InputType.ToDescriptionString();

        /// <summary>
        /// ChildContent of the MudInput will only be displayed if InputType.Hidden and if its not null.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private ElementReference _elementReferenceStart, _elementReferenceEnd;

        /// <summary>
        /// Custom separator icon, leave null for default.
        /// </summary>
        [Parameter] public string SeparatorIcon { get; set; } = Icons.Material.Filled.ArrowRightAlt;

        /// <summary>
        /// Focuses the start input of MudRangeInput
        /// </summary>
        /// <returns></returns>
        public ValueTask FocusStartAsync() => _elementReferenceStart.FocusAsync();

        /// <summary>
        /// Selects the start text of MudRangeInput
        /// </summary>
        /// <returns></returns>
        public ValueTask SelectStartAsync() => _elementReferenceStart.MudSelectAsync();

        /// <summary>
        /// Selects the specified range of the start text
        /// </summary>
        /// <param name="pos1">Start position of the selection</param>
        /// <param name="pos2">End position of the selection</param>
        /// <returns></returns>
        public ValueTask SelectRangeStartAsync(int pos1, int pos2) => _elementReferenceStart.MudSelectRangeAsync(pos1, pos2);

        /// <summary>
        /// Focuses the end input of MudRangeInput
        /// </summary>
        /// <returns></returns>
        public ValueTask FocusEndAsync() => _elementReferenceEnd.FocusAsync();

        /// <summary>
        /// Selects the end text of MudRangeInput
        /// </summary>
        /// <returns></returns>
        public ValueTask SelectEndAsync() => _elementReferenceEnd.MudSelectAsync();

        /// <summary>
        /// Selects the specified range of the end text
        /// </summary>
        /// <param name="pos1">Start position of the selection</param>
        /// <param name="pos2">End position of the selection</param>
        /// <returns></returns>
        public ValueTask SelectRangeEndAsync(int pos1, int pos2) => _elementReferenceEnd.MudSelectRangeAsync(pos1, pos2);

        public string TextStart
        {
            get => _textStart;
            set
            {
                if (_textStart == value)
                    return;
                _textStart = value;
                SetTextAsync(RangeConverter<T>.Join(_textStart, _textEnd)).CatchAndLog();
            }
        }

        public string TextEnd
        {
            get => _textEnd;
            set
            {
                if (_textEnd == value)
                    return;
                _textEnd = value;
                SetTextAsync(RangeConverter<T>.Join(_textStart, _textEnd)).CatchAndLog();
            }
        }

        protected override async Task UpdateTextPropertyAsync(bool updateValue)
        {
            await base.UpdateTextPropertyAsync(updateValue);

            RangeConverter<T>.Split(Text, out _textStart, out _textEnd);
        }

        protected override async Task UpdateValuePropertyAsync(bool updateText)
        {
            await base.UpdateValuePropertyAsync(updateText);

            RangeConverter<T>.Split(Text, out _textStart, out _textEnd);
        }
    }
}
