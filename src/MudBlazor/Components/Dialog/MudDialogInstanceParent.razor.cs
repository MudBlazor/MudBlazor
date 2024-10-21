// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.State;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a parent component for a MudDialogInstance.
/// </summary>
internal class MudDialogInstanceParent : ComponentBaseWithState
{
    private readonly ParameterState<DialogOptions> _dialogOptionsState;
    private readonly ParameterState<string?> _titleState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MudDialogInstanceParent"/> class.
    /// </summary>
    public MudDialogInstanceParent()
    {
        var registerScope = CreateRegisterScope();
        _dialogOptionsState = registerScope.RegisterParameter<DialogOptions>(nameof(Options))
            .WithParameter(() => Options);
        _titleState = registerScope.RegisterParameter<string?>(nameof(Title))
            .WithParameter(() => Title);
    }

    /// <summary>
    /// Gets or sets the unique identifier for the dialog instance.
    /// </summary>
    [Parameter]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the options for the dialog.
    /// </summary>
    [Parameter]
    public DialogOptions Options { get; set; } = DialogOptions.Default;

    /// <summary>
    /// Gets or sets the content to be rendered inside the dialog.
    /// </summary>
    [Parameter]
    public RenderFragment? Content { get; set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudDialogInstance>(0);
        builder.SetKey(Id);
        builder.AddComponentParameter(1, nameof(MudDialogInstance.Options), _dialogOptionsState.Value);
        builder.AddComponentParameter(2, nameof(MudDialogInstance.OptionsChanged), EventCallback.Factory.CreateInferred(this, OnOptionsChangedAsync, _dialogOptionsState.Value));
        builder.AddComponentParameter(3, nameof(MudDialogInstance.Title), Title);
        builder.AddComponentParameter(4, nameof(MudDialogInstance.TitleChanged), EventCallback.Factory.CreateInferred(this, OnTitleChangedAsync, _titleState.Value));
        builder.AddComponentParameter(5, nameof(MudDialogInstance.Content), Content);
        builder.AddComponentParameter(6, nameof(MudDialogInstance.Id), Id);
        builder.CloseComponent();
    }

    /// <summary>
    /// Handles the event when the dialog options are changed.
    /// </summary>
    /// <param name="dialogOptions">The new dialog options.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task OnOptionsChangedAsync(DialogOptions? dialogOptions) => _dialogOptionsState.SetValueAsync(dialogOptions ?? DialogOptions.Default);

    /// <summary>
    /// Handles the event when the dialog title is changed.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private Task OnTitleChangedAsync(string? title) => _titleState.SetValueAsync(title);
}
