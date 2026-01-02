// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace MudBlazor.Utilities.ObserverManager;

#nullable enable
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
/// Optimized for performance with minimal memory allocations:
/// - Observers are stored directly in the dictionary without wrapper objects
/// - Lazy allocation of defunct observer lists (only when failures occur)
/// - Direct iteration patterns to avoid LINQ overhead
/// </remarks>
internal class ObserverManager<TIdentity, TObserver> : IEnumerable<TObserver> where TIdentity : notnull
{
    private readonly ConcurrentDictionary<TIdentity, TObserver> _observers;
    private readonly ILogger _log;

    /// <summary>
    /// Initial capacity for the defunct observers list.
    /// This value (4) is chosen to handle a small number of failures without resizing,
    /// while keeping the allocation small since observer failures are rare.
    /// </summary>
    private const int DefunctListInitialCapacity = 4;

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
            foreach (var kvp in _observers)
            {
                result[kvp.Key] = kvp.Value;
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
        return _observers.TryGetValue(id, out observer);
    }

    /// <summary>
    /// Finds the identities of observers that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the observers.</param>
    /// <returns>An enumerable collection of observer identities that match the predicate.</returns>
    public IEnumerable<TIdentity> FindObserverIdentities(Func<TIdentity, TObserver, bool> predicate)
    {
        foreach (var kvp in _observers)
        {
            if (predicate(kvp.Key, kvp.Value))
            {
                yield return kvp.Key;
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
    /// This method needs to determine if the observer existed before the update.
    /// We use TryGetValue before the indexer assignment to detect existence,
    /// since the indexer assignment doesn't provide this information.
    /// </remarks>
    public bool TryGetOrAddSubscription(TIdentity id, TObserver observer, out TObserver newObserver)
    {
        // Check if observer exists before unconditionally setting it.
        // We need to know if it existed for the return value and logging.
        var existed = _observers.TryGetValue(id, out _);
        
        // Always set the observer (add or update)
        _observers[id] = observer;
        
        if (_log.IsEnabled(LogLevel.Trace))
        {
            if (existed)
            {
                _log.LogTrace("Updating entry for {Id}/{Observer}. {Count} total observers.", id, observer, _observers.Count);
            }
            else
            {
                _log.LogTrace("Adding entry for {Id}/{Observer}. {Count} total observers after add.", id, observer, _observers.Count);
            }
        }

        newObserver = observer;
        return existed;
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
        _observers.Remove(id, out _);
        if (_log.IsEnabled(LogLevel.Trace))
        {
            _log.LogTrace("Removed entry for {Id}. {Count} total observers after remove.", id, _observers.Count);
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
        List<TIdentity>? defunct = null;

        foreach (var observer in _observers)
        {
            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Key, observer.Value))
            {
                continue;
            }

            try
            {
                await notification(observer.Value);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed.
                // Lazy allocation with small initial capacity to reduce resize operations.
                defunct ??= new List<TIdentity>(DefunctListInitialCapacity);
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != null)
        {
            foreach (var id in defunct)
            {
                _observers.Remove(id, out _);
                if (_log.IsEnabled(LogLevel.Trace))
                {
                    _log.LogTrace("Removing defunct entry for {Id}. {Count} total observers after remove.", id, _observers.Count);
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
        foreach (var kvp in _observers)
        {
            yield return kvp.Value;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
