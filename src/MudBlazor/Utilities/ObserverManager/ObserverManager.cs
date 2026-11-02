// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace MudBlazor.Utilities.ObserverManager;

/// <summary>
/// Maintains a collection of observers.
/// </summary>
/// <typeparam name="TIdentity">
/// The address type, used to identify observers.
/// </typeparam>
/// <typeparam name="TObserver">
/// The observer type.
/// </typeparam>
/// <remarks>
/// This class maintains a collection of observers and provides functionality to add, remove, and notify observers.
/// It also supports removing defunct observers that have failed during the notification process.
/// </remarks>
internal sealed class ObserverManager<TIdentity, TObserver> : IEnumerable<TObserver> where TIdentity : notnull
{
    private readonly ILogger _log;
    private readonly ConcurrentDictionary<TIdentity, Entry> _observers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManager{TIdentity,TObserver}"/> class. 
    /// </summary>
    public ObserverManager(ILogger log) : this(log, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManager{TIdentity,TObserver}"/> class. 
    /// </summary>
    public ObserverManager(ILogger log, IEqualityComparer<TIdentity>? comparer)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _observers = new(comparer);
    }

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    public int Count => _observers.Count;

    /// <summary>
    /// Gets a copy of the observers.
    /// </summary>
    /// <remarks>
    /// Creates a new dictionary containing all current observers. This is an O(n) operation.
    /// </remarks>
    public IDictionary<TIdentity, TObserver> Observers
    {
        get
        {
            var result = new Dictionary<TIdentity, TObserver>(_observers.Count);
            foreach (var (id, observer) in _observers)
            {
                result.Add(id, observer);
            }
            return result;
        }
    }

    /// <summary>
    /// Removes all observers.
    /// </summary>
    public void Clear() => _observers.Clear();

    /// <summary>
    /// Checks if an observer with the specified identity is subscribed.
    /// </summary>
    /// <param name="id">The identity of the observer.</param>
    /// <returns>True if the observer is subscribed; otherwise, false.</returns>
    public bool IsSubscribed(TIdentity id) => _observers.ContainsKey(id);

    /// <summary>
    /// Tries to get the subscription for the specified identity.
    /// </summary>
    /// <param name="id">The identity of the observer.</param>
    /// <param name="observer">When this method returns, contains the observer associated with the specified identity, if the identity is found; otherwise, the default value for the type of the observer parameter.</param>
    /// <returns>True if the observer is found; otherwise, false.</returns>
    public bool TryGetSubscription(TIdentity id, [MaybeNullWhen(false)] out TObserver observer)
    {
        if (_observers.TryGetValue(id, out var entry))
        {
            observer = entry.Observer;
            return true;
        }
        observer = default;
        return false;
    }

    /// <summary>
    /// Finds the identities of observers that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the observers.</param>
    /// <returns>An enumerable collection of observer identities that match the predicate.</returns>
    public IEnumerable<TIdentity> FindObserverIdentities(Func<TIdentity, TObserver, bool> predicate)
    {
        foreach (var (id, observer) in _observers)
        {
            if (predicate(id, observer.Observer))
            {
                yield return id;
            }
        }
    }

    /// <summary>
    /// Tries to get the existing subscription for the specified identity, or subscribes the observer if it does not exist.
    /// </summary>
    /// <param name="id">The identity of the observer.</param>
    /// <param name="observer">The observer to subscribe if it does not already exist.</param>
    /// <param name="newObserver">When this method returns, contains the observer associated with the specified identity, whether it was already subscribed or newly subscribed.</param>
    /// <returns>True if the observer was already subscribed; otherwise, false.</returns>
    /// <remarks>
    /// Uses AddOrUpdate for atomic operation that determines if the observer existed.
    /// </remarks>
    public bool TryGetOrAddSubscription(TIdentity id, TObserver observer, out TObserver newObserver)
    {
        var entry = _observers.AddOrUpdate(
            id,
            // Add path
            _ => new Entry(observer, false),
            // Update path
            (_, _) => new Entry(observer, true)
        );
        newObserver = entry.Observer;

        if (_log.IsEnabled(LogLevel.Trace))
        {
            if (entry.WasExisting)
            {
                var count = _observers.Count;
                _log.LogTrace("Updating entry for {Id}/{Observer}. {Count} total observers.", id, observer, count);
            }
            else
            {
                var count = _observers.Count;
                _log.LogTrace("Adding entry for {Id}/{Observer}. {Count} total observers after add.", id, observer, count);
            }
        }

        return entry.WasExisting;
    }

