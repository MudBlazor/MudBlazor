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
/// </remarks>
internal class ObserverManager<TIdentity, TObserver> : IEnumerable<TObserver> where TIdentity : notnull
{
    private readonly ConcurrentDictionary<TIdentity, ObserverEntry> _observers = new();
    private readonly ILogger _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObserverManager{TIdentity,TObserver}"/> class. 
    /// </summary>
    public ObserverManager(ILogger log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    public int Count => _observers.Count;

    /// <summary>
    /// Gets a copy of the observers.
    /// </summary>
    public IDictionary<TIdentity, TObserver> Observers => _observers.ToDictionary(_ => _.Key, _ => _.Value.Observer);

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
    public IEnumerable<TIdentity> FindObserverIdentities(Func<TIdentity, TObserver, bool> predicate) =>
        _observers.Where(kvp => predicate(kvp.Key, kvp.Value.Observer)).Select(kvp => kvp.Key);

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
        // Add or update the subscription.
        if (_observers.TryGetValue(id, out var entry))
        {
            entry.Observer = observer;
            if (_log.IsEnabled(LogLevel.Trace))
            {
                _log.LogTrace("Updating entry for {Id}/{Observer}. {Count} total observers.", id, observer, _observers.Count);
            }
        }
        else
        {
            _observers[id] = new ObserverEntry(observer);
            if (_log.IsEnabled(LogLevel.Trace))
            {
                _log.LogTrace("Adding entry for {Id}/{Observer}. {Count} total observers after add.", id, observer, _observers.Count);
            }
        }
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
        var defunct = default(List<TIdentity>);
        foreach (var observer in _observers)
        {
            // Skip observers which don't match the provided predicate.
            if (predicate != null && !predicate(observer.Key, observer.Value.Observer))
            {
                continue;
            }

            try
            {
                await notification(observer.Value.Observer);
            }
            catch (Exception)
            {
                // Failing observers are considered defunct and will be removed..
                defunct ??= new List<TIdentity>();
                defunct.Add(observer.Key);
            }
        }

        // Remove defunct observers.
        if (defunct != default(List<TIdentity>))
        {
            foreach (var observer in defunct)
            {
                _observers.Remove(observer, out _);
                if (_log.IsEnabled(LogLevel.Trace))
                {
                    _log.LogTrace("Removing defunct entry for {Id}. {Count} total observers after remove.", observer, _observers.Count);
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
    public IEnumerator<TObserver> GetEnumerator() => _observers.Select(observer => observer.Value.Observer).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// An observer entry.
    /// </summary>
    private class ObserverEntry
    {
        /// <summary>
        /// Gets or sets the observer.
        /// </summary>
        public TObserver Observer { get; set; }

        public ObserverEntry(TObserver observer)
        {
            Observer = observer;
        }
    }
}
