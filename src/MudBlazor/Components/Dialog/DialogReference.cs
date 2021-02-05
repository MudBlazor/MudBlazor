// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class DialogReference : IDialogReference
    {
        private readonly TaskCompletionSource<DialogResult> _resultCompletion = new TaskCompletionSource<DialogResult>();

        private readonly DialogService _dialogService;

        public DialogReference(Guid dialogInstanceId, RenderFragment dialogInstance, DialogService dialogService)
        {
            Id = dialogInstanceId;
            DialogInstance = dialogInstance;
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

        internal RenderFragment DialogInstance { get; }

        public Task<DialogResult> Result => _resultCompletion.Task;
    }
}
