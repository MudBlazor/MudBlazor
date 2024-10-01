// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// An instance of a <see cref="MudDialog"/>.
    /// </summary>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="MudDialogInstance"/>
    /// <seealso cref="MudDialogProvider"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogService"/>
    public class DialogReference : IDialogReference
    {
        private readonly TaskCompletionSource<DialogResult?> _resultCompletion = new();

        private readonly IDialogService _dialogService;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dialogInstanceId">The unique ID of the dialog.</param>
        /// <param name="dialogService">The service used to manage dialogs.</param>
        public DialogReference(Guid dialogInstanceId, IDialogService dialogService)
        {
            Id = dialogInstanceId;
            _dialogService = dialogService;
        }

        /// <inheritdoc />
        public void Close()
        {
            _dialogService.Close(this);
        }

        /// <inheritdoc />
        public void Close(DialogResult? result)
        {
            _dialogService.Close(this, result);
        }

        /// <inheritdoc />
        public virtual bool Dismiss(DialogResult? result)
        {
            return _resultCompletion.TrySetResult(result);
        }

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public object? Dialog { get; private set; }

        /// <inheritdoc />
        public RenderFragment? RenderFragment { get; set; }

        /// <inheritdoc />
        public Task<DialogResult?> Result => _resultCompletion.Task;

        TaskCompletionSource<bool> IDialogReference.RenderCompleteTaskCompletionSource { get; } = new();

        /// <inheritdoc />
        public void InjectDialog(object inst)
        {
            Dialog = inst;
        }

        /// <inheritdoc />
        public void InjectRenderFragment(RenderFragment rf)
        {
            RenderFragment = rf;
        }

        /// <inheritdoc />
        public async Task<T?> GetReturnValueAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
        {
            var result = await Result;
            try
            {
                return (T?)result?.Data;
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine($"Could not cast return value to {typeof(T)}, returning default.");
                return default;
            }
        }
    }
}
