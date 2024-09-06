using System;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A set of methods which generate CSS classes for <see cref="MudBaseInput{T}" /> components.
/// </summary>
internal static class MudInputCssHelper
{
    /// <summary>
    /// Gets the CSS classes for the specified input component.
    /// </summary>
    /// <typeparam name="T">The type of data collect by the input.</typeparam>
    /// <param name="baseInput">The input control to use.</param>
    /// <param name="shrinkWhen">The function which determines when to shrink the input.</param>
    /// <returns>A set of CSS classes.</returns>
    public static string GetClassname<T>(MudBaseInput<T> baseInput, Func<bool> shrinkWhen) =>
        new CssBuilder("mud-input")
            .AddClass($"mud-input-{baseInput.Variant.ToDescriptionString()}")
            .AddClass($"mud-input-{baseInput.Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(baseInput.Label))
            .AddClass($"mud-input-adorned-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
            .AddClass($"mud-input-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
            .AddClass("mud-input-underline", when: () => baseInput.Underline && baseInput.Variant != Variant.Outlined)
            .AddClass("mud-shrink", when: shrinkWhen)
            .AddClass("mud-disabled", baseInput.Disabled)
            .AddClass("mud-input-error", baseInput.HasErrors)
            .AddClass("mud-ltr", baseInput.GetInputType() == InputType.Email || baseInput.GetInputType() == InputType.Telephone)
            .AddClass($"mud-typography-{baseInput.Typo.ToDescriptionString()}")
            .AddClass(baseInput.Class)
            .Build();

    /// <summary>
    /// Gets the CSS classes for the specified input component slot.
    /// </summary>
    /// <typeparam name="T">The type of data collect by the input.</typeparam>
    /// <param name="baseInput">The input control to use.</param>
    /// <returns>A set of CSS classes.</returns>
    public static string GetInputClassname<T>(MudBaseInput<T> baseInput) =>
        new CssBuilder("mud-input-slot")
            .AddClass("mud-input-root")
            .AddClass($"mud-input-root-{baseInput.Variant.ToDescriptionString()}")
            .AddClass($"mud-input-root-adorned-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
            .AddClass($"mud-input-root-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
            .AddClass(baseInput.Class)
            .Build();

    /// <summary>
    /// Gets the CSS classes for the specified input adornment.
    /// </summary>
    /// <typeparam name="T">The type of data collect by the input.</typeparam>
    /// <param name="baseInput">The input control to use.</param>
    /// <returns>A set of CSS classes.</returns>
    public static string GetAdornmentClassname<T>(MudBaseInput<T> baseInput) =>
        new CssBuilder("mud-input-adornment")
            .AddClass($"mud-input-adornment-{baseInput.Adornment.ToDescriptionString()}", baseInput.Adornment != Adornment.None)
            .AddClass($"mud-text", !string.IsNullOrEmpty(baseInput.AdornmentText))
            .AddClass($"mud-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
            .AddClass(baseInput.Class)
            .Build();
}
