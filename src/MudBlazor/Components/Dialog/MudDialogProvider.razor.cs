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

        private readonly Collection<DialogReference> Dialogs = new Collection<DialogReference>();
        private readonly DialogOptions GlobalDialogOptions = new DialogOptions();

        protected override void OnInitialized()
        {
            ((DialogService)DialogService).OnDialogInstanceAdded += AddInstance;
            ((DialogService)DialogService).OnDialogCloseRequested += DismissInstance;
            NavigationManager.LocationChanged += LocationChanged;

            GlobalDialogOptions.DisableBackdropClick = DisableBackdropClick;
            GlobalDialogOptions.CloseButton = CloseButton;
            GlobalDialogOptions.NoHeader = NoHeader;
            GlobalDialogOptions.Position = Position;
            GlobalDialogOptions.FullWidth = FullWidth;
            GlobalDialogOptions.MaxWidth = MaxWidth;
        }

        internal void DismissInstance(Guid id, DialogResult result)
        {
            var reference = GetDialogReference(id);
            if (reference != null)
                DismissInstance(reference, result);
        }

        private void AddInstance(DialogReference dialog)
        {
            Dialogs.Add(dialog);
            StateHasChanged();
        }

        private void DismissAll()
        {
            Dialogs.ToList().ForEach(r => r.Dismiss(DialogResult.Cancel()));
            Dialogs.Clear();
            StateHasChanged();
        }

        private void DismissInstance(DialogReference dialog, DialogResult result)
        {
            dialog.Dismiss(result);
            Dialogs.Remove(dialog);
            StateHasChanged();
        }

        private DialogReference GetDialogReference(Guid id)
        {
            return Dialogs.SingleOrDefault(x => x.Id == id);
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
