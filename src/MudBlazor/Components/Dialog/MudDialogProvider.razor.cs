// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored


using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using System;
using System.Linq;

namespace MudBlazor.Dialog
{
    public partial class MudDialogProvider
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
            ((DialogService)DialogService).OnDialogInstanceAdded += Update;
            ((DialogService)DialogService).OnDialogCloseRequested += CloseInstance;
            NavigationManager.LocationChanged += CancelDialogs;

            GlobalDialogOptions.DisableBackdropClick = DisableBackdropClick;
            GlobalDialogOptions.CloseButton = CloseButton;
            GlobalDialogOptions.NoHeader = NoHeader;
            GlobalDialogOptions.Position = Position;
            GlobalDialogOptions.FullWidth = FullWidth;
            GlobalDialogOptions.MaxWidth = MaxWidth;
        }

        internal async void CloseInstance(DialogReference dialog, DialogResult result)
        {
            await DismissInstance(dialog, result);
        }

        internal void CloseInstance(Guid Id)
        {
            var reference = GetDialogReference(Id);
            CloseInstance(reference, DialogResult.Ok<object>(null));
        }

        internal void CancelInstance(Guid Id)
        {
            var reference = GetDialogReference(Id);
            CloseInstance(reference, DialogResult.Cancel());
        }

        internal Task DismissInstance(Guid Id, DialogResult result)
        {
            var reference = GetDialogReference(Id);
            return DismissInstance(reference, result);
        }

        internal async Task DismissInstance(DialogReference dialog, DialogResult result)
        {
            if (dialog != null)
            {
                dialog.Dismiss(result);
                Dialogs.Remove(dialog);
                await InvokeAsync(StateHasChanged);
            }
        }

        private async void CancelDialogs(object sender, LocationChangedEventArgs e)
        {
            foreach (var DialogReference in Dialogs.ToList())
            {
                DialogReference.Dismiss(DialogResult.Cancel());
            }

            Dialogs.Clear();
            await InvokeAsync(StateHasChanged);
        }

        private async void Update(DialogReference DialogReference)
        {
            Dialogs.Add(DialogReference);
            await InvokeAsync(StateHasChanged);
        }

        private DialogReference GetDialogReference(Guid Id)
        {
            return Dialogs.SingleOrDefault(x => x.Id == Id);
        }
    }
}
