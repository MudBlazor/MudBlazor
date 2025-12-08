// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Exposes read-only state and state mutation helpers for a single step.
/// Implemented by <see cref="MudStep"/> and passed to templates (for example <see cref="MudStepper.TitleTemplate"/>,
/// <see cref="MudStepper.LabelTemplate"/> and <see cref="MudStepper.ConnectorTemplate"/>).
/// </summary>
public interface IStepContext
{
    /// <summary>
    /// The title of this step.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>. Use this to display the step title in templates.
    /// Corresponds to <see cref="MudStep.Title"/>.
    /// </remarks>
    string? Title { get; }

    /// <summary>
    /// True when the step has been completed.
    /// </summary>
    /// <remarks>
    /// Use this to render a completed state in templates. Corresponds to <see cref="MudStep.Completed"/>.
    /// </remarks>
    bool Completed { get; }

    /// <summary>
    /// True when the step is disabled and cannot be activated by the user.
    /// </summary>
    /// <remarks>
    /// Corresponds to <see cref="MudStep.Disabled"/>.
    /// </remarks>
    bool Disabled { get; }

    /// <summary>
    /// True when the step is in an error state.
    /// </summary>
    /// <remarks>
    /// Corresponds to <see cref="MudStep.HasError"/>.
    /// </remarks>
    bool HasError { get; }

    /// <summary>
    /// True when the step has been skipped.
    /// </summary>
    /// <remarks>
    /// Corresponds to <see cref="MudStep.Skipped"/>.
    /// </remarks>
    bool Skipped { get; }

    /// <summary>
    /// True when the step can be skipped by the user.
    /// </summary>
    /// <remarks>
    /// Corresponds to <see cref="MudStep.Skippable"/>.
    /// </remarks>
    bool Skippable { get; }

    /// <summary>
    /// True when the step is the currently active step.
    /// </summary>
    /// <remarks>
    /// Use this to highlight or reveal the content for the active step. Corresponds to <see cref="MudStep.IsActive"/>.
    /// </remarks>
    bool IsActive { get; }

    /// <summary>
    /// Sets the <see cref="HasError"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    /// <param name="value">New value for <see cref="HasError"/>.</param>
    /// <param name="refreshParent">If <c>true</c>, notifies the parent <see cref="MudStepper"/> to re-render. Defaults to <c>true</c>.</param>
    Task SetHasErrorAsync(bool value, bool refreshParent = true);

    /// <summary>
    /// Sets the <see cref="Completed"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    /// <param name="value">New value for <see cref="Completed"/>.</param>
    /// <param name="refreshParent">If <c>true</c>, notifies the parent <see cref="MudStepper"/> to re-render. Defaults to <c>true</c>.</param>
    Task SetCompletedAsync(bool value, bool refreshParent = true);

    /// <summary>
    /// Sets the <see cref="Disabled"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    /// <param name="value">New value for <see cref="Disabled"/>.</param>
    /// <param name="refreshParent">If <c>true</c>, notifies the parent <see cref="MudStepper"/> to re-render. Defaults to <c>true</c>.</param>
    Task SetDisabledAsync(bool value, bool refreshParent = true);

    /// <summary>
    /// Sets the <see cref="Skipped"/> parameter, and optionally refreshes the parent <see cref="MudStepper"/>.
    /// </summary>
    /// <param name="value">New value for <see cref="Skipped"/>.</param>
    /// <param name="refreshParent">If <c>true</c>, notifies the parent <see cref="MudStepper"/> to re-render. Defaults to <c>true</c>.</param>
    Task SetSkippedAsync(bool value, bool refreshParent = true);
}
