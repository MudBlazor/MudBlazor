// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor.Utilities;

[EnumExtensions]
internal enum SumOperator
{
    [Description("+")]
    Add,
    [Description("-")]
    Subtract,
}
internal sealed class CalcSum : LengthPercentage
{
    public CalcSum(LengthPercentage a, SumOperator op, LengthPercentage b) => Value = $"calc({a} {op.ToStringFast(true)} {b})";
}
