// License: MIT
// Copyright (c) 2019 Blazored - See https://github.com/Blazored
// Copyright (c) 2020 Jonny Larsson and Meinrad Recheis

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDialog : MudComponentBase
    {
        protected string ContentClass => new CssBuilder("mud-dialog-content")
          .AddClass($"mud-dialog-no-side-padding", DisableSidePadding)
          .AddClass(ClassContent)
        .Build();

        protected string ActionClass => new CssBuilder("mud-dialog-actions")
          .AddClass(ClassActions)
        .Build();

        [CascadingParameter] private MudDialogInstance DialogInstance { get; set; }

        [Inject] public IDialogService DialogService { get; set; }

        /// <summary>
        /// Define the dialog title as a renderfragment (overrides Title)
        /// </summary>
        [Parameter] public RenderFragment TitleContent { get; set; }

        /// <summary>
        /// Define the dialog body here
        /// </summary>
        [Parameter] public RenderFragment DialogContent { get; set; }

        /// <summary>
        /// Define the action buttons here
        /// </summary>
        [Parameter] public RenderFragment DialogActions { get; set; }

        /// <summary>
        /// Default options to pass to Show(), if none are explicitly provided.
        /// Typically useful on inline dialogs.
        /// </summary>
        [Parameter] public DialogOptions Options { get; set; }

        /// <summary>
        /// No padding at the sides
        /// </summary>
        [Parameter] public bool DisableSidePadding { get; set; }

        /// <summary>
        /// CSS class that will be applied to the dialog content
        /// </summary>
        [Parameter] public string ClassContent { get; set; }

        /// <summary>
        /// CSS class that will be applied to the action buttons container
        /// </summary>
        [Parameter] public string ClassActions { get; set; }

        /// <summary>
        /// CSS styles to be applied to the dialog content
        /// </summary>
        [Parameter] public string ContentStyle { get; set; }

        /// <summary>
        /// Bind this two-way to show and close an inlined dialog. Has no effect on opened dialogs
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
                        Show();
                    else
                        Close();
                }
                IsVisibleChanged.InvokeAsync(value);
            }
        }
        private bool _isVisible;

        /// <summary>
        /// Raised when the inline dialog's display status changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        private bool IsInline => DialogInstance == null;

        private IDialogReference _reference;

        /// <summary>
        /// Show this inlined dialog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public IDialogReference Show(string title = null, DialogOptions options = null)
        {
            if (!IsInline)
                throw new InvalidOperationException("You can only show an inlined dialog.");
            if (_reference != null)
                Close();
            var parameters = new DialogParameters()
            {
                [nameof(TitleContent)] = TitleContent,
                [nameof(DialogContent)] = DialogContent,
                [nameof(DialogActions)] = DialogActions,
                [nameof(DisableSidePadding)] = DisableSidePadding,
                [nameof(ClassContent)] = ClassContent,
                [nameof(ClassActions)] = ClassActions,
            };
            _reference = DialogService.Show<MudDialog>(title, parameters, options ?? Options);
            _reference.Result.ContinueWith(t =>
            {
                _isVisible = false;
                InvokeAsync(() => IsVisibleChanged.InvokeAsync(false));
            });
            return _reference;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (IsInline && _reference != null)
                (_reference.Dialog as MudDialog)?.ForceUpdate(); // forward render update to instance
            base.OnAfterRender(firstRender);
        }

        /// <summary>
        /// Used for forwarding state changes from inlined dialog to its instance
        /// </summary>
        internal void ForceUpdate()
        {
            StateHasChanged();
        }

        /// <summary>
        /// Close the currently open inlined dialog
        /// </summary>
        /// <param name="result"></param>
        public void Close(DialogResult result = null)
        {
            if (!IsInline || _reference == null)
                return;
            _reference.Close(result);
            _reference = null;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DialogInstance?.Register(this);
        }
    }
}