    /// <summary>
    /// Ensures that the provided <paramref name="observer"/> is subscribed, renewing its subscription.
    /// </summary>
    /// <param name="id">
    /// The observer's identity.
    /// </param>
    /// <param name="observer">
    /// The observer.
    /// </param>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public void Subscribe(TIdentity id, TObserver observer)
    {
        _ = TryGetOrAddSubscription(id, observer, out _);
    }

    /// <summary>
    /// Ensures that the provided <paramref name="id"/> is unsubscribed.
    /// </summary>
    /// <param name="id">
    /// The observer.
    /// </param>
    public void Unsubscribe(TIdentity id)
    {
        _observers.TryRemove(id, out _);
        if (_log.IsEnabled(LogLevel.Trace))
        {
            var count = _observers.Count;
            _log.LogTrace("Removed entry for {Id}. {Count} total observers after remove.", id, count);
        }
    }

    /// <summary>
    /// Notifies all observers.
    /// </summary>
    /// <param name="notification">
    /// The notification delegate to call on each observer.
    /// </param>
    /// <param name="predicate">
    /// The predicate used to select observers to notify.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the work performed.
    /// </returns>
    public async Task NotifyAsync(Func<TObserver, Task> notification, Func<TIdentity, TObserver, bool>? predicate = null)
    {
        // Use tuple deconstruction to avoid KeyValuePair struct copying
        foreach (var (id, observer) in _observers)
        {
            // Skip observers which don't match the provided predicate.
            if (predicate is not null && !predicate(id, observer.Observer))
            {
                continue;
            }

            try
            {
                await notification(observer).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and removed immediately.
                // Use TryRemove directly to avoid allocating a defunct list.
                _observers.TryRemove(id, out _);
                if (_log.IsEnabled(LogLevel.Trace))
                {
                    var count = _observers.Count;
                    _log.LogTrace("Removing defunct entry for {Id}. {Count} total observers after remove.", id, count);
                }
            }
        }
    }

    /// <summary>
    /// Notifies a single observer identified by <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    /// The identity of the observer to notify. This should correspond to the key used in the observer manager.
    /// </param>
    /// <param name="notification">
    /// The delegate to invoke on the observer.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous notification operation.
    /// </returns>
    /// <remarks>
    /// If the observer fails during the notification, it is considered defunct and removed from the collection.
    /// This method performs an O(1) lookup by <paramref name="id"/>, making it more efficient than 
    /// using <see cref="NotifyAsync(Func{TObserver, Task}, Func{TIdentity, TObserver, bool}?)"/> with a predicate.
    /// </remarks>
    public async Task NotifyAsync(TIdentity id, Func<TObserver, Task> notification)
    {
        if (_observers.TryGetValue(id, out var entry))
        {
            try
            {
                await notification(entry.Observer).ConfigureAwait(false);
            }
            catch (Exception)
            {
                _observers.TryRemove(id, out _);
                if (_log.IsEnabled(LogLevel.Trace))
                {
                    var count = _observers.Count;
                    _log.LogTrace("Removing defunct entry for {Id}. {Count} total observers after remove.", id, count);
                }
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<TObserver> GetEnumerator()
    {
        foreach (var (_, observer) in _observers)
        {
            yield return observer;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly record struct Entry(TObserver Observer, bool WasExisting)
    {
        public static implicit operator TObserver(Entry entry) => entry.Observer;
    }
}
