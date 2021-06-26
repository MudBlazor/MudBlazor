// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MudBlazor
{
    public partial class MudDialogProvider : IDisposable
    {
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter] public bool? NoHeader { get; set; }
        [Parameter] public bool? CloseButton { get; set; }
        [Parameter] public bool? DisableBackdropClick { get; set; }
        [Parameter] public bool? FullWidth { get; set; }
        [Parameter] public DialogPosition? Position { get; set; }
        [Parameter] public MaxWidth? MaxWidth { get; set; }

        private readonly Collection<DialogReference> _dialogs = new Collection<DialogReference>();
        private readonly DialogOptions _globalDialogOptions = new DialogOptions();

        protected override void OnInitialized()
        {
            ((DialogService)DialogService).OnDialogInstanceAdded += AddInstance;
            ((DialogService)DialogService).OnDialogCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            _globalDialogOptions.DisableBackdropClick = DisableBackdropClick;
            _globalDialogOptions.CloseButton = CloseButton;
            _globalDialogOptions.NoHeader = NoHeader;
            _globalDialogOptions.Position = Position;
            _globalDialogOptions.FullWidth = FullWidth;
            _globalDialogOptions.MaxWidth = MaxWidth;
        }

        internal void DismissInstance(Guid id, DialogResult result)
        {
            var reference = GetDialogReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }

        private void AddInstance(DialogReference dialog)
        {
            _dialogs.Add(dialog);
            StateHasChanged();
        }

        public void DismissAll()
        {
            _dialogs.ToList().ForEach(r => r.Dismiss(DialogResult.Cancel()));
            _dialogs.Clear();
            StateHasChanged();
        }

        private void DismissInstance(DialogReference dialog, DialogResult result)
        {
            dialog.Dismiss(result);
            _dialogs.Remove(dialog);
            StateHasChanged();
        }

        private DialogReference GetDialogReference(Guid id)
        {
            return _dialogs.SingleOrDefault(x => x.Id == id);
        }

        private void LocationChanged(object sender, LocationChangedEventArgs args)
        {
            DismissAll();
        }

        public void Dispose()
        {
            if (NavigationManager != null)
                NavigationManager.LocationChanged -= LocationChanged;
        }
    }
}
