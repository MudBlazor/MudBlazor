// Copyright (c) 2019 Blazored
// Copyright (c) 2020 Adapted by Jonny Larsson, Meinrad Recheis and Contributors

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
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
    public partial class MudDialogInstance : MudComponentBase, IDisposable
    {
        private DialogOptions? _options = new();
        private readonly string _elementId = Identifier.Create("dialog");
        private IKeyInterceptor? _keyInterceptor;

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; } = null!;

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
        [Category(CategoryTypes.Dialog.Misc)]  // Behavior and Appearance
        public DialogOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new DialogOptions();
                return _options;
            }
            set => _options = value;
        }

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

        /// <summary>
        /// The unique ID for this instance.
        /// </summary>
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

        private string Position { get; set; } = null!;
        private string DialogMaxWidth { get; set; } = null!;
        private bool BackdropClick { get; set; } = true;
        private bool CloseOnEscapeKey { get; set; }
        private bool NoHeader { get; set; }
        private bool CloseButton { get; set; }
        private bool FullScreen { get; set; }
        private bool FullWidth { get; set; }

        protected override void OnInitialized()
        {
            ConfigureInstance();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //Since CloseOnEscapeKey is the only thing to be handled, turn interceptor off
                if (CloseOnEscapeKey)
                {
                    _keyInterceptor = KeyInterceptorFactory.Create();

                    await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                    {
                        TargetClass = "mud-dialog",
                        Keys = {
                            new KeyOptions { Key="Escape", SubscribeDown = true },
                        },
                    });
                    _keyInterceptor.KeyDown += HandleKeyDown;
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal void HandleKeyDown(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Escape":
                    if (CloseOnEscapeKey)
                    {
                        Cancel();
                    }
                    break;
            }
        }

        /// <summary>
        /// Overwrites the current dialog options.
        /// </summary>
        /// <param name="options">The new dialog options to use.</param>
        /// <remarks>
        /// Use this method to change options while a dialog is open, such as toggling fullscreen mode.
        /// </remarks>
        public void SetOptions(DialogOptions options)
        {
            Options = options;
            ConfigureInstance();
            StateHasChanged();
        }

        /// <summary>
        /// Overwrites the dialog title.
        /// </summary>
        /// <param name="title">The new dialog title to use.</param>
        /// <remarks>
        /// Use this method to change the title while a dialog is open, such as when the title reflects a value within this dialog.  Has no effect when <see cref="TitleContent"/> is set.
        /// </remarks>
        public void SetTitle(string title)
        {
            Title = title;
            StateHasChanged();
        }

        /// <summary>
        /// Closes this dialog with a result of <c>DialogResult.Ok</c>.
        /// </summary>
        public void Close()
        {
            Close(DialogResult.Ok<object?>(null));
        }

        /// <summary>
        /// Closes this dialog with a custom result.
        /// </summary>
        /// <param name="dialogResult">The result to include, such as <see cref="DialogResult.Ok{T}(T)"/> or <see cref="DialogResult.Cancel"/>.</param>
        public void Close(DialogResult dialogResult)
        {
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <summary>
        /// Closes this dialog with a custom return value.
        /// </summary>
        /// <typeparam name="T">The type of value being returned.</typeparam>
        /// <param name="returnValue">The custom value to include.</param>
        public void Close<T>(T returnValue)
        {
            var dialogResult = DialogResult.Ok<T>(returnValue);
            Parent.DismissInstance(Id, dialogResult);
        }

        /// <summary>
        /// Closes this dialog with a result of <c>DialogResult.Cancel</c>.
        /// </summary>
        public void Cancel()
        {
            Close(DialogResult.Cancel());
        }

        private void ConfigureInstance()
        {
            Position = SetPosition();
            DialogMaxWidth = SetMaxWidth();
            NoHeader = SetHideHeader();
            CloseButton = SetCloseButton();
            FullWidth = SetFullWidth();
            FullScreen = SetFulScreen();
            BackdropClick = SetBackdropClick();
            CloseOnEscapeKey = SetCloseOnEscapeKey();
            Class = Classname;
            BackgroundClassname = new CssBuilder("mud-overlay-dialog").AddClass(Options.BackgroundClass).Build();
        }

        private string SetPosition()
        {
            DialogPosition position;

            if (Options.Position.HasValue)
            {
                position = Options.Position.Value;
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

        private string SetMaxWidth()
        {
            MaxWidth maxWidth;

            if (Options.MaxWidth.HasValue)
            {
                maxWidth = Options.MaxWidth.Value;
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

        private bool SetFullWidth()
        {
            if (Options.FullWidth.HasValue)
                return Options.FullWidth.Value;

            if (GlobalDialogOptions.FullWidth.HasValue)
                return GlobalDialogOptions.FullWidth.Value;

            return false;
        }

        private bool SetFulScreen()
        {
            if (Options.FullScreen.HasValue)
                return Options.FullScreen.Value;

            if (GlobalDialogOptions.FullScreen.HasValue)
                return GlobalDialogOptions.FullScreen.Value;

            return false;
        }

        protected string TitleClassname =>
            new CssBuilder("mud-dialog-title")
                .AddClass(_dialog?.TitleClass)
                .Build();

        protected string Classname =>
            new CssBuilder("mud-dialog")
                .AddClass(DialogMaxWidth, !FullScreen)
                .AddClass("mud-dialog-width-full", FullWidth && !FullScreen)
                .AddClass("mud-dialog-fullscreen", FullScreen)
                .AddClass("mud-dialog-rtl", RightToLeft)
                .AddClass(_dialog?.Class)
            .Build();

        protected string BackgroundClassname { get; set; } = "mud-overlay-dialog";

        private bool SetHideHeader()
        {
            if (Options.NoHeader.HasValue)
                return Options.NoHeader.Value;

            if (GlobalDialogOptions.NoHeader.HasValue)
                return GlobalDialogOptions.NoHeader.Value;

            return false;
        }

        private bool SetCloseButton()
        {
            if (Options.CloseButton.HasValue)
                return Options.CloseButton.Value;

            if (GlobalDialogOptions.CloseButton.HasValue)
                return GlobalDialogOptions.CloseButton.Value;

            return false;
        }

        private bool SetBackdropClick()
        {
            if (Options.BackdropClick.HasValue)
                return Options.BackdropClick.Value;

            if (GlobalDialogOptions.BackdropClick.HasValue)
                return GlobalDialogOptions.BackdropClick.Value;

            return true;
        }

        private bool SetCloseOnEscapeKey()
        {
            if (Options.CloseOnEscapeKey.HasValue)
                return Options.CloseOnEscapeKey.Value;

            if (GlobalDialogOptions.CloseOnEscapeKey.HasValue)
                return GlobalDialogOptions.CloseOnEscapeKey.Value;

            return false;
        }

        private async Task HandleBackgroundClickAsync(MouseEventArgs args)
        {
            if (!BackdropClick)
                return;

            if (_dialog is null || !_dialog.OnBackdropClick.HasDelegate)
            {
                Cancel();
                return;
            }

            await _dialog.OnBackdropClick.InvokeAsync(args);
        }

        private MudDialog? _dialog;
        private bool _disposedValue;

        /// <summary>
        /// Links a dialog with this instance.
        /// </summary>
        /// <param name="dialog">The dialog to use.</param>
        /// <remarks>
        /// This method is used internally when displaying a new dialog.
        /// </remarks>
        public void Register(MudDialog dialog)
        {
            _dialog = dialog;
            Class = dialog.Class;
            Style = dialog.Style;
            TitleContent = dialog.TitleContent;
            StateHasChanged();
        }

        public new void StateHasChanged() => base.StateHasChanged();

        /// <summary>
        /// Closes this dialog and any parent dialogs.
        /// </summary>
        public void CancelAll()
        {
            Parent?.DismissAll();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_keyInterceptor != null)
                    {
                        _keyInterceptor.KeyDown -= HandleKeyDown;
                        if (IsJSRuntimeAvailable)
                        {
                            _keyInterceptor.Dispose();
                        }
                    }
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Releases resources used by this dialog.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
