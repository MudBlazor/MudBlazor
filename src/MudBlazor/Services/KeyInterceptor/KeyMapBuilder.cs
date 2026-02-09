// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Services;

/// <summary>
/// Fluent API for building key-command maps that the key interceptor can execute efficiently.
/// </summary>
/// <remarks>
/// Use this builder when you want readable, declarative keyboard shortcuts and a single observer that can be registered with <see cref="IKeyInterceptorService"/>. It compiles user-friendly declarations into a minimal command list optimized for dispatch.
/// </remarks>
public sealed class KeyMapBuilder
{
    private int _hookCount;
    private readonly List<IKeyCommand> _commands = [];

    /// <summary>
    /// Helper to parse regex patterns from key strings.
    /// </summary>
    private static Regex? ParseRegexPattern(string key)
    {
        // Check if key is a regex pattern like "/pattern/"
        if (key.Length > 2 && key.StartsWith('/') && key.EndsWith('/'))
        {
            try
            {
                return new Regex(key.Substring(1, key.Length - 2), RegexOptions.None, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                // Invalid regex, fall back to literal matching
                return null;
            }
        }

        return null;
    }

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
    /// Hooks within the scope are still inserted at the beginning to ensure they execute first.
    /// </summary>
    public KeyMapBuilder When(Func<bool> condition, Action<KeyMapBuilder> configure)
    {
        var scopedBuilder = new KeyMapBuilder();
        configure(scopedBuilder);

        // Separate hooks from regular commands
        var hooks = new List<IKeyCommand>();
        var regularCommands = new List<IKeyCommand>();

        foreach (var command in scopedBuilder._commands)
        {
            if (command.IsHook)
            {
                hooks.Add(command);
            }
            else
            {
                regularCommands.Add(command);
            }
        }

        // Insert hooks at the current hook position (maintaining declaration order)
        foreach (var hook in hooks)
        {
            _commands.Insert(_hookCount, new ConditionalCommand(hook, condition));
            _hookCount++;
        }

        // Add regular commands at the end
        foreach (var command in regularCommands)
        {
            _commands.Add(new ConditionalCommand(command, condition));
        }

        return this;
    }

    /// <summary>
    /// Registers a hook that will be called for every key down event, allowing subsequent commands to also execute.
    /// Unlike regular commands which stop the command chain after execution, hooks do not stop the chain.
    /// This is useful for maintaining virtual method override patterns while using the KeyCommand API.
    /// </summary>
    /// <remarks>
    /// <para><strong>Important:</strong> Hooks are always executed before regular commands, regardless of their declaration order. You can declare hooks anywhere in the builder chain, and they will be automatically moved to execute first.</para>
    /// <para><strong>Example usage:</strong></para>
    /// <code>
    /// await KeyInterceptorService.SubscribeAsync(elementId, options, keys => keys
    ///     .OnKeyDown("Backspace", HandleBackspaceAsync)  // Regular commands
    ///     .HookKeyDown(OnHandleKeyDownAsync)  // Hook still executes first for ALL keys
    ///     .When(CanHandleKeys, builder => builder
    ///         .OnKeyDownAny(["Escape", "Tab"], CloseAsync)));
    /// </code>
    /// <para>The hook receives the KeyboardEventArgs and can perform any necessary processing (e.g., calling a virtual method that derived classes can override). The hook will execute for every key down event, regardless of whether any specific key commands match.</para>
    /// <para>If a hook is placed inside a <c>When()</c> scope, it will still respect the condition - it won't execute unconditionally.</para>
    /// </remarks>
    /// <param name="hook">The method to call on every key down event.</param>
    /// <returns>The builder for chaining.</returns>
    public KeyMapBuilder HookKeyDown(Func<KeyboardEventArgs, Task> hook)
    {
        _commands.Insert(_hookCount, new HookCommand(KeyEventKind.Down, hook));
        _hookCount++;
        return this;
    }

    /// <summary>
    /// Registers a hook that will be called for every key up event, allowing subsequent commands to also execute.
    /// Unlike regular commands which stop the command chain after execution, hooks do not stop the chain.
    /// This is useful for maintaining virtual method override patterns while using the KeyCommand API.
    /// </summary>
    /// <remarks>
    /// <para><strong>Important:</strong> Hooks are always executed before regular commands, regardless of their declaration order. You can declare hooks anywhere in the builder chain, and they will be automatically moved to execute first.</para>
    /// <para><strong>Example usage:</strong></para>
    /// <code>
    /// await KeyInterceptorService.SubscribeAsync(elementId, options, keys => keys
    ///     .OnKeyUp("Enter", HandleEnterAsync)  // Regular commands
    ///     .HookKeyUp(OnHandleKeyUpAsync)  // Hook still executes first for ALL keys
    ///     .When(CanHandleKeys, builder => builder
    ///         .OnKeyUpAny(["Escape", "Tab"], CloseAsync)));
    /// </code>
    /// <para>The hook receives the KeyboardEventArgs and can perform any necessary processing (e.g., calling a virtual method that derived classes can override). The hook will execute for every key up event, regardless of whether any specific key commands match.</para>
    /// <para>If a hook is placed inside a <c>When()</c> scope, it will still respect the condition - it won't execute unconditionally.</para>
    /// </remarks>
    /// <param name="hook">The method to call on every key up event.</param>
    /// <returns>The builder for chaining.</returns>
    public KeyMapBuilder HookKeyUp(Func<KeyboardEventArgs, Task> hook)
    {
        _commands.Insert(_hookCount, new HookCommand(KeyEventKind.Up, hook));
        _hookCount++;
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
        private readonly Regex? _regex = ParseRegexPattern(key);

        public KeyEventKind Kind { get; } = kind;

        public bool IsHook => false;

        public bool CanExecute(KeyboardEventArgs args)
            => _regex?.IsMatch(args.Key) ?? args.Key == key;

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action();
    }

    private sealed class KeyCommandWithArgs(KeyEventKind kind, string key, Func<KeyboardEventArgs, Task> action) : IKeyCommand
    {
        private readonly Regex? _regex = ParseRegexPattern(key);

        public KeyEventKind Kind { get; } = kind;

        public bool IsHook => false;

        public bool CanExecute(KeyboardEventArgs args)
            => _regex?.IsMatch(args.Key) ?? args.Key == key;

        public Task ExecuteAsync(KeyboardEventArgs args)
            => action(args);
    }

    private sealed class MultiKeyCommand : IKeyCommand
    {
        private readonly HashSet<string> _keys = [];
        private readonly List<Regex> _regexes = [];
        private readonly Func<Task> _action;

        public KeyEventKind Kind { get; }

        public bool IsHook => false;

        public MultiKeyCommand(KeyEventKind kind, IEnumerable<string> keys, Func<Task> action)
        {
            Kind = kind;
            _action = action;

            foreach (var key in keys)
            {
                var regex = ParseRegexPattern(key);
                if (regex is not null)
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
            {
                return true;
            }

            foreach (var regex in _regexes)
            {
                if (regex.IsMatch(args.Key))
                {
                    return true;
                }
            }

            return false;
        }

        public Task ExecuteAsync(KeyboardEventArgs args)
            => _action();
    }

    private sealed class MultiKeyCommandWithArgs : IKeyCommand
    {
        private readonly HashSet<string> _keys = [];
        private readonly List<Regex> _regexes = [];
        private readonly Func<KeyboardEventArgs, Task> _action;

        public KeyEventKind Kind { get; }

        public bool IsHook => false;

        public MultiKeyCommandWithArgs(KeyEventKind kind, IEnumerable<string> keys, Func<KeyboardEventArgs, Task> action)
        {
            Kind = kind;
            _action = action;

            foreach (var key in keys)
            {
                var regex = ParseRegexPattern(key);
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
            {
                return true;
            }

            foreach (var regex in _regexes)
            {
                if (regex.IsMatch(args.Key))
                {
                    return true;
                }
            }

            return false;
        }

        public Task ExecuteAsync(KeyboardEventArgs args)
            => _action(args);
    }

    private sealed class ConditionalCommand(IKeyCommand inner, Func<bool> condition) : IKeyCommand
    {
        public KeyEventKind Kind => inner.Kind;

        public bool IsHook => inner.IsHook;

        public bool CanExecute(KeyboardEventArgs args)
            => condition() && inner.CanExecute(args);

        public Task ExecuteAsync(KeyboardEventArgs args)
            => inner.ExecuteAsync(args);
    }

    private sealed class HookCommand(KeyEventKind kind, Func<KeyboardEventArgs, Task> hook) : IKeyCommand
    {
        public KeyEventKind Kind { get; } = kind;

        public bool IsHook => true;

        // Hook always executes for any key
        public bool CanExecute(KeyboardEventArgs args) => true;

        public Task ExecuteAsync(KeyboardEventArgs args) => hook(args);
    }
}
