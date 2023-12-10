using System;
using MudBlazor.Utilities;

namespace MudBlazor
{
    internal static class MudInputCssHelper
    {
        public static string GetClassname<T>(MudBaseInput<T> baseInput, Func<bool> shrinkWhen) =>
            new CssBuilder("mud-input")
                .AddClass($"mud-input-{baseInput.Variant.ToDescriptionString()}")
                .AddClass("mud-input-adorned-start", baseInput.HasStartAdornment)
                .AddClass("mud-input-adorned-end", baseInput.HasEndAdornment)
                .AddClass($"mud-input-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass("mud-input-underline", when: () => baseInput.DisableUnderLine == false && baseInput.Variant != Variant.Outlined)
                .AddClass("mud-shrink", when: shrinkWhen)
                .AddClass("mud-disabled", baseInput.Disabled)
                .AddClass("mud-input-error", baseInput.HasErrors)
                .AddClass("mud-ltr", baseInput.GetInputType() == InputType.Email || baseInput.GetInputType() == InputType.Telephone)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetInputClassname<T>(MudBaseInput<T> baseInput) =>
            new CssBuilder("mud-input-slot")
                .AddClass("mud-input-root")
                .AddClass($"mud-input-root-{baseInput.Variant.ToDescriptionString()}")
                .AddClass("mud-input-root-adorned-start", baseInput.HasStartAdornment)
                .AddClass("mud-input-root-adorned-end", baseInput.HasEndAdornment)
                .AddClass($"mud-input-root-margin-{baseInput.Margin.ToDescriptionString()}", when: () => baseInput.Margin != Margin.None)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetAdornmentStartClassname<T>(MudBaseInput<T> baseInput) =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-start")
                .AddClass("mud-text", !string.IsNullOrEmpty(baseInput.AdornmentStartText) && !string.IsNullOrEmpty(baseInput.AdornmentEndText))
                .AddClass("mud-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
                .AddClass(baseInput.Class)
                .Build();

        public static string GetAdornmentEndClassname<T>(MudBaseInput<T> baseInput) =>
            new CssBuilder("mud-input-adornment")
                .AddClass("mud-input-adornment-end")
                .AddClass("mud-text", !string.IsNullOrEmpty(baseInput.AdornmentStartText) && !string.IsNullOrEmpty(baseInput.AdornmentEndText))
                .AddClass("mud-input-root-filled-shrink", baseInput.Variant == Variant.Filled)
                .AddClass(baseInput.Class)
                .Build();
    }
}
