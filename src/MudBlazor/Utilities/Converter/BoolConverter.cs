// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities.Converter.Dispatcher;
using static MudBlazor.Utilities.Converter.BoolConverter;
namespace MudBlazor.Utilities.Converter;

#nullable enable
public sealed class BoolConverter<T> : IReversibleConverter<T?, bool?>
{
    private readonly IReversibleConverter<T?, bool?> _dispatcher;

    public BoolConverter()
    {
        _dispatcher = ReversibleTypeDispatcher.Create<T?, bool?>()
            .Add(StringConverter.Instance)                  // string <-> bool?
            .Add<bool>(BoolIdentity.Instance)               // bool <-> bool?
            .Add<bool?>(BoolIdentity.Instance)              // bool? <-> bool?
            .Add(NumberConverter<int>.Instance)             // int <-> bool?
            .Add(NullableNumberConverter<int>.Instance)     // int? <-> bool?
            .Add(NumberConverter<uint>.Instance)            // uint <-> bool?
            .Add(NullableNumberConverter<uint>.Instance)    // uint? <-> bool?
            .Add(NumberConverter<sbyte>.Instance)           // sbyte  <-> bool?
            .Add(NullableNumberConverter<sbyte>.Instance)   // sbyte? <-> bool?
            .Add(NumberConverter<byte>.Instance)            // byte <-> bool?
            .Add(NullableNumberConverter<byte>.Instance)    // byte? <-> bool?
            .Add(NumberConverter<short>.Instance)           // short  <-> bool?
            .Add(NullableNumberConverter<short>.Instance)   // short? <-> bool?
            .Add(NumberConverter<ushort>.Instance)          // ushort <-> bool?
            .Add(NullableNumberConverter<ushort>.Instance)  // ushort? <-> bool?
            .Add(NumberConverter<long>.Instance)            // long <-> bool?
            .Add(NullableNumberConverter<long>.Instance)    // long? <-> bool?
            .Add(NumberConverter<ulong>.Instance)           // ulong <-> bool?
            .Add(NullableNumberConverter<ulong>.Instance)   // ulong? <-> bool?
            .Add(NumberConverter<float>.Instance)           // float <-> bool?
            .Add(NullableNumberConverter<float>.Instance)   // int? <-> bool?
            .Add(NumberConverter<double>.Instance)          // double <-> bool?
            .Add(NullableNumberConverter<double>.Instance)  // double? <-> bool?
            .Add(NumberConverter<decimal>.Instance)         // decimal <-> bool?
            .Add(NullableNumberConverter<decimal>.Instance) // decimal? <-> bool?
            .Add(NumberConverter<char>.Instance)            // char <-> bool?
            .Add(NullableNumberConverter<char>.Instance)    // char? <-> bool?
            .Add(ObjectBoolConverter.Instance)
            .Build();
    }

    public bool? Convert(T? input) => _dispatcher.Convert(input);

    public T? ConvertBack(bool? output) => _dispatcher.ConvertBack(output);

    public static readonly BoolConverter<T> Instance = new();
}
