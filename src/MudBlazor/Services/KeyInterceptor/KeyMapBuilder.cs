// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// A fluent builder for creating declarative key command mappings.
/// Supports conditional execution and efficient command dispatching.
/// </summary>
public sealed class KeyMapBuilder
{
    private readonly List<IKeyCommand> _commands = [];

    /// <summary>
    /// Maps multiple keys to a single action on key down.
    /// </summary>
    public KeyMapBuilder OnKeyDownAny(IEnumerable<string> keys, Func<Task> action)
    {
        _commands.Add(new MultiKeyCommand(KeyEventKind.Down, keys, action));
        return this;
    }

    /// <summary>
    /// Maps multiple keys to a single action on key down that receives the keyboard event args.
    /// </summary>
    public KeyMapBuilder OnKeyDownAny(IEnumerable<string> keys, Func<KeyboardEventArgs, Task> action)
    {
        _commands.Add(new MultiKeyCommandWithArgs(KeyEventKind.Down, keys, action));
        return this;
    }

    /// <summary>
    /// Maps multiple keys to a single action on key up.
    /// </summary>
    public KeyMapBuilder OnKeyUpAny(IEnumerable<string> keys, Func<Task> action)
    {
        _commands.Add(new MultiKeyCommand(KeyEventKind.Up, keys, action));
        return this;
    }

    /// <summary>
    /// Maps multiple keys to a single action on key up that receives the keyboard event args.
    /// </summary>
    public KeyMapBuilder OnKeyUpAny(IEnumerable<string> keys, Func<KeyboardEventArgs, Task> action)
    {
        _commands.Add(new MultiKeyCommandWithArgs(KeyEventKind.Up, keys, action));
        return this;
    }

    /// <summary>
    /// Maps a single key to an action on key down, with optional condition.
    /// </summary>
    /// <param name="key">The key to handle.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="when">Optional condition that must be true for the command to execute.</param>
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

    /// <summary>
    /// Maps a single key to an action on key down that receives the keyboard event args.
    /// Use this when you need access to modifier keys or other event details.
    /// </summary>
    /// <param name="key">The key to handle.</param>
    /// <param name="action">The action to execute, receiving KeyboardEventArgs.</param>
    /// <param name="when">Optional condition that must be true for the command to execute.</param>
    public KeyMapBuilder OnKeyDown(string key, Func<KeyboardEventArgs, Task> action, Func<bool>? when = null)
    {
        IKeyCommand command = new KeyCommandWithArgs(KeyEventKind.Down, key, action);

        if (when is not null)
        {
            command = new ConditionalCommand(command, when);
        }

        _commands.Add(command);
        return this;
    }

    /// <summary>
    /// Maps a single key to an action on key up, with optional condition.
    /// </summary>
    /// <param name="key">The key to handle.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="when">Optional condition that must be true for the command to execute.</param>
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

    /// <summary>
    /// Maps a single key to an action on key up that receives the keyboard event args.
    /// Use this when you need access to modifier keys or other event details.
    /// </summary>
    /// <param name="key">The key to handle.</param>
    /// <param name="action">The action to execute, receiving KeyboardEventArgs.</param>
    /// <param name="when">Optional condition that must be true for the command to execute.</param>
    public KeyMapBuilder OnKeyUp(string key, Func<KeyboardEventArgs, Task> action, Func<bool>? when = null)
    {
        IKeyCommand command = new KeyCommandWithArgs(KeyEventKind.Up, key, action);

        if (when is not null)
        {
            command = new ConditionalCommand(command, when);
        }

        _commands.Add(command);

        return this;
    }

    /// <summary>
    /// Maps multiple keys to an action on key down with a shared condition.
    /// More efficient than calling OnKeyDown multiple times with the same condition.
    /// </summary>
    public KeyMapBuilder OnKeyDownAny(IEnumerable<string> keys, Func<Task> action, Func<bool> when)
    {
        _commands.Add(new ConditionalCommand(new MultiKeyCommand(KeyEventKind.Down, keys, action), when));
        return this;
    }

    /// <summary>
    /// Creates a conditional scope where all commands share the same condition.
    /// This is more efficient than adding the condition to each command individually.
    /// </summary>
    public KeyMapBuilder When(Func<bool> condition, Action<KeyMapBuilder> configure)
    {
        var scopedBuilder = new KeyMapBuilder();
        configure(scopedBuilder);

        foreach (var command in scopedBuilder._commands)
        {
            _commands.Add(new ConditionalCommand(command, condition));
        }

        return this;
    }

    public (IKeyDownObserver, IKeyUpObserver) Build()
    {
        if (_commands.Count == 0)
        {
            return (KeyObserver.KeyDownIgnore(), KeyObserver.KeyUpIgnore());
        }

        var observer = new KeyCommandObserver(_commands);
        return (observer, observer);
    }

