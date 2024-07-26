using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    /// <summary>
    /// A component for collecting start and end values which define a range.
    /// </summary>
    /// <typeparam name="T">The type of object managed by this input.</typeparam>
    public partial class MudRangeInput<T> : MudBaseInput<Range<T>>
    {
        private string _textStart, _textEnd;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudRangeInput()
        {
            Value = new Range<T>();
            Converter = new RangeConverter<T>();
        }

        protected string Classname => MudInputCssHelper.GetClassname(this,
            () => !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(PlaceholderStart) || !string.IsNullOrWhiteSpace(PlaceholderEnd));

        /// <summary>
        /// The type of input collected by this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputType.Text"/>.  Represents a valid HTML5 input type.
        /// </remarks>
        [Parameter]
        public InputType InputType { get; set; } = InputType.Text;

        internal override InputType GetInputType() => InputType;

        protected string InputClassname => MudInputCssHelper.GetInputClassname(this);

        protected string AdornmentClassname => MudInputCssHelper.GetAdornmentClassname(this);

        /// <summary>
        /// The hint displayed before the user enters a starting value.
        /// </summary>
        [Parameter]
        public string PlaceholderStart { get; set; }

        /// <summary>
        /// The hint displayed before the user enters an ending value.
        /// </summary>
        [Parameter]
        public string PlaceholderEnd { get; set; }

        protected bool IsClearable() => Clearable && Value != null;

        protected virtual async Task ClearButtonClickHandlerAsync(MouseEventArgs e)
        {
            await SetTextAsync(string.Empty, updateValue: true);
            await _elementReferenceStart.FocusAsync();
            await OnClearButtonClick.InvokeAsync(e);
        }

        /// <summary>
        /// Occurs when the Clear button is clicked.
        /// </summary>
        /// <remarks>
        /// When clicked, the <see cref="MudBaseInput{T}.Text"/> and <see cref="MudBaseInput{T}.Value"/> properties are reset.
        /// </remarks>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        /// <summary>
        /// Shows a button at the end of the input to clear the value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool Clearable { get; set; }

        protected string InputTypeString => InputType.ToDescriptionString();

        /// <summary>
        /// The content within this input component.
        /// </summary>
        /// <remarks>
        /// Will only display if <see cref="InputType"/> is <see cref="InputType.Hidden"/>.
        /// </remarks>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private ElementReference _elementReferenceStart, _elementReferenceEnd;

        /// <summary>
        /// The icon shown in between start and end values.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowRightAlt"/>.
        /// </remarks>
        [Parameter]
        public string SeparatorIcon { get; set; } = Icons.Material.Filled.ArrowRightAlt;

        /// <summary>
        /// Moves the cursor to the starting input component.
        /// </summary>
        public ValueTask FocusStartAsync() => _elementReferenceStart.FocusAsync();

        /// <summary>
        /// Selects the text in the starting input.
        /// </summary>
        public ValueTask SelectStartAsync() => _elementReferenceStart.MudSelectAsync();

        /// <summary>
        /// Selects the text in the start value.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public ValueTask SelectRangeStartAsync(int pos1, int pos2) => _elementReferenceStart.MudSelectRangeAsync(pos1, pos2);

        /// <summary>
        /// Moves the cursor to the ending input component.
        /// </summary>
        public ValueTask FocusEndAsync() => _elementReferenceEnd.FocusAsync();

        /// <summary>
        /// Selects the text in the ending input.
        /// </summary>
        public ValueTask SelectEndAsync() => _elementReferenceEnd.MudSelectAsync();

        /// <summary>
        /// Selects the text in the end value.
        /// </summary>
        /// <param name="pos1">The index of the first character to select.</param>
        /// <param name="pos2">The index of the last character to select.</param>
        public ValueTask SelectRangeEndAsync(int pos1, int pos2) => _elementReferenceEnd.MudSelectRangeAsync(pos1, pos2);

        /// <summary>
        /// The text of the start of the range.
        /// </summary>
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

        /// <summary>
        /// The text of the end of the range.
        /// </summary>
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
