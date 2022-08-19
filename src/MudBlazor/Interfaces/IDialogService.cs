﻿// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT


using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public interface IDialogService
    {
        public event Action<IDialogReference> OnDialogInstanceAdded;
        public event Action<IDialogReference, DialogResult> OnDialogCloseRequested;

        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : ComponentBase;

        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string title) where TComponent : ComponentBase;

        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string title, DialogOptions options) where TComponent : ComponentBase;

        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string title, DialogParameters parameters) where TComponent : ComponentBase;

        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string title, DialogParameters parameters, DialogOptions options) where TComponent : ComponentBase;

        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component);

        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string title);

        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string title, DialogOptions options);

        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string title, DialogParameters parameters);

        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string title, DialogParameters parameters, DialogOptions options);

        IDialogReference CreateReference();

        Task<bool?> ShowMessageBox(string title, string message, string yesText = "OK",
            string noText = null, string cancelText = null, DialogOptions options = null);

        Task<bool?> ShowMessageBox(string title, MarkupString markupMessage, string yesText = "OK",
            string noText = null, string cancelText = null, DialogOptions options = null);

        Task<bool?> ShowMessageBox(MessageBoxOptions messageBoxOptions, DialogOptions options = null);

        void Close(DialogReference dialog);

        void Close(DialogReference dialog, DialogResult result);
    }
}