    public static KeyMapBuilder Create() => new();

    private sealed class SimpleKeyCommand(KeyEventKind kind, string key, Func<Task> action) : IKeyCommand
    {
        private readonly Regex? _regex = ParseRegex(key);
        
        public KeyEventKind Kind { get; } = kind;

        public bool CanExecute(KeyboardEventArgs args)
            => _regex?.IsMatch(args.Key) ?? args.Key == key;

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action();
            
        private static Regex? ParseRegex(string key)
        {
            // Check if key is a regex pattern like "/pattern/"
            if (key.Length > 2 && key.StartsWith('/') && key.EndsWith('/'))
            {
                try
                {
                    return new Regex(key.Substring(1, key.Length - 2));
                }
                catch
                {
                    // Invalid regex, fall back to literal matching
                    return null;
                }
            }
            return null;
        }
    }

    private sealed class KeyCommandWithArgs(KeyEventKind kind, string key, Func<KeyboardEventArgs, Task> action) : IKeyCommand
    {
        private readonly Regex? _regex = ParseRegex(key);
        
        public KeyEventKind Kind { get; } = kind;

        public bool CanExecute(KeyboardEventArgs args)
            => _regex?.IsMatch(args.Key) ?? args.Key == key;

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action(args);
            
        private static Regex? ParseRegex(string key)
        {
            // Check if key is a regex pattern like "/pattern/"
            if (key.Length > 2 && key.StartsWith('/') && key.EndsWith('/'))
            {
                try
                {
                    return new Regex(key.Substring(1, key.Length - 2));
                }
                catch
                {
                    // Invalid regex, fall back to literal matching
                    return null;
                }
            }
            return null;
        }
    }

    private sealed class MultiKeyCommand : IKeyCommand
    {
        private readonly HashSet<string> _keys = [];
        private readonly List<Regex> _regexes = [];
        private readonly Func<Task> _action;

        public KeyEventKind Kind { get; }

        public MultiKeyCommand(KeyEventKind kind, IEnumerable<string> keys, Func<Task> action)
        {
            Kind = kind;
            _action = action;
            
            foreach (var key in keys)
            {
                var regex = ParseRegex(key);
                if (regex != null)
                {
                    _regexes.Add(regex);
                }
                else
                {
                    _keys.Add(key);
                }
            }
        }

        public bool CanExecute(KeyboardEventArgs args)
        {
            if (_keys.Contains(args.Key))
                return true;
                
            foreach (var regex in _regexes)
            {
                if (regex.IsMatch(args.Key))
                    return true;
            }
            
            return false;
        }

        public Task ExecuteAsync(KeyboardEventArgs args)
            => _action();
            
        private static Regex? ParseRegex(string key)
        {
            // Check if key is a regex pattern like "/pattern/"
            if (key.Length > 2 && key.StartsWith('/') && key.EndsWith('/'))
            {
                try
                {
                    return new Regex(key.Substring(1, key.Length - 2));
                }
                catch
                {
                    // Invalid regex, fall back to literal matching
                    return null;
                }
            }
            return null;
        }
    }

    private sealed class MultiKeyCommandWithArgs : IKeyCommand
    {
        private readonly HashSet<string> _keys = [];
        private readonly List<Regex> _regexes = [];
        private readonly Func<KeyboardEventArgs, Task> _action;

        public KeyEventKind Kind { get; }

        public MultiKeyCommandWithArgs(KeyEventKind kind, IEnumerable<string> keys, Func<KeyboardEventArgs, Task> action)
        {
            Kind = kind;
            _action = action;
            
            foreach (var key in keys)
            {
                var regex = ParseRegex(key);
                if (regex != null)
                {
                    _regexes.Add(regex);
                }
                else
                {
                    _keys.Add(key);
                }
            }
        }

        public bool CanExecute(KeyboardEventArgs args)
        {
            if (_keys.Contains(args.Key))
                return true;
                
            foreach (var regex in _regexes)
            {
                if (regex.IsMatch(args.Key))
                    return true;
            }
            
            return false;
        }

        public Task ExecuteAsync(KeyboardEventArgs args)
            => _action(args);
            
        private static Regex? ParseRegex(string key)
        {
            // Check if key is a regex pattern like "/pattern/"
            if (key.Length > 2 && key.StartsWith('/') && key.EndsWith('/'))
            {
                try
                {
                    return new System.Text.RegularExpressions.Regex(key.Substring(1, key.Length - 2));
                }
                catch
                {
                    // Invalid regex, fall back to literal matching
                    return null;
                }
            }
            return null;
        }
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
