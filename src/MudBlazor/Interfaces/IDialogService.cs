// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT


using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

#nullable enable

namespace MudBlazor
{
    /// <summary>
    /// A service for managing <see cref="MudDialog"/> components.
    /// </summary>
    /// <remarks>
    /// This service requires a <see cref="MudDialogProvider"/> in your layout page.
    /// </remarks>
    public interface IDialogService
    {
        public event Action<IDialogReference>? OnDialogInstanceAdded;
        public event Action<IDialogReference, DialogResult?>? OnDialogCloseRequested;

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <typeparam name="TComponent">The type of dialog to display.</typeparam>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title.
        /// </summary>
        /// <typeparam name="TComponent">The type of dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title and options.
        /// </summary>
        /// <typeparam name="TComponent">The type of dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogOptions options) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with custom parameters.
        /// </summary>
        /// <typeparam name="TComponent">The type of dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogParameters parameters) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with custom options and parameters.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogParameters parameters, DialogOptions? options) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="component">The type of dialog to display.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component);

        /// <summary>
        /// Displays a dialog with a custom title.
        /// </summary>
        /// <param name="component">The type of dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title);

        /// <summary>
        /// Displays a dialog with a custom title and options.
        /// </summary>
        /// <param name="component">The type of dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogOptions options);

        /// <summary>
        /// Displays a dialog with a custom title and options.
        /// </summary>
        /// <param name="component">The type of dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogParameters parameters);

        /// <summary>
        /// Displays a dialog with a custom title, parameters, and options.
        /// </summary>
        /// <param name="component">The type of dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogParameters parameters, DialogOptions options);

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <typeparam name="TComponent">The dialog to display.</typeparam>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>() where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title.
        /// </summary>
        /// <typeparam name="TComponent">The dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title and options.
        /// </summary>
        /// <typeparam name="TComponent">The dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogOptions options) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title and parameters.
        /// </summary>
        /// <typeparam name="TComponent">The dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogParameters parameters) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog with a custom title, parameters, and options.
        /// </summary>
        /// <typeparam name="TComponent">The dialog to display.</typeparam>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(string? title, DialogParameters parameters, DialogOptions? options) where TComponent : IComponent;

        /// <summary>
        /// Displays a dialog.
        /// </summary>
        /// <param name="component">The dialog to display.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component);

        /// <summary>
        /// Displays a dialog with a custom title.
        /// </summary>
        /// <param name="component">The dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title);

        /// <summary>
        /// Displays a dialog with a custom title and options.
        /// </summary>
        /// <param name="component">The dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogOptions options);

        /// <summary>
        /// Displays a dialog with a custom title and parameters.
        /// </summary>
        /// <param name="component">The dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogParameters parameters);

        /// <summary>
        /// Displays a dialog with a custom title, parameters, and options.
        /// </summary>
        /// <param name="component">The dialog to display.</param>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="parameters">The custom parameters to set within the dialog.</param>
        /// <param name="options">The custom display options for the dialog.</param>
        /// <returns>A reference to the dialog.</returns>
        Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogParameters parameters, DialogOptions options);

        /// <summary>
        /// Creates a reference to a dialog.
        /// </summary>
        /// <returns>The dialog reference.</returns>
        IDialogReference CreateReference();

        /// <summary>
        /// Shows a simple dialog with a title, message, and up to three custom buttons.
        /// </summary>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="message">The message displayed under the title.</param>
        /// <param name="yesText">The text of the "Yes" button.  Defaults to <c>OK</c>.</param>
        /// <param name="noText">The text of the "No" button.  Defaults to <c>null</c>.</param>
        /// <param name="cancelText">The text of the "Cancel" button.  Defaults to <c>null</c>.</param>
        /// <param name="options">The custom display options for the dialog.  Defaults to <c>null</c>.</param>
        /// <returns>Returns <c>null</c> if the <c>Cancel</c> button was clicked, <c>true</c> if the <c>Yes</c> button was clicked, or <c>false</c> if the <c>No</c> button was clicked.</returns>
        Task<bool?> ShowMessageBox(string? title, string message, string yesText = "OK",
            string? noText = null, string? cancelText = null, DialogOptions? options = null);

        /// <summary>
        /// Shows a simple dialog with a title, HTML message, and up to three custom buttons.
        /// </summary>
        /// <param name="title">The text at the top of the dialog.</param>
        /// <param name="markupMessage">The HTML message displayed under the title.</param>
        /// <param name="yesText">The text of the "Yes" button.  Defaults to <c>OK</c>.</param>
        /// <param name="noText">The text of the "No" button.  Defaults to <c>null</c>.</param>
        /// <param name="cancelText">The text of the "Cancel" button.  Defaults to <c>null</c>.</param>
        /// <param name="options">The custom display options for the dialog.  Defaults to <c>null</c>.</param>
        /// <returns>Returns <c>null</c> if the <c>Cancel</c> button was clicked, <c>true</c> if the <c>Yes</c> button was clicked, or <c>false</c> if the <c>No</c> button was clicked.</returns>
        Task<bool?> ShowMessageBox(string? title, MarkupString markupMessage, string yesText = "OK",
            string? noText = null, string? cancelText = null, DialogOptions? options = null);

        /// <summary>
        /// Shows a simple dialog with custom options.
        /// </summary>
        /// <param name="messageBoxOptions">The options for the message box.</param>
        /// <param name="options">The custom display options for the dialog.  Defaults to <c>null</c>.</param>
        /// <returns>Returns <c>null</c> if the <c>Cancel</c> button was clicked, <c>true</c> if the <c>Yes</c> button was clicked, or <c>false</c> if the <c>No</c> button was clicked.</returns>
        Task<bool?> ShowMessageBox(MessageBoxOptions messageBoxOptions, DialogOptions? options = null);

        /// <summary>
        /// Hides an existing dialog.
        /// </summary>
        /// <param name="dialog">The reference of the dialog to hide.</param>
        void Close(IDialogReference dialog);

        /// <summary>
        /// Hides an existing dialog.
        /// </summary>
        /// <param name="dialog">The reference of the dialog to hide.</param>
        /// <param name="result">The result to include.</param>
        void Close(IDialogReference dialog, DialogResult? result);
    }
}
