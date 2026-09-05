// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor.Utilities;

[EnumExtensions]
internal enum ProductOperator
{

    [Description("*")]
    Multiply,
    [Description("/")]
    Divide
}
internal sealed class CalcProduct : LengthPercentage
{
    public CalcProduct(LengthPercentage a, ProductOperator op, double b) => Value = $"calc({a} {op.ToStringFast(true)} {b})";
}
