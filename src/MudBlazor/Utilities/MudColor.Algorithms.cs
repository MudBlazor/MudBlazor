// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

public partial class MudColor
{
    /// <summary>
    /// Linearly interpolates between two <see cref="MudColor"/> instances.
    /// </summary>
    /// <param name="colorStart">The starting <see cref="MudColor"/> instance.</param>
    /// <param name="colorEnd">The ending <see cref="MudColor"/> instance.</param>
    /// <param name="t">The interpolation factor (0.0 to 1.0).</param>
    /// <returns>A new <see cref="MudColor"/> instance that is the result of the interpolation.</returns>
    public static MudColor Lerp(MudColor colorStart, MudColor colorEnd, float t)
    {
        t = Math.Clamp(t, 0.0f, 1.0f);
        var r = InterpolateValue(colorStart.R, colorEnd.R);
        var g = InterpolateValue(colorStart.G, colorEnd.G);
        var b = InterpolateValue(colorStart.B, colorEnd.B);
        var a = InterpolateValue(colorStart.A, colorEnd.A);
        var aPercentage = NormalizeAlpha(a, 3);
        // Using alpha as a percentage ensures more accurate alpha blending. 
        // Creating a MudColor from an alpha byte or integer can result in fractional alpha values (e.g., 0.996078431372549), 
        // which makes it difficult to compare two colors accurately in real-world scenarios.
        return new MudColor(r, g, b, alpha: aPercentage);

        int InterpolateValue(byte start, byte end) => (int)(start * (1.0f - t) + end * t);
    }

    /// <summary>
    /// Generates a gradient palette of colors between two specified colors.
    /// </summary>
    /// <param name="startColor">The starting color of the gradient.</param>
    /// <param name="endColor">The ending color of the gradient.</param>
    /// <param name="numberOfColors">The total number of colors in the gradient palette.</param>
    /// <returns>An enumerable collection of <see cref="MudColor"/> representing the gradient palette.</returns>
    public static IEnumerable<MudColor> GenerateGradientPalette(MudColor startColor, MudColor endColor, int numberOfColors = 5)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfColors);
        for (var i = 0; i < numberOfColors; i++)
        {
            var t = i / (float)(numberOfColors - 1);
            yield return Lerp(startColor, endColor, t);
        }
    }

    /// <summary>
    /// Generates an analogous palette of colors based on a specified base color.
    /// </summary>
    /// <param name="baseColor">The base color to generate the analogous palette from.</param>
    /// <param name="numberOfColors">The total number of colors in the analogous palette.</param>
    /// <param name="angle">The angle between each color in the analogous palette.</param>
    /// <returns>An enumerable collection of <see cref="MudColor"/> representing the analogous palette.</returns>
    public static IEnumerable<MudColor> GenerateAnalogousPalette(MudColor baseColor, int numberOfColors = 5, double angle = 30)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfColors);
        yield return baseColor;
        for (var i = 1; i < numberOfColors; i++)
        {
            yield return baseColor.SetH((baseColor.H + i * angle) % 360);
        }
    }

    /// <summary>
    /// Generates a palette of colors by lightening and darkening the base color.
    /// </summary>
    /// <param name="baseColor">The base color to generate the palette from.</param>
    /// <param name="numberOfColors">The total number of colors in the palette.</param>
    /// <param name="tintStep">The step value for lightening the color. If <paramref name="tintStep"/> is <c>0</c>, no lighter colors will be added to the palette.</param>
    /// <param name="shadeStep">The step value for darkening the color. If <paramref name="shadeStep"/> is <c>0</c>, no darker colors will be added to the palette.</param>
    /// <returns>A read-only list of <see cref="MudColor"/> representing the generated palette.</returns>
    public static IEnumerable<MudColor> GenerateTintShadePalette(MudColor baseColor, int numberOfColors = 5, double tintStep = 0.075, double shadeStep = 0.075)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfColors);
        ArgumentOutOfRangeException.ThrowIfNegative(tintStep);
        ArgumentOutOfRangeException.ThrowIfNegative(shadeStep);

        if (tintStep > 0 && shadeStep > 0)
        {
            var halfColors = (numberOfColors - 1) / 2;
            var lighterColors = halfColors + (numberOfColors % 2 == 0 ? 1 : 0);
            var darkerColors = halfColors;

            // lighter colors
            for (var i = lighterColors; i > 0; i--)
            {
                yield return baseColor.ColorLighten(i * tintStep);
            }

            // Yield the base color
            yield return baseColor;

            // darker colors
            for (var i = 1; i <= darkerColors; i++)
            {
                yield return baseColor.ColorDarken(i * shadeStep);
            }
        }
        else if (tintStep > 0) // Only lighter colors
        {
            yield return baseColor;
            for (var i = 1; i < numberOfColors; i++)
            {
                yield return baseColor.ColorLighten(i * tintStep);
            }
        }
        else if (shadeStep > 0) // Only darker colors
        {
            yield return baseColor;
            for (var i = 1; i < numberOfColors; i++)
            {
                yield return baseColor.ColorDarken(i * shadeStep);
            }
        }
    }
}
