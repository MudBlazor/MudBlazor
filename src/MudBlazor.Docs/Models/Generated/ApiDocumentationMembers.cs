// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

#nullable enable

/// <summary>
/// Represents the XML documentation for the members of documented types.
/// </summary>
/// <remarks>
/// <para>
/// Member documentation is about ten times the size of the type documentation, and an API page shows one type's members - 37 rows for <c>MudButton</c>, 154 for <c>MudDataGrid</c>.
/// So it is built one type at a time, the first time something asks for that type's members.
/// The pages that make up most of the site never ask at all.
/// </para>
/// <para>
/// A member's key begins with the key of the type that declares it, so the owning type can be derived from the member name and no separate index is needed.
/// </para>
/// </remarks>
public static partial class ApiDocumentationMembers
{
    /// <summary>
    /// Every member built so far, by key.
    /// </summary>
    /// <remarks>
    /// A member is created by the loader of the type that declares it, so a type which inherits members shares the instances declared by its base types.
    /// </remarks>
    internal static Dictionary<string, DocumentedProperty> PropertiesInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedMethod> MethodsInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedField> FieldsInternal { get; } = [];

    /// <inheritdoc cref="PropertiesInternal"/>
    internal static Dictionary<string, DocumentedEvent> EventsInternal { get; } = [];

    private static readonly HashSet<string> LoadedTypes = [];

    private static readonly HashSet<string> LoadingTypes = [];

    private static readonly Dictionary<string, Exception> FailedTypes = [];

    /// <summary>
    /// Guards building, and the reads that trigger it.
    /// </summary>
    /// <remarks>
    /// The WASM client is single threaded, but the prerender host renders crawler requests concurrently.
    /// Every read of a member collection comes through here, so a reader cannot see a type that another thread is still building.
    /// The lock is reentrant, which is what lets a type build the types it inherits from.
    /// </remarks>
    private static readonly Lock Gate = new();

    private static bool _allLoaded;

    private static bool _allLoading;

    private static Exception? _allFailure;

    /// <summary>
    /// How many times something has asked for every type to be built.
    /// </summary>
    /// <remarks>
    /// A member lookup by key must never move this, which is what the tests assert.
    /// Checking the count rather than what happens to be loaded keeps those tests independent of the order the suite runs in.
    /// </remarks>
    internal static int GlobalLoadRequests { get; private set; }

    /// <summary>
    /// Builds the members of a type, and of the types it inherits from.
    /// </summary>
    /// <param name="typeKey">The key of the type to build, or <c>null</c> to do nothing.</param>
    public static void EnsureType(string? typeKey)
    {
        if (typeKey is null)
        {
            return;
        }

        lock (Gate)
        {
            if (LoadedTypes.Contains(typeKey))
            {
                return;
            }

            if (FailedTypes.TryGetValue(typeKey, out var earlier))
            {
                throw LoadFailure(typeKey, earlier);
            }

            // A type reached again while it is still loading is already being built further up this call stack, which is how a type builds the types it inherits from.
            if (!LoadingTypes.Add(typeKey))
            {
                return;
            }

            try
            {
                LoadType(typeKey);
                // Recorded only now, so a loader that threw leaves a partial build looking unfinished rather than complete.
                LoadedTypes.Add(typeKey);
            }
            catch (Exception exception)
            {
                FailedTypes[typeKey] = exception;

                throw LoadFailure(typeKey, exception);
            }
            finally
            {
                LoadingTypes.Remove(typeKey);
            }
        }
    }

    /// <summary>
    /// Builds the members of every documented type.
    /// </summary>
    /// <remarks>
    /// Only needed by the few places that look across all members rather than at one type - the global settings page, and the by-name fallback in <see cref="ApiDocumentation"/>.
    /// </remarks>
    public static void EnsureAll()
    {
        lock (Gate)
        {
            GlobalLoadRequests++;

            if (_allLoaded || _allLoading)
            {
                return;
            }

            if (_allFailure is not null)
            {
                throw LoadFailure(_allFailure);
            }

            _allLoading = true;

            try
            {
                foreach (var typeKey in ApiDocumentation.Types.Keys)
                {
                    EnsureType(typeKey);
                }

                _allLoaded = true;
            }
            catch (Exception exception)
            {
                _allFailure = exception;

                throw LoadFailure(exception);
            }
            finally
            {
                _allLoading = false;
            }
        }
    }

    /// <summary>
    /// Describes a build that failed.
    /// </summary>
    /// <remarks>
    /// A loader assigns members into the shared dictionaries and adds them to its type's collections, and those additions cannot be repeated, so a failed build cannot be retried.
    /// Every later read of the same documentation gets this instead of the partial data the failure left behind.
    /// </remarks>
    private static InvalidOperationException LoadFailure(string typeKey, Exception cause)
    {
        return new InvalidOperationException($"The documentation for '{typeKey}' could not be built.", cause);
    }

    /// <inheritdoc cref="LoadFailure(string, Exception)"/>
    private static InvalidOperationException LoadFailure(Exception cause)
    {
        return new InvalidOperationException("The documentation for every type could not be built.", cause);
    }

    /// <summary>
    /// Gets the key of the type which declares a member.
    /// </summary>
    /// <param name="memberKey">The key of the member, such as <c>MudBlazor.MudAlert.Class</c>.</param>
    /// <returns>The part before the final separator, or <c>null</c> when there is none.</returns>
    internal static string? DeclaringTypeKey(string? memberKey)
    {
        var separator = memberKey?.LastIndexOf('.') ?? -1;

        return separator <= 0 ? null : memberKey![..separator];
    }

    /// <summary>
    /// Gets a property, building the type that declares it if needed.
    /// </summary>
    internal static DocumentedProperty? Property(string key)
    {
        lock (Gate)
        {
            EnsureType(DeclaringTypeKey(key));

            return PropertiesInternal.GetValueOrDefault(key);
        }
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedMethod? Method(string key)
    {
        lock (Gate)
        {
            EnsureType(DeclaringTypeKey(key));

            return MethodsInternal.GetValueOrDefault(key);
        }
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedField? Field(string key)
    {
        lock (Gate)
        {
            EnsureType(DeclaringTypeKey(key));

            return FieldsInternal.GetValueOrDefault(key);
        }
    }

    /// <inheritdoc cref="Property"/>
    internal static DocumentedEvent? Event(string key)
    {
        lock (Gate)
        {
            EnsureType(DeclaringTypeKey(key));

            return EventsInternal.GetValueOrDefault(key);
        }
    }

    /// <summary>
    /// Gets a member of any kind by its key, building the type that declares it if needed.
    /// </summary>
    /// <remarks>
    /// A caller looking for a member without knowing its kind must come through here rather than trying each kind in turn.
    /// The single-kind lookups above fall back to a search by name when the key misses, and that search builds every type, so a method reference would pay for the whole member tier on its way past the property lookup.
    /// </remarks>
    internal static DocumentedMember? Member(string key)
    {
        lock (Gate)
        {
            EnsureType(DeclaringTypeKey(key));

            return (DocumentedMember?)PropertiesInternal.GetValueOrDefault(key)
                ?? (DocumentedMember?)MethodsInternal.GetValueOrDefault(key)
                ?? (DocumentedMember?)FieldsInternal.GetValueOrDefault(key)
                ?? EventsInternal.GetValueOrDefault(key);
        }
    }

    /// <summary>
    /// Builds the members declared by one type.
    /// </summary>
    /// <param name="typeKey">The key of the type to build.</param>
    static partial void LoadType(string typeKey);
}
