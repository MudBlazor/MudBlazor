// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities.Converter.Base;
using MudBlazor.Utilities.Converter.Dispatcher;

namespace MudBlazor.Utilities.Converter;

#nullable enable
public sealed class BoolConverter<T> : IReversibleConverter<T?, bool?>
{
    private readonly IReversibleConverter<T?, bool?> _dispatcher;

    public BoolConverter()
    {
        _dispatcher = ReversibleTypeDispatcher.Create<T?, bool?>()
            .Add(BoolStringConverter.Instance)      // string     <-> bool?
            .Add<bool>(BoolIdentity.Instance)             // bool       <-> bool?
            .Add<bool?>(BoolIdentity.Instance)     // bool?      <-> bool?
            .Add<int>(BoolIntConverter.Instance)         // int        <-> bool?
            .Add<int?>(BoolIntConverter.Instance) // int?       <-> bool?
            .Add(ObjectBoolConverter.Instance)
            .Build();
    }

    public bool? Convert(T? input) => _dispatcher.Convert(input);

    public T? ConvertBack(bool? output) => _dispatcher.ConvertBack(output);

    public sealed class BoolStringConverter : IReversibleConverter<string?, bool?>
    {
        public bool? Convert(string? input)
        {
            if (input is null) return null;
            if (bool.TryParse(input, out var b)) return b;

            return input.ToLowerInvariant() switch
            {
                "on" => true,
                "off" => false,
                _ => null
            };
        }

        public string? ConvertBack(bool? value) =>
            value switch
            {
                true => "on",
                false => "off",
                _ => null
            };

        public static BoolStringConverter Instance { get; } = new();
    }

    // bool? <-> int
    // bool? <-> int?
    public sealed class BoolIntConverter : IReversibleConverter<int, bool?>, IReversibleConverter<int?, bool?>
    {
        public bool? Convert(int i) => i switch
        {
            0 => false,
            _ => true
        };

        public bool? Convert(int? i) => i switch
        {
            null => null,
            > 0 => true,
            _ => false
        };

        int IReversibleConverter<int, bool?>.ConvertBack(bool? b) => b switch
        {
            null => 0,
            false => 0,
            true => 1
        };

        public int? ConvertBack(bool? b) => b switch
        {
            true => 1,
            false => 0,
            _ => null
        };

        public static BoolIntConverter Instance { get; } = new();
    }

    // bool? <-> bool
    // bool? <-> bool?
    public sealed class BoolIdentity : IReversibleConverter<bool, bool?>, IReversibleConverter<bool?, bool?>
    {
        public bool? Convert(bool value) => value;

        public bool? Convert(bool? value) => value;

        bool IReversibleConverter<bool, bool?>.ConvertBack(bool? value) => value == true;

        public bool? ConvertBack(bool? value) => value;

        public static BoolIdentity Instance { get; } = new();
    }

    /// <summary>
    /// Converts from object? to bool? and back by delegating to specific typed reversible converters.
    /// </summary>
    public sealed class ObjectBoolConverter : IReversibleConverter<object?, bool?>
    {
        public bool? Convert(object? input)
        {
            return input switch
            {
                null => null,
                bool b => BoolIdentity.Instance.Convert(b),
                int i => BoolIntConverter.Instance.Convert(i),
                string s => BoolStringConverter.Instance.Convert(s),
                _ => throw new InvalidOperationException($"Cannot convert type {input.GetType()} to bool?")
            };
        }

        public object? ConvertBack(bool? value)
        {
            return BoolIdentity.Instance.ConvertBack(value);
        }

        public static ObjectBoolConverter Instance { get; } = new();
    }
}
