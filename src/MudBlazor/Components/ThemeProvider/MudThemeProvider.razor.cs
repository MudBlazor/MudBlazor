using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Provides a standard set of colors, shapes, sizes and shadows to a layout.
/// </summary>
/// <seealso cref="MudTheme"/>
partial class MudThemeProvider : ComponentBaseWithState, IAsyncDisposable
{
    // private const string Breakpoint = "mud-breakpoint";
    private bool _disposed;
    private bool _observing;
    private const string Palette = "mud-palette";
    private const string Ripple = "mud-ripple";
    private const string Elevation = "mud-elevation";
    private const string Typography = "mud-typography";
    private const string LayoutProperties = "mud";
    private const string Zindex = "mud-zindex";

    private readonly MudTheme _originalMudTheme = new();
    private readonly ParameterState<bool> _isDarkModeState;
    private readonly ParameterState<Palette?> _currentPaletteState;
    private readonly ParameterState<bool> _observeSystemDarkModeChangeState;
    private readonly Lazy<DotNetObjectReference<MudThemeProvider>> _lazyDotNetRef;

    private event Func<bool, Task>? DarkModeChanged;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The theme used by the application.
    /// </summary>
    [Parameter]
    public MudTheme? Theme { get; set; }

    /// <summary>
    /// Uses the browser default scrollbar instead of the MudBlazor scrollbar. 
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool DefaultScrollbar { get; set; }

    /// <summary>
    /// Detects when the system theme has changed between Light Mode and Dark Mode.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// When <c>true</c>, the theme will automatically change to Light Mode or Dark Mode as the system theme changes.
    /// </remarks>
    [Parameter, ParameterState]
    public bool ObserveSystemDarkModeChange { get; set; } = true;

    /// <summary>
    /// Uses darker colors for all MudBlazor components.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// When this value changes, <see cref="IsDarkModeChanged"/> occurs.
    /// </remarks>
    [Parameter, ParameterState]
    public bool IsDarkMode { get; set; }

    /// <summary>
    /// Occurs when <see cref="IsDarkMode"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsDarkModeChanged { get; set; }

    /// <summary>
    /// Gets the currently active palette based on the <see cref="IsDarkMode"/> setting.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="MudTheme.PaletteDark"/> when <see cref="IsDarkMode"/> is <c>true</c>; otherwise, returns <see cref="MudTheme.PaletteLight"/>.
    /// When this value changes, <see cref="CurrentPaletteChanged"/> occurs.
    /// </remarks>
    [Parameter, ParameterState]
    public Palette? CurrentPalette { get; set; }

    /// <summary>
    /// Occurs when <see cref="CurrentPalette"/> has changed.
    /// </summary>
    [Parameter]
    public EventCallback<Palette?> CurrentPaletteChanged { get; set; }

    [DynamicDependency(nameof(SystemDarkModeChangedAsync))]
    public MudThemeProvider()
    {
        using var registerScope = CreateRegisterScope();
        _isDarkModeState = registerScope.RegisterParameter<bool>(nameof(IsDarkMode))
            .WithParameter(() => IsDarkMode)
            .WithEventCallback(() => IsDarkModeChanged);
        _observeSystemDarkModeChangeState = registerScope.RegisterParameter<bool>(nameof(ObserveSystemDarkModeChange))
            .WithParameter(() => ObserveSystemDarkModeChange)
            .WithChangeHandler(OnObserveSystemDarkModeChangeChanged);
        _currentPaletteState = registerScope.RegisterParameter<Palette?>(nameof(CurrentPalette))
            .WithParameter(() => CurrentPalette)
            .WithEventCallback(() => CurrentPaletteChanged);
        _lazyDotNetRef = new Lazy<DotNetObjectReference<MudThemeProvider>>(CreateDotNetObjectReference);
    }

    /// <summary>
    /// Gets the browser's color preference.
    /// </summary>
    /// <returns>
    /// Returns <c>true</c> if the theme is Dark Mode; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> GetSystemDarkModeAsync()
    {
        var (_, value) = await JsRuntime.InvokeAsyncWithErrorHandling(false, "mudThemeProvider.isDarkMode");
        return value;
    }

