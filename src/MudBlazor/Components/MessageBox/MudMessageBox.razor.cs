﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    public partial class MudMessageBox : MudComponentBase
    {
        [Inject] private IDialogService DialogService { get; set; }

        [CascadingParameter] private MudDialogInstance DialogInstance { get; set; }

        /// <summary>
        /// The message box title. If null or empty, title will be hidden
        /// </summary>
        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// Define the message box title as a renderfragment (overrides Title)
        /// </summary>
        [Parameter]
        public RenderFragment TitleContent { get; set; }

        /// <summary>
        /// The message box title. If null or empty, title will be hidden
        /// </summary>
        [Parameter]
        public string Message { get; set; }

        /// <summary>
        /// Define the message box body as a renderfragment (overrides Message)
        /// </summary>
        [Parameter]
        public RenderFragment MessageContent { get; set; }


        /// <summary>
        /// Text of the cancel button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        public string CancelText { get; set; }

        /// <summary>
        /// Define the cancel button as a render fragment (overrides CancelText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        public RenderFragment CancelButton { get; set; }

        /// <summary>
        /// Text of the no button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        public string NoText { get; set; }

        /// <summary>
        /// Define the no button as a render fragment (overrides CancelText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        public RenderFragment NoButton { get; set; }

        /// <summary>
        /// Text of the yes/OK button. Leave null to hide the button.
        /// </summary>
        [Parameter]
        public string YesText { get; set; } = "OK";

        /// <summary>
        /// Define the cancel button as a render fragment (overrides CancelText).
        /// Must be a MudButton
        /// </summary>
        [Parameter]
        public RenderFragment YesButton { get; set; }

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
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                if (IsInline)
                {
                    if (_isVisible)
                        _ = Show();
                    else
                        Close();
                }

                IsVisibleChanged.InvokeAsync(value);
            }
        }

        private bool _isVisible;
        private bool IsInline => DialogInstance == null;

        private IDialogReference _reference;

        /// <summary>
        /// Raised when the inline dialog's display status changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsVisibleChanged { get; set; }

        public async Task<bool?> Show(DialogOptions options = null)
        {
            if (DialogService == null)
                return null;
            var parameters = new DialogParameters()
            {
                [nameof(Title)] = Title,
                [nameof(TitleContent)] = TitleContent,
                [nameof(Message)] = Message,
                [nameof(MessageContent)] = MessageContent,
                [nameof(CancelText)] = CancelText,
                [nameof(CancelButton)] = CancelButton,
                [nameof(NoText)] = NoText,
                [nameof(NoButton)] = NoButton,
                [nameof(YesText)] = YesText,
                [nameof(YesButton)] = YesButton,
            };
            _reference = DialogService.Show<MudMessageBox>(parameters: parameters, options: options, title: Title);
            var result = await _reference.Result;
            if (result.Cancelled || !(result.Data is bool))
                return null;
            return (bool)result.Data;
        }

        public void Close()
        {
            _reference?.Close();
        }

        private ActivatableCallback _yesCallback, _cancelCallback, _noCallback;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (YesButton != null)
                _yesCallback = new ActivatableCallback() { ActivateCallback = OnYesActivated };
            if (NoButton != null)
                _noCallback = new ActivatableCallback() { ActivateCallback = OnNoActivated };
            if (CancelButton != null)
                _cancelCallback = new ActivatableCallback() { ActivateCallback = OnCancelActivated };
        }

        private void OnYesActivated(object arg1, MouseEventArgs arg2) => OnYesClicked();

        private void OnNoActivated(object arg1, MouseEventArgs arg2) => OnNoClicked();

        private void OnCancelActivated(object arg1, MouseEventArgs arg2) => OnCancelClicked();

        private void OnYesClicked() => DialogInstance.Close(DialogResult.Ok(true));

        private void OnNoClicked() => DialogInstance.Close(DialogResult.Ok(false));

        private void OnCancelClicked() => DialogInstance.Close(DialogResult.Cancel());
    }


}
