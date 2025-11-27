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
            .Add(BoolIdentity.Instance)             // bool       <-> bool?
            .Add(BoolNullableIdentity.Instance)     // bool?      <-> bool?
            .Add(BoolStringConverter.Instance)      // string     <-> bool?
            .Add(BoolIntConverter.Instance)         // int        <-> bool?
            .Add(BoolNullableIntConverter.Instance) // int?       <-> bool?
            .Add(ObjectBoolConverter.Instance)
            .Build();
        //_dispatcher = ReversibleTypeDispatcher<T?, bool?>
        //    .CreateReversible()
        //    .Add(new BoolIdentity())             // bool       <-> bool?
        //    .Add(new BoolNullableIdentity())     // bool?      <-> bool?
        //    .Add(new BoolStringConverter())      // string     <-> bool?
        //    .Add(new BoolIntConverter())         // int        <-> bool?
        //    .Add(new BoolNullableIntConverter()) // int?       <-> bool?
        //    .Add(new ObjectBoolConverter())
        //    .Build();
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
    public sealed class BoolIntConverter : IReversibleConverter<int, bool?>
    {
        public bool? Convert(int i) => i switch
        {
            0 => false,
            _ => true
        };

        public int ConvertBack(bool? b) => b switch
        {
            null => 0,
            false => 0,
            true => 1
        };

        public static BoolIntConverter Instance { get; } = new();
    }

    // bool? <-> int?
    public sealed class BoolNullableIntConverter : IReversibleConverter<int?, bool?>
    {
        public bool? Convert(int? i) => i switch
        {
            null => null,
            > 0 => true,
            _ => false
        };

        public int? ConvertBack(bool? b) => b switch
        {
            true => 1,
            false => 0,
            _ => null
        };

        public static BoolNullableIntConverter Instance { get; } = new();
    }

    // bool? <-> bool? (identity)
    public sealed class BoolNullableIdentity : IReversibleConverter<bool?, bool?>
    {
        public bool? Convert(bool? b) => b;

        public bool? ConvertBack(bool? b) => b;

        public static BoolNullableIdentity Instance { get; } = new();
    }

    // bool? <-> bool (non-nullable)
    public sealed class BoolIdentity : IReversibleConverter<bool, bool?>
    {
        public bool? Convert(bool b) => b;

        public bool ConvertBack(bool? b) => b == true;

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
            // For ConvertBack we default to returning bool? (could extend to choose original runtime type)
            return new BoolNullableIdentity().ConvertBack(value);
        }

        public static ObjectBoolConverter Instance { get; } = new();
    }
}
