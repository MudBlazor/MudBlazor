﻿// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class DialogReference : IDialogReference
    {
        private readonly TaskCompletionSource<DialogResult> _resultCompletion = new();

        private readonly IDialogService _dialogService;

        public DialogReference(Guid dialogInstanceId, IDialogService dialogService)
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

        public virtual bool Dismiss(DialogResult result)
        {
            _resultCompletion.TrySetResult(result);

            return true;
        }

        public Guid Id { get; }

        public object Dialog { get; private set; }
        public RenderFragment RenderFragment { get; set; }

        public Task<DialogResult> Result => _resultCompletion.Task;

        public bool AreParametersRendered { get; set; }

        public void InjectDialog(object inst)
        {
            Dialog = inst;
        }

        public void InjectRenderFragment(RenderFragment rf)
        {
            RenderFragment = rf;
        }

        public async Task<T> GetReturnValueAsync<T>()
        {
            var result=await Result;
            try
            {
                return (T)result.Data;
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine($"Could not cast return value to {typeof(T)}, returning default.");
                return default;
            }
        }

    }
}
