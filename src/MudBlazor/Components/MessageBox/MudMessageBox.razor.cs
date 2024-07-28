using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    public partial class MudMessageBox : MudComponentBase
    {
        private readonly ParameterState<bool> _visibleState;
        private IDialogReference? _reference;
        private ActivatableCallback? _yesCallback, _cancelCallback, _noCallback;

        protected string Classname =>
            new CssBuilder("mud-message-box")
                .Build();

        public MudMessageBox()
        {
            using var registerScope = CreateRegisterScope();
            _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
                .WithParameter(() => Visible)
                .WithEventCallback(() => VisibleChanged)
                .WithChangeHandler(OnVisibleChangedAsync);
        }

        [Inject]
        private IDialogService DialogService { get; set; } = null!;

        [CascadingParameter]
        internal MudDialogInstance? DialogInstance { get; set; }

        /// <summary>
        /// The message box title. If null or empty, title will be hidden
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// Define the message box title as a renderfragment (overrides Title)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The message box message as string.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? Message { get; set; }

        /// <summary>
        /// The message box message as markup string.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public MarkupString MarkupMessage { get; set; }

        /// <summary>
        /// Define the message box body as a renderfragment (overrides Message)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? MessageContent { get; set; }

        /// <summary>
        /// Text of the cancel button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? CancelText { get; set; }

        /// <summary>
        /// Define the cancel button as a render fragment (overrides CancelText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? CancelButton { get; set; }

        /// <summary>
        /// Text of the no button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? NoText { get; set; }

        /// <summary>
        /// Define the no button as a render fragment (overrides NoText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? NoButton { get; set; }

        /// <summary>
        /// Text of the yes/OK button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string YesText { get; set; } = "OK";

        /// <summary>
        /// Define the yes button as a render fragment (overrides YesText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? YesButton { get; set; }

        /// <summary>
        /// Fired when the yes button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnYes { get; set; }

        /// <summary>
        /// Fired when the no button is clicked
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnNo { get; set; }

        /// <summary>
        /// Fired when the cancel button is clicked or the msg box was closed via the X
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnCancel { get; set; }

        /// <summary>
        /// Bind this two-way to show and close an inlined message box. Has no effect on opened msg boxes
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// Raised when the inline dialog's display status changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [MemberNotNullWhen(false, nameof(DialogInstance))]
        private bool IsInline => DialogInstance is null;

        public async Task<bool?> ShowAsync(DialogOptions? options = null)
        {
            var parameters = new DialogParameters
            {
                [nameof(Title)] = Title,
                [nameof(TitleContent)] = TitleContent,
                [nameof(Message)] = Message,
                [nameof(MarkupMessage)] = MarkupMessage,
                [nameof(MessageContent)] = MessageContent,
                [nameof(CancelText)] = CancelText,
                [nameof(CancelButton)] = CancelButton,
                [nameof(NoText)] = NoText,
                [nameof(NoButton)] = NoButton,
                [nameof(YesText)] = YesText,
                [nameof(YesButton)] = YesButton,
            };
            _reference = await DialogService.ShowAsync<MudMessageBox>(title: Title, parameters: parameters, options: options);
            var result = await _reference.Result;

            if (result is null)
            {
                return null;
            }

            if (result.Canceled || result.Data is not bool data)
            {
                return null;
            }

            return data;
        }

        public void Close() => _reference?.Close();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (YesButton is not null)
            {
                _yesCallback = new ActivatableCallback { ActivateCallback = OnYesActivated };
            }

            if (NoButton is not null)
            {
                _noCallback = new ActivatableCallback { ActivateCallback = OnNoActivated };
            }

            if (CancelButton is not null)
            {
                _cancelCallback = new ActivatableCallback { ActivateCallback = OnCancelActivated };
            }
        }

        private async Task OnVisibleChangedAsync(ParameterChangedEventArgs<bool> arg)
        {
            if (IsInline)
            {
                if (arg.Value)
                {
                    await ShowAsync();
                }
                else
                {
                    Close();
                }
            }
        }

        private void OnYesActivated(object arg1, MouseEventArgs arg2) => OnYesClicked();

        private void OnNoActivated(object arg1, MouseEventArgs arg2) => OnNoClicked();

        private void OnCancelActivated(object arg1, MouseEventArgs arg2) => OnCancelClicked();

        private void OnYesClicked() => DialogInstance?.Close(DialogResult.Ok(true));

        private void OnNoClicked() => DialogInstance?.Close(DialogResult.Ok(false));

        private void OnCancelClicked() => DialogInstance?.Close(DialogResult.Cancel());
    }
}
