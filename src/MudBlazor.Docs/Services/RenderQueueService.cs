// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using MudBlazor.Docs.Components;

namespace MudBlazor.Docs.Services;

/// <summary>
/// Implements deferred rendering features for a service.
/// </summary>
public interface IRenderQueueService
{
    /// <summary>
    /// Queues a component for rendering.
    /// </summary>
    /// <param name="component">The content to render.</param>
    Task Enqueue(QueuedContent component);

    /// <summary>
    /// The number of sections rendered immediately before being deferred.
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Waits for all pending content to finish rendering.
    /// </summary>
    Task WaitUntilEmpty();

    /// <summary>
    /// Clears all pending render operations.
    /// </summary>
    void Clear();
}

/// <summary>
/// A service for rendering queued content.
/// </summary>
public class RenderQueueService : IRenderQueueService
{
    private CancellationTokenSource _cancelRenderSource = new();
    private CancellationToken _cancelToken;
    private Queue<QueuedContent> _components = [];
    private Task _renderTask;
    private int count;

    /// <summary>
    /// The number of sections rendered immediately before being deferred.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>2</c>.  Should be the number of sections guaranteed to fill the height of the browser.
    /// </remarks>
    public int Capacity { get; init; } = 2;

    /// <inheritdoc />
    public void Clear()
    {
        // Cancel any pending renders
        _cancelRenderSource.Cancel();
        // Reset the queue
        _components.Clear();
        count = 0;
        // Start a new token to cancel the next queue
        _cancelRenderSource = new();
        _cancelToken = _cancelRenderSource.Token;
    }

    /// <inheritdoc />
    public async Task Enqueue(QueuedContent component)
    {
        count++;
        // Should we defer rendering?
        if (count <= Capacity)
        {
            // No. Show the content immediately
            await component.ShowAsync(_cancelToken);
        }
        else
        {
            // Is this the first deferred section?
            if (_components.Count == 0)
            {
                // Yes.  Start processing the queue
                _renderTask = BeginRenderAsync();
            }
            // Yes.  Render it later
            _components.Enqueue(component);
        }
    }

    /// <summary>
    /// Begins displaying deferred sections.
    /// </summary>
    public async Task BeginRenderAsync()
    {
        // Let the first page render occur
        await Task.Delay(500, _cancelToken);
        // Now process all queued sections
        while (_components.TryDequeue(out var component))
        {
            // Show the section
            await component.ShowAsync(_cancelToken);
            // Let the render occur for a moment
            await Task.Delay(10, _cancelToken);
        }
    }

    /// <inheritdoc />
    public Task WaitUntilEmpty() => _renderTask;
}
