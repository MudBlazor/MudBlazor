//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

#nullable enable

namespace MudBlazor;

/// <summary>
/// Defines the snackbar service.
/// </summary>
public interface ISnackbar : IDisposable
{
    /// <summary>
    /// Gets the collection of currently shown snackbars.
    /// </summary>
    IEnumerable<Snackbar> ShownSnackbars { get; }

    /// <summary>
    /// Gets the global configuration for a snackbar.
    /// </summary>
    SnackbarConfiguration Configuration { get; }

    /// <summary>
    /// Event triggered when the collection of shown snackbars is updated.
    /// </summary>
    event Action? OnSnackbarsUpdated;

    /// <summary>
    /// Adds a new snackbar with the specified message.
    /// </summary>
    /// <param name="message">The message to display in the snackbar.</param>
    /// <param name="severity">The severity of the snackbar message. Default is <see cref="Severity.Normal"/>.</param>
    /// <param name="configure">Optional action to configure the <see cref="SnackbarOptions"/>.</param>
    /// <param name="key">An optional key to uniquely identify the snackbar. Default is the value of <paramref name="message"/>.</param>
    /// <returns>The created snackbar instance, or null if not created.</returns>
    /// <remarks>If a <paramref name="key"/> is provided, this snackbar will not be shown while any snackbar with the same key is being shown.</remarks>
    Snackbar? Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null);

    /// <summary>
    /// Adds a new snackbar with the specified markup message.
    /// </summary>
    /// <param name="message">The markup message to display in the snackbar.</param>
    /// <param name="severity">The severity of the snackbar message. Default is <see cref="Severity.Normal"/>.</param>
    /// <param name="configure">Optional action to configure the <see cref="SnackbarOptions"/>.</param>
    /// <param name="key">An optional key to uniquely identify the snackbar. Default is the value of <paramref name="message"/>.</param>
    /// <returns>The created snackbar instance, or null if not created.</returns>
    /// <remarks>If a <paramref name="key"/> is provided, this snackbar will not be shown while any snackbar with the same key is being shown.</remarks>
    Snackbar? Add(MarkupString message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null);

    /// <summary>
    /// Adds a new snackbar with the specified render fragment.
    /// </summary>
    /// <param name="message">The render fragment to display in the snackbar.</param>
    /// <param name="severity">The severity of the snackbar message. Default is <see cref="Severity.Normal"/>.</param>
    /// <param name="configure">Optional action to configure the <see cref="SnackbarOptions"/>.</param>
    /// <param name="key">An optional key to uniquely identify the snackbar.</param>
    /// <returns>The created snackbar instance, or null if not created.</returns>
    /// <remarks>If a <paramref name="key"/> is provided, this snackbar will not be shown while any snackbar with the same key is being shown.</remarks>
    Snackbar? Add(RenderFragment message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null);

    /// <summary>
    /// Adds a new snackbar with the specified component.
    /// </summary>
    /// <typeparam name="T">The component type to render inside the snackbar.</typeparam>
    /// <param name="componentParameters">Optional parameters to pass to the component.</param>
    /// <param name="severity">The severity of the snackbar message. Default is <see cref="Severity.Normal"/>.</param>
    /// <param name="configure">Optional action to configure the <see cref="SnackbarOptions"/>.</param>
    /// <param name="key">An optional key to uniquely identify the snackbar.</param>
    /// <returns>The created snackbar instance, or null if not created.</returns>
    /// <remarks>If a <paramref name="key"/> is provided, this snackbar will not be shown while any snackbar with the same key is being shown.</remarks>
    Snackbar? Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null) where T : IComponent;

    /// <summary>
    /// Clears all displayed snackbars.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes the specified snackbar.
    /// </summary>
    /// <param name="snackbar">The snackbar to remove.</param>
    void Remove(Snackbar snackbar);

    /// <summary>
    /// Removes a snackbar by its key.
    /// </summary>
    /// <param name="key">The key of the snackbar to remove.</param>
    void RemoveByKey(string key);
}
