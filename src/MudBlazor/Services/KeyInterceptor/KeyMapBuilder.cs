// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

#nullable enable
public sealed class KeyMapBuilder
{
    private readonly List<IKeyCommand> _commands = [];

    public KeyMapBuilder OnKeyDownAny(IEnumerable<string> keys, Func<Task> action)
    {
        _commands.Add(new MultiKeyCommand(KeyEventKind.Down, keys, action));
        return this;
    }

    public KeyMapBuilder OnKeyUpAny(IEnumerable<string> keys, Func<Task> action)
    {
        _commands.Add(new MultiKeyCommand(KeyEventKind.Up, keys, action));
        return this;
    }

    public KeyMapBuilder OnKeyDown(string key, Func<Task> action, Func<bool>? when = null)
    {
        IKeyCommand command = new SimpleKeyCommand(KeyEventKind.Down, key, action);

        if (when is not null)
        {
            command = new ConditionalCommand(command, when);
        }

        _commands.Add(command);
        return this;
    }

    public KeyMapBuilder OnKeyUp(string key, Func<Task> action, Func<bool>? when = null)
    {
        IKeyCommand command = new SimpleKeyCommand(KeyEventKind.Up, key, action);

        if (when is not null)
        {
            command = new ConditionalCommand(command, when);
        }

        _commands.Add(command);

        return this;
    }

    public KeyMapBuilder OnKeyDownAny(IEnumerable<string> keys, Func<Task> action, Func<bool> when)
    {
        foreach (var key in keys)
        {
            OnKeyDown(key, action, when);
        }
        return this;
    }

    public (IKeyDownObserver?, IKeyUpObserver?) Build()
    {
        if (_commands.Count == 0)
        {
            return (null, null);
        }

        var observer = new KeyCommandObserver(_commands);
        return (observer, observer);
    }

    public static KeyMapBuilder Create() => new();

    private sealed class SimpleKeyCommand(KeyEventKind kind, string key, Func<Task> action) : IKeyCommand
    {
        public KeyEventKind Kind { get; } = kind;

        public bool CanExecute(KeyboardEventArgs args)
            => args.Key == key;

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action();
    }

    private sealed class MultiKeyCommand(KeyEventKind kind, IEnumerable<string> keys, Func<Task> action)
        : IKeyCommand
    {
        private readonly HashSet<string> _keys = keys.ToHashSet();

        public KeyEventKind Kind { get; } = kind;

        public bool CanExecute(KeyboardEventArgs args)
            => _keys.Contains(args.Key);

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action();
    }

    private sealed class ConditionalCommand(IKeyCommand inner, Func<bool> condition) : IKeyCommand
    {
        public KeyEventKind Kind => inner.Kind;

        public bool CanExecute(KeyboardEventArgs args)
            => condition() && inner.CanExecute(args);

        public Task ExecuteAsync(KeyboardEventArgs args)
            => inner.ExecuteAsync(args);
    }
}
