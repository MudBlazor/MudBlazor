// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored
// Copyright (c) 2020 - Adapted by MudBlazor Contributors

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A manager for <see cref="MudDialog"/> instances.
    /// </summary>
    /// <remarks>
    /// Add this component to your layout page if your application needs to display dialogs.
    /// </remarks>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="MudDialogContainer"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    /// <seealso cref="MudBlazor.DialogService"/>
    public partial class MudDialogProvider : IDisposable
    {
        private DialogOptions _globalDialogOptions = new();
        private readonly List<IDialogReference> _dialogs = [];

        [Inject]
        private IDialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// Hides headers for all dialogs by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? NoHeader { get; set; }

        /// <summary>
        /// Shows a close button in the top-right corner for all dialogs by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? CloseButton { get; set; }

        /// <summary>
        /// Allows dialogs to be closed by clicking outside of them by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? BackdropClick { get; set; }

        /// <summary>
        /// Allows dialogs to be closed by pressing the Escape key by default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? CloseOnEscapeKey { get; set; }

        /// <summary>
        /// Sets the width of dialogs to the width of the screen by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public bool? FullWidth { get; set; }

        /// <summary>
        /// The location of dialogs by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public DialogPosition? Position { get; set; }

        /// <summary>
        /// The maximum allowed with of the dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public MaxWidth? MaxWidth { get; set; }

        /// <summary>
        /// The custom CSS classes to apply to dialogs by default.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? BackgroundClass { get; set; }

        protected override void OnInitialized()
        {
            DialogService.DialogInstanceAddedAsync += AddInstanceAsync;
            DialogService.OnDialogCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            var newOptions = _globalDialogOptions with
            {
                BackdropClick = BackdropClick,
                CloseOnEscapeKey = CloseOnEscapeKey,
                CloseButton = CloseButton,
                NoHeader = NoHeader,
                Position = Position,
                FullWidth = FullWidth,
                MaxWidth = MaxWidth,
                BackgroundClass = BackgroundClass
            };

            _globalDialogOptions = newOptions;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                foreach (var dialogReference in _dialogs.ToArray().Where(x => !x.Result.IsCompleted))
                {
                    dialogReference.RenderCompleteTaskCompletionSource.TrySetResult(true);
                }
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        internal void DismissInstance(Guid id, DialogResult result)
        {
            var reference = GetDialogReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }

        private Task AddInstanceAsync(IDialogReference dialog)
        {
            _dialogs.Add(dialog);
            return InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Hides all currently visible dialogs.
        /// </summary>
        public void DismissAll()
        {
            foreach (var dialog in _dialogs.ToArray())
            {
                DismissInstance(dialog, DialogResult.Cancel());
            }
            StateHasChanged();
        }

        private void DismissInstance(IDialogReference dialog, DialogResult? result)
        {
            if (!dialog.Dismiss(result)) return;

            _dialogs.Remove(dialog);
            StateHasChanged();
        }

        private IDialogReference? GetDialogReference(Guid id)
        {
            return _dialogs.ToArray().FirstOrDefault(d => d.Id == id);
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs args)
        {
            DismissAll();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                NavigationManager.LocationChanged -= LocationChanged;
                DialogService.DialogInstanceAddedAsync -= AddInstanceAsync;
                DialogService.OnDialogCloseRequested -= DismissInstance;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
