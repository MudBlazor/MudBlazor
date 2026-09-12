// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

/// <summary>
/// The most broadly valid CSS size in the <see cref="Units"/> hierarchy.
/// </summary>
/// <remarks>
/// <para>
/// Excludes fr, auto, min-content, max-content (see <see cref="IInflexibleBreadth"/>).
/// </para>
/// </remarks>
public abstract class LengthPercentage : CssStringBuilder, IInflexibleBreadth, IFixedSize, ICssUnit
{
    /// <summary>
    /// Adds two units together using CSS <c>calc()</c>.
    /// </summary>
    public static LengthPercentage operator +(LengthPercentage a, LengthPercentage b) => new CalcSum(a, SumOperator.Add, b);

    /// <summary>
    /// Subtracts two units using CSS <c>calc()</c>.
    /// </summary>
    public static LengthPercentage operator -(LengthPercentage a, LengthPercentage b) => new CalcSum(a, SumOperator.Subtract, b);

    /// <summary>
    /// Multiplies unit using CSS <c>calc()</c>.
    /// </summary>
    public static LengthPercentage operator *(LengthPercentage a, double b) => new CalcProduct(a, ProductOperator.Multiply, b);

    /// <summary>
    /// Divides unit using CSS <c>calc()</c>.
    /// </summary>
    public static LengthPercentage operator /(LengthPercentage a, double b) => new CalcProduct(a, ProductOperator.Divide, b);
}
