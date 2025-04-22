// Copyright (c) 2020 MudBlazor
// License: MIT

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A pop-up dialog with a simple message and button choices.
    /// </summary>
    /// <seealso cref="MudDialog" />
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
        internal IMudDialogInstance? DialogInstance { get; set; }

        /// <summary>
        /// The title of this message box.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c>, the title will be hidden.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The custom content within the title.
        /// </summary>
        /// <remarks>
        /// When set, the <see cref="Title"/> property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The content within this message box.
        /// </summary>
        /// <remarks>
        /// When <see cref="MessageContent"/> is set, this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? Message { get; set; }

        /// <summary>
        /// The markup content within this message box.
        /// </summary>
        /// <remarks>
        /// When <see cref="MessageContent"/> is set, this property is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public MarkupString MarkupMessage { get; set; }

        /// <summary>
        /// The custom content within this message box.
        /// </summary>
        /// <remarks>
        /// When set, <see cref="Message" /> and <see cref="MarkupMessage"/> are ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? MessageContent { get; set; }

        /// <summary>
        /// The text of the Cancel button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c>, the <c>Cancel</c> button will be hidden.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? CancelText { get; set; }

        /// <summary>
        /// The custom content for the Cancel button.
        /// </summary>
        /// <remarks>
        /// Must be a <see cref="MudButton"/>.  When set, <see cref="CancelText"/> is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? CancelButton { get; set; }

        /// <summary>
        /// The text of the No button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c>, the <c>No</c> button will be hidden.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string? NoText { get; set; }

        /// <summary>
        /// The custom content for the No button.
        /// </summary>
        /// <remarks>
        /// Must be a <see cref="MudButton"/>.  When set, <see cref="NoText"/> is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? NoButton { get; set; }

        /// <summary>
        /// The text of the Yes/OK button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>OK</c>.  When <c>null</c>, the <c>Yes</c> button will be hidden.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public string YesText { get; set; } = "OK";

        /// <summary>
        /// The custom content for the Yes button.
        /// </summary>
        /// <remarks>
        /// Must be a <see cref="MudButton"/>.  When set, <see cref="YesText"/> is ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public RenderFragment? YesButton { get; set; }

        /// <summary>
        /// Occurs when the Yes button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnYes { get; set; }

        /// <summary>
        /// Occurs when the No button is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnNo { get; set; }

        /// <summary>
        /// Occurs when the Cancel button is clicked, or this message box is closed via the Close button.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OnCancel { get; set; }

        /// <summary>
        /// Shows this message box.
        /// </summary>
        /// <remarks>
        /// Can be bound via <c>@bind-Visible</c> to show and hide an inlined MessageBox.  Has no effect on previously opened message boxes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.MessageBox.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// Occurs when <see cref="Visible"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        [MemberNotNullWhen(false, nameof(DialogInstance))]
        private bool IsInline => DialogInstance is null;

        /// <summary>
        /// Shows this message box.
        /// </summary>
        /// <param name="options">The optional dialog options to use.</param>
        /// <returns>When <c>true</c>, the Yes/OK button was clicked.  When <c>false</c>, the No button was clicked.  When <c>null</c>, the Cancel button was clicked or this message box was closed.</returns>
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

        /// <summary>
        /// Hides this message box.
        /// </summary>
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
