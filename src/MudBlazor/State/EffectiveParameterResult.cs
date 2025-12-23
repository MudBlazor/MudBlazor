// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.State;

#nullable enable
public readonly record struct EffectiveParameterResult<T1, T2>
{
    public bool HasEffectiveParameter { get; }

    [MemberNotNullWhen(true, nameof(Parameter1Value))]
    public bool IsParameter1 { get; }

    [MemberNotNullWhen(true, nameof(Parameter2Value))]
    public bool IsParameter2 => HasEffectiveParameter && !IsParameter1;

    public T1? Parameter1Value { get; }

    public T2? Parameter2Value { get; }

    private EffectiveParameterResult(
        bool hasEffectiveParameter,
        bool isParameter1,
        T1? value1,
        T2? value2)
    {
        HasEffectiveParameter = hasEffectiveParameter;
        IsParameter1 = isParameter1;
        Parameter1Value = value1;
        Parameter2Value = value2;
    }

    internal static EffectiveParameterResult<T1, T2> None()
        => new(false, false, default, default);

    internal static EffectiveParameterResult<T1, T2> FromParameter1(
        string name,
        T1? value)
        => new(true, true, value, default);

    internal static EffectiveParameterResult<T1, T2> FromParameter2(
        string name,
        T2? value)
        => new(true, false, default, value);
}
