// License: MIT
// Copyright (c) 2019 Blazored - See https://github.com/Blazored
// Copyright (c) 2020 Jonny Larsson and Meinrad Recheis

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDialog : MudComponentBase
    {
        private IDialogReference _reference;
        private readonly ParameterState<bool> _visibleState;

        public MudDialog()
        {
            using var registerScope = CreateRegisterScope();
            _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
                .WithParameter(() => Visible)
                .WithEventCallback(() => VisibleChanged);
        }

        protected string ContentClassname => new CssBuilder("mud-dialog-content")
            .AddClass("mud-dialog-no-side-padding", !Gutters)
            .AddClass(ContentClass)
            .Build();

        protected string ActionsClassname => new CssBuilder("mud-dialog-actions")
            .AddClass(ActionsClass)
            .Build();

        [CascadingParameter]
        private MudDialogInstance DialogInstance { get; set; }

        [CascadingParameter(Name = "IsNested")]
        private bool IsNested { get; set; }

        [Inject]
        protected IDialogService DialogService { get; set; }

        /// <summary>
        /// Define the dialog title as a RenderFragment (overrides Title)
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
        /// Setting BackdropClick to "false" disables both - OnBackdropClick as well
        /// as the default logic.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public EventCallback<MouseEventArgs> OnBackdropClick { get; set; }

        /// <summary>
        /// Add padding at the sides
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public bool Gutters { get; set; } = true;

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
        public bool Visible { get; set; }

        /// <summary>
        /// Raised when the inline dialog's display status changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Defines the element that will receive the focus when the dialog is opened.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public DefaultFocus DefaultFocus { get; set; } = MudGlobal.DialogDefaults.DefaultFocus;

        private bool IsInline => IsNested || DialogInstance is null;

        /// <summary>
        /// Shows this inlined dialog asynchronously.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="options">The options for the dialog.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IDialogReference> ShowAsync(string title = null, DialogOptions options = null)
        {
            if (!IsInline)
            {
                throw new InvalidOperationException("You can only show an inlined dialog.");
            }

            if (_reference is not null)
            {
                await CloseAsync();
            }

            var parameters = new DialogParameters
            {
                [nameof(Class)] = Class,
                [nameof(Style)] = Style,
                [nameof(Tag)] = Tag,
                [nameof(UserAttributes)] = UserAttributes,
                [nameof(TitleContent)] = TitleContent,
                [nameof(DialogContent)] = DialogContent,
                [nameof(DialogActions)] = DialogActions,
                [nameof(OnBackdropClick)] = OnBackdropClick,
                [nameof(Gutters)] = Gutters,
                [nameof(TitleClass)] = TitleClass,
                [nameof(ContentClass)] = ContentClass,
                [nameof(ActionsClass)] = ActionsClass,
                [nameof(ContentStyle)] = ContentStyle,
                [nameof(DefaultFocus)] = DefaultFocus,
            };

            await _visibleState.SetValueAsync(true);

            // ReSharper disable MethodHasAsyncOverload ignore for now
            _reference = DialogService.Show<MudDialog>(title, parameters, options ?? Options);
            // ReSharper restore MethodHasAsyncOverload

            // Do not await this!
            _reference.Result.ContinueWith(t =>
            {
                return InvokeAsync(() => _visibleState.SetValueAsync(false));
            }).AndForget();

            return _reference;
        }

        /// <summary>
        /// Closes the currently open inlined dialog asynchronously.
        /// </summary>
        /// <param name="result">The result to be passed to the dialog's completion task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CloseAsync(DialogResult result = null)
        {
            if (!IsInline || _reference is null)
            {
                return;
            }

            await _visibleState.SetValueAsync(false);
            _reference.Close(result);
            _reference = null;
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsInline)
            {
                if (_visibleState.Value && _reference is null)
                {
                    // If visible and we don't have any reference we need to call Show
                    await ShowAsync();
                }
                else if (_reference is not null)
                {
                    if (_visibleState.Value)
                    {
                        // Forward render update to instance
                        (_reference.Dialog as IMudStateHasChanged)?.StateHasChanged();
                    }
                    else
                    {
                        // If we still have reference, but it's not visible call Close
                        await CloseAsync();
                    }
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (!IsNested)
            {
                DialogInstance?.Register(this);
            }
        }
    }
}
