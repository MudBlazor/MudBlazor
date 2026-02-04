// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using MudBlazor.Utilities.Converter.Dispatcher;
using static MudBlazor.BoolConverter;

namespace MudBlazor;

public sealed class BoolConverter<T> : IReversibleConverter<T?, bool?>
{
    private static readonly SafeType[] _numericTypes =
    [
        typeof(int), typeof(uint),
        typeof(short), typeof(ushort),
        typeof(long), typeof(ulong),
        typeof(byte), typeof(sbyte),
        typeof(float), typeof(double), typeof(decimal), typeof(char)
    ];
    private readonly IReversibleConverter<T?, bool?> _dispatcher;

    private BoolConverter()
    {
        var builder = ReversibleTypeDispatcher.Create<T?, bool?>()
            .Add(StringConverter.Instance)      // string <-> bool?
            .Add<bool>(BoolIdentity.Instance)   // bool <-> bool?
            .Add<bool?>(BoolIdentity.Instance)  // bool? <-> bool?
            .Add(ObjectBoolConverter.Instance); // object <-> bool?

        // If Microsoft's adds this API https://github.com/dotnet/runtime/issues/28033
        // Then we can make for any T that implements INumber<T>, right now we will use know numeric types only for simplicity
        foreach (var type in _numericTypes)
        {
            // Very small overhead as this is only done once per T when the static Instance is accessed the first time
            var numberConv = Activator.CreateInstance(typeof(NumberConverter<>).MakeGenericType(type));
            var nullableConv = Activator.CreateInstance(typeof(NullableNumberConverter<>).MakeGenericType(type));

            builder.AddDynamic(type, numberConv); // T <-> bool? where T : INumber<T>
            builder.AddDynamic(typeof(Nullable<>).MakeGenericType(type), nullableConv); // T? <-> bool? where T : INumber<T>, struct
        }

        _dispatcher = builder.Build();
    }

    public bool? Convert(T? input) => _dispatcher.Convert(input);

    public T? ConvertBack(bool? output) => _dispatcher.ConvertBack(output);

    public static readonly BoolConverter<T> Instance = new();

    // Stupid, but we can't apply [DynamicallyAccessedMembers] for Type[] so we need a wrapper, otherwise MakeGenericType will complain.
    private readonly struct SafeType
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        private Type Type { get; }

        private SafeType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type) => Type = type;

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public static implicit operator Type(SafeType safeType) => safeType.Type;

        public static implicit operator SafeType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type) => new(type);
    }
}