    /// <summary>
    /// Calls a function when the system's color has changed.
    /// </summary>
    /// <param name="onChange">The function to call when the system theme has changed.</param>
    /// <remarks>
    /// A value of <c>true</c> is passed if the system is now in Dark Mode. Otherwise, the system is now in Light Mode.
    /// </remarks>
    public Task WatchSystemDarkModeAsync(Func<bool, Task> onChange)
    {
        DarkModeChanged += onChange;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Occurs when the system's dark mode has changed.
    /// </summary>
    /// <param name="isDarkMode">When <c>true</c>, the system is in Dark Mode; <c>false</c> is Light Mode.</param>
    [JSInvokable]
    public async Task SystemDarkModeChangedAsync(bool isDarkMode)
    {
        await _isDarkModeState.SetValueAsync(isDarkMode);
        var handler = DarkModeChanged;
        if (handler is not null)
        {
            await handler(isDarkMode);
        }
    }

    // <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_observeSystemDarkModeChangeState.Value && !_observing)
            {
                _observing = true;
                await WatchDarkMode();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    // <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await _currentPaletteState.SetValueAsync(GetCurrentPalette());

        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// Gets the CSS styles for this provider.
    /// </summary>
    /// <returns>A <c>style</c> HTML element containing this theme's styles.</returns>
    protected string BuildTheme()
    {
        var theme = GetTheme();
        var themeStringBuilder = new StringBuilder();
        themeStringBuilder.AppendLine("<style class='mud-theme-provider'>");
        themeStringBuilder.Append(theme.PseudoCss.Scope);
        themeStringBuilder.AppendLine("{");
        GenerateTheme(themeStringBuilder);
        themeStringBuilder.AppendLine("}");
        themeStringBuilder.AppendLine("</style>");

        return themeStringBuilder.ToString();
    }

    /// <summary>
    /// Gets the CSS styles for the browser scrollbar.
    /// </summary>
    /// <returns>A <c>style</c> HTML element containing the scrollbar's styles.</returns>
    protected static string BuildMudBlazorScrollbar()
    {
        var scrollbar = new StringBuilder();

        scrollbar.AppendLine("<style>");
        scrollbar.AppendLine("::-webkit-scrollbar {width: 8px;height: 8px;z-index: 1;}");
        scrollbar.AppendLine("::-webkit-scrollbar-track {background: transparent;}");
        scrollbar.AppendLine("::-webkit-scrollbar-thumb {background: #c4c4c4;border-radius: 1px;}");
        scrollbar.AppendLine("::-webkit-scrollbar-thumb:hover {background: #a6a6a6;}");

        //Firefox
        scrollbar.AppendLine("html, body * {scrollbar-color: #c4c4c4 transparent;scrollbar-width: thin;}");
        scrollbar.AppendLine("</style>");

        return scrollbar.ToString();
    }

    /// <summary>
    /// Generates the CSS styles for the specified theme.
    /// </summary>
    /// <param name="themeStringBuilder">The theme to append to.</param>
    /// <remarks>
    /// Several CSS values for color, opacity, and elevation are appended based on the value of <see cref="IsDarkMode"/>.
    /// </remarks>
    protected virtual void GenerateTheme(StringBuilder themeStringBuilder)
    {
        var theme = GetTheme();

        var palette = _isDarkModeState.Value ? theme.PaletteDark : theme.PaletteLight;

        //Palette
        themeStringBuilder.AppendLine($"--{Palette}-black: {palette.Black};");
        themeStringBuilder.AppendLine($"--{Palette}-white: {palette.White};");

        themeStringBuilder.AppendLine($"--{Palette}-primary: {palette.Primary};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-primary-rgb: {palette.Primary.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-primary-text: {palette.PrimaryContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-primary-darken: {palette.PrimaryDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-primary-lighten: {palette.PrimaryLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-primary-hover: {palette.Primary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-secondary: {palette.Secondary};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-secondary-rgb: {palette.Secondary.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-secondary-text: {palette.SecondaryContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-secondary-darken: {palette.SecondaryDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-secondary-lighten: {palette.SecondaryLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-secondary-hover: {palette.Secondary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-tertiary: {palette.Tertiary};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-tertiary-rgb: {palette.Tertiary.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-tertiary-text: {palette.TertiaryContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-tertiary-darken: {palette.TertiaryDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-tertiary-lighten: {palette.TertiaryLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-tertiary-hover: {palette.Tertiary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-info: {palette.Info};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-info-rgb: {palette.Info.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-info-text: {palette.InfoContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-info-darken: {palette.InfoDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-info-lighten: {palette.InfoLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-info-hover: {palette.Info.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-success: {palette.Success};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-success-rgb: {palette.Success.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-success-text: {palette.SuccessContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-success-darken: {palette.SuccessDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-success-lighten: {palette.SuccessLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-success-hover: {palette.Success.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-warning: {palette.Warning};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-warning-rgb: {palette.Warning.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-warning-text: {palette.WarningContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-warning-darken: {palette.WarningDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-warning-lighten: {palette.WarningLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-warning-hover: {palette.Warning.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-error: {palette.Error};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-error-rgb: {palette.Error.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-error-text: {palette.ErrorContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-error-darken: {palette.ErrorDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-error-lighten: {palette.ErrorLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-error-hover: {palette.Error.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-dark: {palette.Dark};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-dark-rgb: {palette.Dark.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-dark-text: {palette.DarkContrastText};");
        themeStringBuilder.AppendLine($"--{Palette}-dark-darken: {palette.DarkDarken};");
        themeStringBuilder.AppendLine($"--{Palette}-dark-lighten: {palette.DarkLighten};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-dark-hover: {palette.Dark.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");

        themeStringBuilder.AppendLine($"--{Palette}-text-primary: {palette.TextPrimary};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-text-primary-rgb: {palette.TextPrimary.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-text-secondary: {palette.TextSecondary};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-text-secondary-rgb: {palette.TextSecondary.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-text-disabled: {palette.TextDisabled};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-text-disabled-rgb: {palette.TextDisabled.ToString(MudColorOutputFormats.ColorElements)};");

        themeStringBuilder.AppendLine($"--{Palette}-action-default: {palette.ActionDefault};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-action-default-hover: {palette.ActionDefault.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        themeStringBuilder.AppendLine($"--{Palette}-action-disabled: {palette.ActionDisabled};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-action-disabled-background: {palette.ActionDisabledBackground};");

        themeStringBuilder.AppendLine($"--{Palette}-surface: {palette.Surface};");
        themeStringBuilder.AppendLine($"--{Palette}-surface-rgb: {palette.Surface.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-background: {palette.Background};");
        themeStringBuilder.AppendLine($"--{Palette}-background-gray: {palette.BackgroundGray};");
        themeStringBuilder.AppendLine($"--{Palette}-drawer-background: {palette.DrawerBackground};");
        themeStringBuilder.AppendLine($"--{Palette}-drawer-text: {palette.DrawerText};");
        themeStringBuilder.AppendLine($"--{Palette}-drawer-icon: {palette.DrawerIcon};");
        themeStringBuilder.AppendLine($"--{Palette}-appbar-background: {palette.AppbarBackground};");
        themeStringBuilder.AppendLine($"--{Palette}-appbar-text: {palette.AppbarText};");

        themeStringBuilder.AppendLine($"--{Palette}-lines-default: {palette.LinesDefault};");
        themeStringBuilder.AppendLine($"--{Palette}-lines-inputs: {palette.LinesInputs};");

        themeStringBuilder.AppendLine($"--{Palette}-table-lines: {palette.TableLines};");
        themeStringBuilder.AppendLine($"--{Palette}-table-striped: {palette.TableStriped};");
        themeStringBuilder.AppendLine($"--{Palette}-table-hover: {palette.TableHover};");

        themeStringBuilder.AppendLine($"--{Palette}-divider: {palette.Divider};");
        themeStringBuilder.AppendLine(
            $"--{Palette}-divider-rgb: {palette.Divider.ToString(MudColorOutputFormats.ColorElements)};");
        themeStringBuilder.AppendLine($"--{Palette}-divider-light: {palette.DividerLight};");

        themeStringBuilder.AppendLine($"--{Palette}-skeleton: {palette.Skeleton};");

        themeStringBuilder.AppendLine($"--{Palette}-gray-default: {palette.GrayDefault};");
        themeStringBuilder.AppendLine($"--{Palette}-gray-light: {palette.GrayLight};");
        themeStringBuilder.AppendLine($"--{Palette}-gray-lighter: {palette.GrayLighter};");
        themeStringBuilder.AppendLine($"--{Palette}-gray-dark: {palette.GrayDark};");
        themeStringBuilder.AppendLine($"--{Palette}-gray-darker: {palette.GrayDarker};");

        themeStringBuilder.AppendLine($"--{Palette}-overlay-dark: {palette.OverlayDark};");
        themeStringBuilder.AppendLine($"--{Palette}-overlay-light: {palette.OverlayLight};");

        themeStringBuilder.AppendLine($"--{Palette}-border-opacity: {palette.BorderOpacity.ToString(CultureInfo.InvariantCulture)};");

        //Ripple
        themeStringBuilder.AppendLine($"--{Ripple}-color: var(--{Palette}-text-primary);");
        themeStringBuilder.AppendLine($"--{Ripple}-opacity: {theme.PaletteLight.RippleOpacity.ToString(CultureInfo.InvariantCulture)};");
        themeStringBuilder.AppendLine($"--{Ripple}-opacity-secondary: {theme.PaletteLight.RippleOpacitySecondary.ToString(CultureInfo.InvariantCulture)};");

        //Elevations
        themeStringBuilder.AppendLine($"--{Elevation}-0: {theme.Shadows.Elevation.GetValue(0)};");
        themeStringBuilder.AppendLine($"--{Elevation}-1: {theme.Shadows.Elevation.GetValue(1)};");
        themeStringBuilder.AppendLine($"--{Elevation}-2: {theme.Shadows.Elevation.GetValue(2)};");
        themeStringBuilder.AppendLine($"--{Elevation}-3: {theme.Shadows.Elevation.GetValue(3)};");
        themeStringBuilder.AppendLine($"--{Elevation}-4: {theme.Shadows.Elevation.GetValue(4)};");
        themeStringBuilder.AppendLine($"--{Elevation}-5: {theme.Shadows.Elevation.GetValue(5)};");
        themeStringBuilder.AppendLine($"--{Elevation}-6: {theme.Shadows.Elevation.GetValue(6)};");
        themeStringBuilder.AppendLine($"--{Elevation}-7: {theme.Shadows.Elevation.GetValue(7)};");
        themeStringBuilder.AppendLine($"--{Elevation}-8: {theme.Shadows.Elevation.GetValue(8)};");
        themeStringBuilder.AppendLine($"--{Elevation}-9: {theme.Shadows.Elevation.GetValue(9)};");
        themeStringBuilder.AppendLine($"--{Elevation}-10: {theme.Shadows.Elevation.GetValue(10)};");
        themeStringBuilder.AppendLine($"--{Elevation}-11: {theme.Shadows.Elevation.GetValue(11)};");
        themeStringBuilder.AppendLine($"--{Elevation}-12: {theme.Shadows.Elevation.GetValue(12)};");
        themeStringBuilder.AppendLine($"--{Elevation}-13: {theme.Shadows.Elevation.GetValue(13)};");
        themeStringBuilder.AppendLine($"--{Elevation}-14: {theme.Shadows.Elevation.GetValue(14)};");
        themeStringBuilder.AppendLine($"--{Elevation}-15: {theme.Shadows.Elevation.GetValue(15)};");
        themeStringBuilder.AppendLine($"--{Elevation}-16: {theme.Shadows.Elevation.GetValue(16)};");
        themeStringBuilder.AppendLine($"--{Elevation}-17: {theme.Shadows.Elevation.GetValue(17)};");
        themeStringBuilder.AppendLine($"--{Elevation}-18: {theme.Shadows.Elevation.GetValue(18)};");
        themeStringBuilder.AppendLine($"--{Elevation}-19: {theme.Shadows.Elevation.GetValue(19)};");
        themeStringBuilder.AppendLine($"--{Elevation}-20: {theme.Shadows.Elevation.GetValue(20)};");
        themeStringBuilder.AppendLine($"--{Elevation}-21: {theme.Shadows.Elevation.GetValue(21)};");
        themeStringBuilder.AppendLine($"--{Elevation}-22: {theme.Shadows.Elevation.GetValue(22)};");
        themeStringBuilder.AppendLine($"--{Elevation}-23: {theme.Shadows.Elevation.GetValue(23)};");
        themeStringBuilder.AppendLine($"--{Elevation}-24: {theme.Shadows.Elevation.GetValue(24)};");
        themeStringBuilder.AppendLine($"--{Elevation}-25: {theme.Shadows.Elevation.GetValue(25)};");

        //Layout Properties
        themeStringBuilder.AppendLine(
            $"--{LayoutProperties}-default-borderradius: {theme.LayoutProperties.DefaultBorderRadius};");
        themeStringBuilder.AppendLine($"--{LayoutProperties}-drawer-width-left: {theme.LayoutProperties.DrawerWidthLeft};");
        themeStringBuilder.AppendLine($"--{LayoutProperties}-drawer-width-right: {theme.LayoutProperties.DrawerWidthRight};");
        themeStringBuilder.AppendLine(
            $"--{LayoutProperties}-drawer-width-mini-left: {theme.LayoutProperties.DrawerMiniWidthLeft};");
        themeStringBuilder.AppendLine(
            $"--{LayoutProperties}-drawer-width-mini-right: {theme.LayoutProperties.DrawerMiniWidthRight};");
        themeStringBuilder.AppendLine($"--{LayoutProperties}-appbar-height: {theme.LayoutProperties.AppbarHeight};");

        //Breakpoint
        //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
        //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
        //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
        //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
        //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");
        //theme.AppendLine($"--{Breakpoint}-xxl: {Theme.Breakpoints.xxl};");

        //Typography
        themeStringBuilder.AppendLine(
            $"--{Typography}-default-family: {FormatFontFamily(theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-default-size: {theme.Typography.Default.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-default-weight: {theme.Typography.Default.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-default-lineheight: {theme.Typography.Default.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-default-letterspacing: {theme.Typography.Default.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-default-text-transform: {theme.Typography.Default.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h1-family: {FormatFontFamily(theme.Typography.H1.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h1-size: {theme.Typography.H1.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h1-weight: {theme.Typography.H1.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h1-lineheight: {theme.Typography.H1.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h1-letterspacing: {theme.Typography.H1.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h1-text-transform: {theme.Typography.H1.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h2-family: {FormatFontFamily(theme.Typography.H2.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h2-size: {theme.Typography.H2.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h2-weight: {theme.Typography.H2.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h2-lineheight: {theme.Typography.H2.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h2-letterspacing: {theme.Typography.H2.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h2-text-transform: {theme.Typography.H2.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h3-family: {FormatFontFamily(theme.Typography.H3.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h3-size: {theme.Typography.H3.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h3-weight: {theme.Typography.H3.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h3-lineheight: {theme.Typography.H3.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h3-letterspacing: {theme.Typography.H3.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h3-text-transform: {theme.Typography.H3.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h4-family: {FormatFontFamily(theme.Typography.H4.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h4-size: {theme.Typography.H4.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h4-weight: {theme.Typography.H4.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h4-lineheight: {theme.Typography.H4.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h4-letterspacing: {theme.Typography.H4.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h4-text-transform: {theme.Typography.H4.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h5-family: {FormatFontFamily(theme.Typography.H5.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h5-size: {theme.Typography.H5.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h5-weight: {theme.Typography.H5.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h5-lineheight: {theme.Typography.H5.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h5-letterspacing: {theme.Typography.H5.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h5-text-transform: {theme.Typography.H5.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-h6-family: {FormatFontFamily(theme.Typography.H6.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-h6-size: {theme.Typography.H6.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-h6-weight: {theme.Typography.H6.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-h6-lineheight: {theme.Typography.H6.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-h6-letterspacing: {theme.Typography.H6.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-h6-text-transform: {theme.Typography.H6.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-subtitle1-family: {FormatFontFamily(theme.Typography.Subtitle1.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle1-size: {theme.Typography.Subtitle1.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle1-weight: {theme.Typography.Subtitle1.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-subtitle1-lineheight: {theme.Typography.Subtitle1.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle1-letterspacing: {theme.Typography.Subtitle1.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle1-text-transform: {theme.Typography.Subtitle1.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-subtitle2-family: {FormatFontFamily(theme.Typography.Subtitle2.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle2-size: {theme.Typography.Subtitle2.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle2-weight: {theme.Typography.Subtitle2.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-subtitle2-lineheight: {theme.Typography.Subtitle2.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle2-letterspacing: {theme.Typography.Subtitle2.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-subtitle2-text-transform: {theme.Typography.Subtitle2.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-body1-family: {FormatFontFamily(theme.Typography.Body1.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-body1-size: {theme.Typography.Body1.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-body1-weight: {theme.Typography.Body1.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-body1-lineheight: {theme.Typography.Body1.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-body1-letterspacing: {theme.Typography.Body1.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-body1-text-transform: {theme.Typography.Body1.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-body2-family: {FormatFontFamily(theme.Typography.Body2.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-body2-size: {theme.Typography.Body2.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-body2-weight: {theme.Typography.Body2.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-body2-lineheight: {theme.Typography.Body2.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-body2-letterspacing: {theme.Typography.Body2.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-body2-text-transform: {theme.Typography.Body2.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-button-family: {FormatFontFamily(theme.Typography.Button.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-button-size: {theme.Typography.Button.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-button-weight: {theme.Typography.Button.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-button-lineheight: {theme.Typography.Button.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-button-letterspacing: {theme.Typography.Button.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-button-text-transform: {theme.Typography.Button.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-caption-family: {FormatFontFamily(theme.Typography.Caption.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-caption-size: {theme.Typography.Caption.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-caption-weight: {theme.Typography.Caption.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-caption-lineheight: {theme.Typography.Caption.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-caption-letterspacing: {theme.Typography.Caption.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-caption-text-transform: {theme.Typography.Caption.TextTransform};");

        themeStringBuilder.AppendLine(
            $"--{Typography}-overline-family: {FormatFontFamily(theme.Typography.Overline.FontFamily ?? theme.Typography.Default.FontFamily ?? Array.Empty<string>())};");
        themeStringBuilder.AppendLine($"--{Typography}-overline-size: {theme.Typography.Overline.FontSize};");
        themeStringBuilder.AppendLine($"--{Typography}-overline-weight: {theme.Typography.Overline.FontWeight};");
        themeStringBuilder.AppendLine(
            $"--{Typography}-overline-lineheight: {theme.Typography.Overline.LineHeight};");
        themeStringBuilder.AppendLine($"--{Typography}-overline-letterspacing: {theme.Typography.Overline.LetterSpacing};");
        themeStringBuilder.AppendLine($"--{Typography}-overline-text-transform: {theme.Typography.Overline.TextTransform};");

        //Z-Index
        themeStringBuilder.AppendLine($"--{Zindex}-drawer: {theme.ZIndex.Drawer};");
        themeStringBuilder.AppendLine($"--{Zindex}-appbar: {theme.ZIndex.AppBar};");
        themeStringBuilder.AppendLine($"--{Zindex}-dialog: {theme.ZIndex.Dialog};");
        themeStringBuilder.AppendLine($"--{Zindex}-popover: {theme.ZIndex.Popover};");
        themeStringBuilder.AppendLine($"--{Zindex}-snackbar: {theme.ZIndex.Snackbar};");
        themeStringBuilder.AppendLine($"--{Zindex}-tooltip: {theme.ZIndex.Tooltip};");

        // Native HTML control light/dark mode
        themeStringBuilder.AppendLine($"--mud-native-html-color-scheme: {(_isDarkModeState.Value ? "dark" : "light")};");
    }

    protected MudTheme GetTheme() => Theme ?? _originalMudTheme;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases managed resources associated with this object asynchronously.
    /// </summary>
    /// <returns>The task representing asynchronous execution of this method.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        _disposed = true;

        DarkModeChanged = null;

        if (_lazyDotNetRef.IsValueCreated)
        {
            _lazyDotNetRef.Value.Dispose();
        }

        await StopWatchingDarkMode();
    }

    private Palette GetCurrentPalette()
    {
        var theme = GetTheme();

        return _isDarkModeState.Value ? theme.PaletteDark : theme.PaletteLight;
    }

    private async Task OnObserveSystemDarkModeChangeChanged(ParameterChangedEventArgs<bool> arg)
    {
        // The _observing flag prevents attempting to stop observation when it hasn't been started.
        // For example, ObserveSystemDarkModeChange is true by default, and if it's set to false in the initial component setup 
        // like <MudThemeProvider ObserveSystemDarkModeChange="false" />, the ChangeHandler of ParameterState will be invoked.
        // Therefore, it's not desirable to stop an observation that hasn't been started.
        if (arg.Value)
        {
            if (!_observing)
            {
                _observing = true;
                await WatchDarkMode();
            }
        }
        else
        {
            if (_observing)
            {
                _observing = false;
                await StopWatchingDarkMode();
            }
        }
    }

    private ValueTask WatchDarkMode() => JsRuntime.InvokeVoidAsyncIgnoreErrors("mudThemeProvider.watchDarkMode", _lazyDotNetRef.Value);

    private ValueTask StopWatchingDarkMode() => JsRuntime.InvokeVoidAsyncIgnoreErrors("mudThemeProvider.stopWatchingDarkMode");

    private DotNetObjectReference<MudThemeProvider> CreateDotNetObjectReference() => DotNetObjectReference.Create(this);

    private static string FormatFontFamily(string[] fontFamilies)
    {
        return string.Join(", ", fontFamilies.Select(font => font.Contains(' ') ? $"'{font}'" : font));
    }
}
