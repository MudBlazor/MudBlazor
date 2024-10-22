// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
public interface IMudDialogInstance
{
    /// <summary>
    /// The unique ID for this instance.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The unique HTML element ID of the dialog container (mud-dialog-container).
    /// </summary>
    string ElementId { get; }

    /// <summary>
    /// The options used for this dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to the options in the <see cref="MudDialog"/> or options passed during <see cref="DialogService.ShowAsync(Type)"/> methods.
    /// </remarks>
    DialogOptions Options { get; }

    /// <summary>
    /// The text displayed at the top of this dialog if <see cref="MudDialogContainer.TitleContent" /> is not set.
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// Overwrites the current dialog options.
    /// </summary>
    /// <param name="options">The new dialog options to use.</param>
    /// <remarks>
    /// Use this method to change options while a dialog is open, such as toggling fullscreen mode.
    /// </remarks>
    Task SetOptionsAsync(DialogOptions options);

    /// <summary>
    /// Overwrites the dialog title.
    /// </summary>
    /// <param name="title">The new dialog title to use.</param>
    /// <remarks>
    /// Use this method to change the title while a dialog is open, such as when the title reflects a value within this dialog.  Has no effect when <see cref="MudDialogContainer.TitleContent"/> is set.
    /// </remarks>
    Task SetTitleAsync(string? title);

    /// <summary>
    /// Closes this dialog with a result of <c>DialogResult.Ok</c>.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes this dialog with a custom result.
    /// </summary>
    /// <param name="dialogResult">The result to include, such as <see cref="DialogResult.Ok{T}(T)"/> or <see cref="DialogResult.Cancel"/>.</param>
    void Close(DialogResult dialogResult);

    /// <summary>
    /// Closes this dialog with a custom return value.
    /// </summary>
    /// <typeparam name="T">The type of value being returned.</typeparam>
    /// <param name="returnValue">The custom value to include.</param>
    void Close<T>(T returnValue);

    /// <summary>
    /// Closes this dialog with a result of <c>DialogResult.Cancel</c>.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Closes this dialog and any parent dialogs.
    /// </summary>
    void CancelAll();

    /// <summary>
    /// Notifies the component that its state has changed. When applicable, this will
    /// cause the component to be re-rendered.
    /// </summary>
    void StateHasChanged();
}
