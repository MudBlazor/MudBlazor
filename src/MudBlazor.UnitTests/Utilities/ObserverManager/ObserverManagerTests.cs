// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Utilities.ObserverManager;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.ObserverManager;

#nullable enable
[TestFixture]
public class ObserverManagerTests
{
    private ObserverManager<int, string> _observerManager = new(NullLogger<ObserverManager<int, string>>.Instance);

    [SetUp]
    public void Setup()
    {
        //Reset on each test
        _observerManager = new ObserverManager<int, string>(NullLogger<ObserverManager<int, string>>.Instance);
    }

    [Test]
    public void Subscribe_AddsObserverToDictionary()
    {
        // Arrange
        var id = 1;
        var observer = "Observer1";

        // Act
        _observerManager.Subscribe(id, observer);

        // Assert
        Assert.AreEqual(1, _observerManager.Count);
        Assert.AreEqual(observer, _observerManager.Observers[id]);
    }

    [Test]
    public void Subscribe_UpdatesExistingObserverInDictionary()
    {
        // Arrange
        var id = 1;
        var observer1 = "Observer1";
        var observer2 = "Observer2";

        // Act
        _observerManager.Subscribe(id, observer1);
        _observerManager.Subscribe(id, observer2);

        // Assert
        Assert.AreEqual(1, _observerManager.Count);
        Assert.AreEqual(observer2, _observerManager.Observers[id]);
    }

    [Test]
    public void Unsubscribe_RemovesObserverFromDictionary()
    {
        // Arrange
        var id = 1;
        var observer = "Observer1";
        _observerManager.Subscribe(id, observer);

        // Act
        _observerManager.Unsubscribe(id);

        // Assert
        Assert.AreEqual(0, _observerManager.Count);
        Assert.IsFalse(_observerManager.Observers.ContainsKey(id));
    }

    [Test]
    public async Task NotifyAsync_CallsNotificationForEachObserver()
    {
        // Arrange
        var observer1 = "Observer1";
        var observer2 = "Observer2";
        var notificationCalledCount = 0;

        _observerManager.Subscribe(1, observer1);
        _observerManager.Subscribe(2, observer2);

        // Act
        async Task NotificationAsync(string _)
        {
            notificationCalledCount++;
            await Task.Delay(10); // Simulate some async work
        }

        await _observerManager.NotifyAsync(NotificationAsync);

        // Assert
        Assert.AreEqual(2, notificationCalledCount);
    }

    [Test]
    public async Task NotifyAsync_RemovesDefunctObservers()
    {
        // Arrange
        var observer1 = "Observer1";
        var observer2 = "Observer2";
        var observer3 = "Observer3";

        _observerManager.Subscribe(1, observer1);
        _observerManager.Subscribe(2, observer2);
        _observerManager.Subscribe(3, observer3);

        // Act
        Task NotificationAsync(string observer)
        {
            if (observer == observer2)
            {
                throw new Exception("Notification failed");
            }

            return Task.CompletedTask;
        }

        await _observerManager.NotifyAsync(NotificationAsync);

        // Assert
        Assert.AreEqual(2, _observerManager.Count);
        Assert.IsTrue(_observerManager.Observers.ContainsKey(1));
        Assert.IsTrue(_observerManager.Observers.ContainsKey(3));
        Assert.IsFalse(_observerManager.Observers.ContainsKey(2));
    }

    [Test]
    public void Clear_ClearsAllObservers()
    {
        // Arrange
        _observerManager.Subscribe(1, "Observer1");
        _observerManager.Subscribe(2, "Observer2");
        _observerManager.Subscribe(3, "Observer3");

        // Act
        _observerManager.Clear();

        // Assert
        Assert.AreEqual(0, _observerManager.Count, "Count should be 0 after clearing.");
        CollectionAssert.IsEmpty(_observerManager.Observers, "Observers collection should be empty after clearing.");
    }

    [Test]
    public void GetEnumerator_ReturnsAllObservers()
    {
        // Expected
        var expectedObservers = new List<string> { "Observer1", "Observer2", "Observer3" };

        // Arrange
        for (var i = 0; i < expectedObservers.Count; i++)
        {
            _observerManager.Subscribe(i + 1, expectedObservers[i]);
        }

        // Act
        var actualObservers = new List<string>();
        using (var enumerator = _observerManager.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                actualObservers.Add(enumerator.Current);
            }
        }

        // Assert
        CollectionAssert.AreEqual(expectedObservers, actualObservers, "Enumerated observers should match the expected observers.");
    }

    [Test]
    public void GetEnumeratorNonGeneric_ReturnsAllObservers()
    {
        // Expected
        var expectedObservers = new List<string> { "Observer1", "Observer2", "Observer3" };

        // Arrange
        for (var i = 0; i < expectedObservers.Count; i++)
        {
            _observerManager.Subscribe(i + 1, expectedObservers[i]);
        }

        // Act
        var actualObservers = new List<string>();
        var enumerator = ((IEnumerable)_observerManager).GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is string observer)
            {
                actualObservers.Add(observer);
            }
        }

        // Assert
        CollectionAssert.AreEqual(expectedObservers, actualObservers, "Enumerated observers should match the expected observers.");
    }

    [Test]
    public void IEnumerable_GetEnumerator_ReturnsAllObservers()
    {
        // Expected
        var expectedObservers = new List<string> { "Observer1", "Observer2", "Observer3" };

        // Arrange
        for (var i = 0; i < expectedObservers.Count; i++)
        {
            _observerManager.Subscribe(i + 1, expectedObservers[i]);
        }

        // Act
        var actualObservers = _observerManager.ToList();

        // Assert
        CollectionAssert.AreEqual(expectedObservers, actualObservers, "Enumerated observers should match the expected observers.");
    }


    [Test]
    public void Unsubscribe_Subscribe_UpdateSubscribe_DebugLogEnabled_LogsDebugInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        loggerMock.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

        var observerManager = new ObserverManager<int, string>(loggerMock.Object);

        const int Id = 1;
        const string Observer = "Observer1";

        // Act
        observerManager.Subscribe(Id, Observer);
        observerManager.Subscribe(Id, Observer);
        observerManager.Unsubscribe(Id);

        // Assert
        loggerMock
            .VerifyLogging($"Adding entry for {Id}/{Observer}. 1 total observers after add.")
            .VerifyLogging($"Updating entry for {Id}/{Observer}. 1 total observers.")
            .VerifyLogging($"Removed entry for {Id}. 0 total observers after remove.");
    }

    [Test]
    public async Task NotifyAsync_DefunctObserver_LogsDebugInformation()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        loggerMock.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

        var observerManager = new ObserverManager<int, string>(loggerMock.Object);

        const int DefunctObserverId = 1;
        const string DefunctObserver = "DefunctObserver";

        observerManager.Subscribe(DefunctObserverId, DefunctObserver);

        bool Predicate(string observer) => observer == DefunctObserver;

        async Task NotificationAsync(string observer)
        {
            await Task.Delay(10); // Simulating some asynchronous operation
            throw new Exception("Simulated exception");
        }

        // Act
        await observerManager.NotifyAsync(NotificationAsync, Predicate);

        // Assert
        loggerMock
            .VerifyLogging($"Adding entry for {DefunctObserverId}/{DefunctObserver}. 1 total observers after add.")
            .VerifyLogging($"Removing defunct entry for {DefunctObserverId}. 0 total observers after remove.");
    }
}
