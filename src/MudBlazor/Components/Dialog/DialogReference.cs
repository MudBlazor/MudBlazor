// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/Garderoben/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class DialogReference : IDialogReference
    {
        private readonly TaskCompletionSource<DialogResult> _resultCompletion = new TaskCompletionSource<DialogResult>();

        private readonly DialogService _dialogService;

        public DialogReference(Guid dialogInstanceId, DialogService dialogService)
        {
            Id = dialogInstanceId;
            _dialogService = dialogService;
        }

        public void Close()
        {
            _dialogService.Close(this);
        }

        public void Close(DialogResult result)
        {
            _dialogService.Close(this, result);
        }

        internal void Dismiss(DialogResult result)
        {
            _resultCompletion.TrySetResult(result);
        }

        internal Guid Id { get; }

        public MudDialog Dialog { get; private set; }
        internal RenderFragment RenderFragment { get; private set; }

        public Task<DialogResult> Result => _resultCompletion.Task;

        internal void InjectDialog(object inst)
        {
            Dialog = inst as MudDialog;
        }

        internal void InjectRenderFragment(RenderFragment rf)
        {
            RenderFragment = rf;
        }
    }
}
