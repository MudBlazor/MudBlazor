﻿// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/MudBlazor/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

#nullable enable

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MudBlazor
{
    /// <summary>
    /// Represents a manager for <see cref="MudDialog"/> instances.
    /// </summary>
    /// <remarks>
    /// Add this component to your layout page if your application needs to display dialogs.
    /// </remarks>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="MudDialogInstance"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    public partial class MudDialogProvider : IDisposable
    {
        [Inject]
        private IDialogService DialogService { get; set; } = null!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// Hides headers for all dialogs, by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? NoHeader { get; set; }

        /// <summary>
        /// Shows a close button in the top-right corner for all dialogs, by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? CloseButton { get; set; }

        /// <summary>
        /// Allows dialogs to be closed by clicking outside of them, by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? BackdropClick { get; set; }

        /// <summary>
        /// Allows dialogs to be closed by pressing the <c>Escape</c> key, by default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Dialog.Behavior)]
        public bool? CloseOnEscapeKey { get; set; }

        /// <summary>
        /// Sets the width of dialogs to the width of the screen, by default.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public bool? FullWidth { get; set; }

        /// <summary>
        /// The location of dialogs, by default.
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
        /// The custom CSS classes to apply to dialogs, by default.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Dialog.Appearance)]
        public string? BackgroundClass { get; set; }

        private readonly Collection<IDialogReference> _dialogs = new();
        private readonly DialogOptions _globalDialogOptions = new();

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            DialogService.OnDialogInstanceAdded += AddInstance;
            DialogService.OnDialogCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            _globalDialogOptions.BackdropClick = BackdropClick;
            _globalDialogOptions.CloseOnEscapeKey = CloseOnEscapeKey;
            _globalDialogOptions.CloseButton = CloseButton;
            _globalDialogOptions.NoHeader = NoHeader;
            _globalDialogOptions.Position = Position;
            _globalDialogOptions.FullWidth = FullWidth;
            _globalDialogOptions.MaxWidth = MaxWidth;
            _globalDialogOptions.BackgroundClass = BackgroundClass;
        }

        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                foreach (var dialogReference in _dialogs.Where(x => !x.Result.IsCompleted))
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

        private void AddInstance(IDialogReference dialog)
        {
            _dialogs.Add(dialog);
            StateHasChanged();
        }

        /// <summary>
        /// Hides all currently visible dialogs.
        /// </summary>
        public void DismissAll()
        {
            _dialogs.ToList().ForEach(r => DismissInstance(r, DialogResult.Cancel()));
            StateHasChanged();
        }

        private void DismissInstance(IDialogReference dialog, DialogResult result)
        {
            if (!dialog.Dismiss(result)) return;

            _dialogs.Remove(dialog);
            StateHasChanged();
        }

        private IDialogReference? GetDialogReference(Guid id)
        {
            return _dialogs.SingleOrDefault(x => x.Id == id);
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs args)
        {
            DismissAll();
        }

        /// <summary>
        /// Releases resources used by this provider.
        /// </summary>
        public void Dispose()
        {
            if (NavigationManager != null)
                NavigationManager.LocationChanged -= LocationChanged;

            if (DialogService != null)
            {
                DialogService.OnDialogInstanceAdded -= AddInstance;
                DialogService.OnDialogCloseRequested -= DismissInstance;
            }
        }
    }
}
