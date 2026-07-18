// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <inheritdoc cref="IOverlayService" />
internal sealed class OverlayService : IOverlayService
{
    private readonly object _syncRoot = new();

    // Ordered by registration; the last entry is the most recently opened (top-most) overlay.
    private readonly List<Registration> _registrations = new();

    /// <inheritdoc />
    public bool HasVisibleOverlay
    {
        get
        {
            lock (_syncRoot)
            {
                return _registrations.Count > 0;
            }
        }
    }

    /// <inheritdoc />
    public IDisposable RegisterOverlay(Func<Task> closeAsync)
    {
        ArgumentNullException.ThrowIfNull(closeAsync);

        var registration = new Registration(this, closeAsync);
        lock (_syncRoot)
        {
            _registrations.Add(registration);
        }

        return registration;
    }

    /// <inheritdoc />
    public async Task<bool> CloseLastOverlayAsync()
    {
        Registration? top;
        lock (_syncRoot)
        {
            if (_registrations.Count == 0)
            {
                return false;
            }

            top = _registrations[^1];

            // Remove eagerly so state is consistent even if the overlay never re-renders to unregister itself.
            // The overlay also disposes its token as it hides, but Unregister is idempotent, so that is a no-op.
            _registrations.RemoveAt(_registrations.Count - 1);
        }

        // Runs the overlay's own close path (Visible = false + OnClosed), the same as an outside click.
        await top.CloseAsync();

        return true;
    }

    private void Unregister(Registration registration)
    {
        lock (_syncRoot)
        {
            _registrations.Remove(registration);
        }
    }

    private sealed class Registration : IDisposable
    {
        private readonly OverlayService _owner;
        private readonly Func<Task> _closeAsync;

        public Registration(OverlayService owner, Func<Task> closeAsync)
        {
            _owner = owner;
            _closeAsync = closeAsync;
        }

        public Task CloseAsync() => _closeAsync();

        public void Dispose() => _owner.Unregister(this);
    }
}
