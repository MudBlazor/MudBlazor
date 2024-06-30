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

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// An overlay providing the user with information, a choice, or other input.
    /// </summary>
    /// <seealso cref="MudDialogInstance"/>
    /// <seealso cref="MudDialogProvider"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    /// <seealso cref="DialogService"/>
    public partial class MudDialog : MudComponentBase
    {
        private IDialogReference? _reference;
        private readonly ParameterState<bool> _visibleState;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
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
        private MudDialogInstance? DialogInstance { get; set; }

        [CascadingParameter(Name = "IsNested")]
        private bool IsNested { get; set; }

        [Inject]
        protected IDialogService DialogService { get; set; } = null!;

        /// <summary>
        /// The custom content for this dialog's title.
        /// </summary>
        /// <remarks>
        /// When <c>null</c>, the <see cref="MudDialogInstance.Title"/> will be used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The main content for this dialog.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment? DialogContent { get; set; }

        /// <summary>
        /// The custom actions for this dialog.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment? DialogActions { get; set; }

        /// <summary>
        /// The default options for this dialog.
        /// </summary>
        /// <remarks>
        /// These options are used if none are provided during the <see cref="ShowAsync(string, DialogOptions)"/> method.  This is typically used for inline dialogs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Misc)]  // Behavior and Appearance
        public DialogOptions? Options { get; set; }

        /// <summary>
        /// Occurs when the area outside the dialog has been clicked if <see cref="DialogOptions.BackdropClick"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// When set, this event will be called instead of the default backdrop click behavior of closing the dialog.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public EventCallback<MouseEventArgs> OnBackdropClick { get; set; }

        /// <summary>
        /// Adds padding to the sides of this dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// The CSS classes to apply to the title.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? TitleClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the main dialog content.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? ContentClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the action buttons content.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? ActionsClass { get; set; }

        /// <summary>
        /// The CSS styles applied to the main dialog content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? ContentStyle { get; set; }

        /// <summary>
        /// For inline dialogs, shows this dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.<br />
        /// This can be bound via <c>@bind-Visible</c> to show or hide inline dialogs.  For regular dialogs, use the <see cref="DialogService.ShowAsync(Type)"/> and <see cref="MudDialogInstance.Close()"/> methods.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// Occurs when <see cref="Visible"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// The element which will receive focus when this dialog is shown.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudGlobal.DialogDefaults.DefaultFocus"/>.        
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public DefaultFocus DefaultFocus { get; set; } = MudGlobal.DialogDefaults.DefaultFocus;

        private bool IsInline => IsNested || DialogInstance is null;

        /// <summary>
        /// For inlined dialogs, shows this dialog.
        /// </summary>
        /// <param name="title">The title of this dialog.</param>
        /// <param name="options">The options for this dialog.</param>
        /// <returns>The reference to the displayed instance of this dialog.</returns>
        public async Task<IDialogReference> ShowAsync(string? title = null, DialogOptions? options = null)
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
            }).CatchAndLog();

            return _reference;
        }

        /// <summary>
        /// For inlined dialogs, hides this dialog.
        /// </summary>
        /// <param name="result">The optional data to include.</param>
        public async Task CloseAsync(DialogResult? result = null)
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
