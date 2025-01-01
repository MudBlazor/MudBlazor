// Copyright (c) 2019 Blazored
// Copyright (c) 2020 Adapted by Jonny Larsson, Meinrad Recheis and Contributors

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// An instance of a <see cref="MudDialog"/>.
    /// </summary>
    /// <remarks>
    /// When a <see cref="MudDialog"/> is shown, a new instance is created.  This instance can then be used to perform actions such as hiding the dialog programmatically.
    /// </remarks>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="MudDialogProvider"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    /// <seealso cref="DialogService"/>
    public partial class MudDialogContainer : MudComponentBase, IMudDialogInstanceInternal, IAsyncDisposable
    {
        private bool _disposed;
        private MudDialog? _dialog;
        private readonly ParameterState<DialogOptions> _dialogOptionsState;
        private readonly ParameterState<string?> _titleState;
        private readonly string _elementId = Identifier.Create("dialog");

        public MudDialogContainer()
        {
            var registerScope = CreateRegisterScope();
            _dialogOptionsState = registerScope.RegisterParameter<DialogOptions>(nameof(Options))
                .WithParameter(() => Options);
            _titleState = registerScope.RegisterParameter<string?>(nameof(Title))
                .WithParameter(() => Title);
        }

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        [CascadingParameter]
        private MudDialogProvider Parent { get; set; } = null!;

        [CascadingParameter]
        private DialogOptions GlobalDialogOptions { get; set; } = DialogOptions.Default;

        /// <summary>
        /// The options used for this dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to the options in the <see cref="MudDialog"/> or options passed during <see cref="DialogService.ShowAsync(Type)"/> methods.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Misc)] // Behavior and Appearance
        public DialogOptions Options { get; set; } = DialogOptions.Default;

        /// <summary>
        /// The text displayed at the top of this dialog if <see cref="TitleContent" /> is not set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The custom content at the top of this dialog.
        /// </summary>
        /// <remarks>
        /// This content will display so long as <see cref="Title"/> is not set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The content within this dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to the content of the <see cref="MudDialog"/> being displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public RenderFragment? Content { get; set; }

        /// <inheritdoc />
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public Guid Id { get; set; }

        /// <summary>
        /// The custom icon displayed in the upper-right corner for closing this dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Close"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        protected string TitleClassname =>
            new CssBuilder("mud-dialog-title")
                .AddClass(_dialog?.TitleClass)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-dialog")
                .AddClass(GetMaxWidth(), !GetFullScreen())
                .AddClass("mud-dialog-width-full", GetFullWidth() && !GetFullScreen())
                .AddClass("mud-dialog-fullscreen", GetFullScreen())
                .AddClass("mud-dialog-rtl", RightToLeft)
                .AddClass(_dialog?.Class)
                .Build();

        protected string BackgroundClassname =>
            new CssBuilder("mud-overlay-dialog")
                .AddClass("mud-skip-overlay-positioning") // popovers try to position the overlay by zindex, this skips that behavior if a user puts the dialog provider above the popover provider
                .AddClass(GetDialogOptionsOrDefault.BackgroundClass)
                .Build();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-dialog",
                    [
                        new("/./", subscribeDown: true, subscribeUp: true)
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDownAsync, keyUp: HandleKeyUpAsync);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Escape":
                    if (GetCloseOnEscapeKey())
                    {
                        ((IMudDialogInstance)this).Cancel();
                    }
                    break;
            }
            if (_dialog is not null && _dialog.OnKeyDown.HasDelegate)
            {
                await _dialog.OnKeyDown.InvokeAsync(args);
                // Note: we need to force a render here because the user will expect this blazor standard functionality.
                // Since the event originates from KeyInterceptor it will not cause a render automatically.
                await InvokeAsync(StateHasChanged);
            }
        }

        internal async Task HandleKeyUpAsync(KeyboardEventArgs args)
        {
            if (_dialog is not null && _dialog.OnKeyUp.HasDelegate)
            {
                await _dialog.OnKeyUp.InvokeAsync(args);
                // note: we need to force a render here because the user will expect this blazor standard functionality
                // Since the event originates from KeyInterceptor it will not cause a render automatically.
                await InvokeAsync(StateHasChanged);
            }
        }

        private bool GetHideHeader()
        {
            if (GetDialogOptionsOrDefault.NoHeader.HasValue)
                return GetDialogOptionsOrDefault.NoHeader.Value;

            if (GlobalDialogOptions.NoHeader.HasValue)
                return GlobalDialogOptions.NoHeader.Value;

            return false;
        }

        private bool GetCloseButton()
        {
            if (GetDialogOptionsOrDefault.CloseButton.HasValue)
                return GetDialogOptionsOrDefault.CloseButton.Value;

            if (GlobalDialogOptions.CloseButton.HasValue)
                return GlobalDialogOptions.CloseButton.Value;

            return false;
        }

        private bool GetBackdropClick()
        {
            if (GetDialogOptionsOrDefault.BackdropClick.HasValue)
                return GetDialogOptionsOrDefault.BackdropClick.Value;

            if (GlobalDialogOptions.BackdropClick.HasValue)
                return GlobalDialogOptions.BackdropClick.Value;

            return true;
        }

        private bool GetCloseOnEscapeKey()
        {
            if (GetDialogOptionsOrDefault.CloseOnEscapeKey.HasValue)
                return GetDialogOptionsOrDefault.CloseOnEscapeKey.Value;

            if (GlobalDialogOptions.CloseOnEscapeKey.HasValue)
                return GlobalDialogOptions.CloseOnEscapeKey.Value;

            return false;
        }

        private async Task HandleBackgroundClickAsync(MouseEventArgs args)
        {
            if (!GetBackdropClick())
                return;

            if (_dialog is null || !_dialog.OnBackdropClick.HasDelegate)
            {
                ((IMudDialogInstance)this).Cancel();
                return;
            }

            await _dialog.OnBackdropClick.InvokeAsync(args);
        }

        private string GetPosition()
        {
            DialogPosition position;

            if (GetDialogOptionsOrDefault.Position.HasValue)
            {
                position = GetDialogOptionsOrDefault.Position.Value;
            }
            else if (GlobalDialogOptions.Position.HasValue)
            {
                position = GlobalDialogOptions.Position.Value;
            }
            else
            {
                position = DialogPosition.Center;
            }
            return $"mud-dialog-{position.ToDescriptionString()}";
        }

        private string GetMaxWidth()
        {
            MaxWidth maxWidth;

            if (GetDialogOptionsOrDefault.MaxWidth.HasValue)
            {
                maxWidth = GetDialogOptionsOrDefault.MaxWidth.Value;
            }
            else if (GlobalDialogOptions.MaxWidth.HasValue)
            {
                maxWidth = GlobalDialogOptions.MaxWidth.Value;
            }
            else
            {
                maxWidth = MaxWidth.Small;
            }
            return $"mud-dialog-width-{maxWidth.ToDescriptionString()}";
        }

        private bool GetFullWidth() => GetDialogOptionsOrDefault.FullWidth ?? GlobalDialogOptions.FullWidth ?? false;

        private bool GetFullScreen() => GetDialogOptionsOrDefault.FullScreen ?? GlobalDialogOptions.FullScreen ?? false;

        private DialogOptions GetDialogOptionsOrDefault => _dialogOptionsState.Value ?? DialogOptions.Default;

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        string IMudDialogInstance.ElementId => _elementId;

        /// <inheritdoc />
        string? IMudDialogInstance.Title => Title;

        /// <inheritdoc />
        DialogOptions IMudDialogInstance.Options => GetDialogOptionsOrDefault;

        /// <inheritdoc />
        async Task IMudDialogInstance.SetOptionsAsync(DialogOptions options)
        {
            await _dialogOptionsState.SetValueAsync(options);
            await InvokeAsync(StateHasChanged);
        }

        /// <inheritdoc />
        async Task IMudDialogInstance.SetTitleAsync(string? title)
        {
            await _titleState.SetValueAsync(title);
            await InvokeAsync(StateHasChanged);
        }

        /// <inheritdoc />
        void IMudDialogInstance.Close()
        {
            ((IMudDialogInstance)this).Close(DialogResult.Ok<object?>(null));
        }

        /// <inheritdoc />
        void IMudDialogInstance.Close(DialogResult dialogResult)
        {
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <inheritdoc />
        void IMudDialogInstance.Close<T>(T returnValue)
        {
            var dialogResult = DialogResult.Ok<T>(returnValue);
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <inheritdoc />
        void IMudDialogInstance.Cancel() => ((IMudDialogInstance)this).Close(DialogResult.Cancel());

        /// <inheritdoc />
        void IMudDialogInstanceInternal.Register(MudDialog dialog)
        {
            _dialog = dialog;
            Class = dialog.Class;
            Style = dialog.Style;
            TitleContent = dialog.TitleContent;
            StateHasChanged();
        }

        /// <inheritdoc />
        void IMudDialogInstance.StateHasChanged() => StateHasChanged();

        /// <inheritdoc />
        void IMudDialogInstance.CancelAll()
        {
            Parent?.DismissAll();
        }
    }
}
