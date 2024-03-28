﻿// License: MIT
// Copyright (c) 2019 Blazored - See https://github.com/Blazored
// Copyright (c) 2020 Jonny Larsson and Meinrad Recheis

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDialog : MudComponentBase
    {
        protected string ContentClassname => new CssBuilder("mud-dialog-content")
            .AddClass("mud-dialog-no-side-padding", DisableSidePadding)
            .AddClass(ContentClass)
            .Build();

        protected string ActionsClassname => new CssBuilder("mud-dialog-actions")
            .AddClass(ActionsClass)
            .Build();

        [CascadingParameter] private MudDialogInstance DialogInstance { get; set; }
        [CascadingParameter(Name = "IsNested")] private bool IsNested { get; set; }

        [Inject] public IDialogService DialogService { get; set; }

        /// <summary>
        /// Define the dialog title as a renderfragment (overrides Title)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment TitleContent { get; set; }

        /// <summary>
        /// Define the dialog body here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment DialogContent { get; set; }

        /// <summary>
        /// Define the action buttons here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment DialogActions { get; set; }

        /// <summary>
        /// Default options to pass to Show(), if none are explicitly provided.
        /// Typically useful on inline dialogs.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Misc)]  // Behavior and Appearance
        public DialogOptions Options { get; set; }

        /// <summary>
        /// Defines delegate with custom logic when user clicks overlay behind dialogue.
        /// Is being invoked instead of default "Backdrop Click" logic.
        /// Setting DisableBackdropClick to "true" disables both - OnBackdropClick as well
        /// as the default logic.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public EventCallback<MouseEventArgs> OnBackdropClick { get; set; }

        /// <summary>
        /// No padding at the sides
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public bool DisableSidePadding { get; set; }

        /// <summary>
        /// CSS class that will be applied to the dialog title container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string TitleClass { get; set; }

        /// <summary>
        /// CSS class that will be applied to the dialog content
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string ContentClass { get; set; }

        /// <summary>
        /// CSS class that will be applied to the action buttons container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string ActionsClass { get; set; }

        /// <summary>
        /// CSS styles to be applied to the dialog content
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string ContentStyle { get; set; }

        /// <summary>
        /// Bind this two-way to show and close an inlined dialog. Has no effect on opened dialogs
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                IsVisibleChanged.InvokeAsync(value);
            }
        }
        private bool _isVisible;

        /// <summary>
        /// Raised when the inline dialog's display status changes.
        /// </summary>
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

        /// <summary>
        /// Define the element that will receive the focus when the dialog is opened
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;

        private bool IsInline => IsNested || DialogInstance == null;

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
                [nameof(Class)] = Class,
                [nameof(Style)] = Style,
                [nameof(Tag)] = Tag,
                [nameof(UserAttributes)] = UserAttributes,
                [nameof(TitleContent)] = TitleContent,
                [nameof(DialogContent)] = DialogContent,
                [nameof(DialogActions)] = DialogActions,
                [nameof(OnBackdropClick)] = OnBackdropClick,
                [nameof(DisableSidePadding)] = DisableSidePadding,
                [nameof(TitleClass)] = TitleClass,
                [nameof(ContentClass)] = ContentClass,
                [nameof(ActionsClass)] = ActionsClass,
                [nameof(ContentStyle)] = ContentStyle,
                [nameof(DefaultFocus)] = DefaultFocus,
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
            if (IsInline)
            {
                if (_isVisible && _reference == null)
                {
                    Show(); // if isVisible and we don't have any reference we need to call Show
                }
                else if (_reference != null)
                {
                    if (IsVisible)
                        (_reference.Dialog as IMudStateHasChanged)?.StateHasChanged(); // forward render update to instance
                    else
                        Close(); // if we still have reference but it's not visible call Close
                }
            }
            base.OnAfterRender(firstRender);
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
            if (!IsNested)
                DialogInstance?.Register(this);
        }
    }
}
