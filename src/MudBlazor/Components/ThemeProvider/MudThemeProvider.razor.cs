using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
partial class MudThemeProvider : ComponentBaseWithState, IDisposable
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

    private MudTheme? _theme;
    private readonly ParameterState<bool> _isDarkModeState;
    private readonly ParameterState<bool> _observeSystemThemeChangeState;
    private readonly Lazy<DotNetObjectReference<MudThemeProvider>> _lazyDotNetRef;

    private event Func<bool, Task>? _darkLightModeChanged;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The theme used by the application.
    /// </summary>
    [Parameter]
    public MudTheme? Theme { get; set; }

    /// <summary>
    ///  If true, will not apply MudBlazor styled scrollbar and use browser default. 
    /// </summary>
    [Parameter]
    public bool DefaultScrollbar { get; set; }

    /// <summary>
    /// Sets a value indicating whether to observe changes in the system theme preference.
    /// Default is <c>true</c>.
    /// </summary>
    [Parameter]
    public bool ObserveSystemThemeChange { get; set; } = true;

    /// <summary>
    /// The active palette of the theme.
    /// </summary>
    [Parameter]
    public bool IsDarkMode { get; set; }

    /// <summary>
    /// Invoked when the dark mode changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsDarkModeChanged { get; set; }

    [DynamicDependency(nameof(SystemPreferenceChanged))]
    public MudThemeProvider()
    {
        using var registerScope = CreateRegisterScope();
        _isDarkModeState = registerScope.RegisterParameter<bool>(nameof(IsDarkMode))
            .WithParameter(() => IsDarkMode)
            .WithEventCallback(() => IsDarkModeChanged);
        _observeSystemThemeChangeState = registerScope
            .RegisterParameter<bool>(nameof(ObserveSystemThemeChange))
            .WithParameter(() => ObserveSystemThemeChange)
            .WithChangeHandler(OnObserveSystemThemeChangeChanged);
        _lazyDotNetRef = new Lazy<DotNetObjectReference<MudThemeProvider>>(CreateDotNetObjectReference);
    }

    /// <summary>
    /// Returns the dark mode preference of the user. True if dark mode is preferred.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> GetSystemPreference()
    {
        var (_, value) = await JsRuntime.InvokeAsyncWithErrorHandling(false, "darkModeChange");

        return value;
    }

    public Task WatchSystemPreference(Func<bool, Task> functionOnChange)
    {
        _darkLightModeChanged += functionOnChange;

        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task SystemPreferenceChanged(bool isDarkMode)
    {
        await _isDarkModeState.SetValueAsync(isDarkMode);
        var handler = _darkLightModeChanged;
        if (handler is not null)
        {
            await handler(isDarkMode);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_observeSystemThemeChangeState.Value && !_observing)
            {
                _observing = true;
                await WatchDarkThemeMedia();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnInitialized()
    {
        _theme = Theme ?? new MudTheme();
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (Theme is not null)
        {
            if (!ReferenceEquals(_theme, Theme))
            {
                _theme = Theme;
            }
        }

        base.OnParametersSet();
    }

    protected string BuildTheme()
    {
        _theme = Theme ?? new MudTheme();
        var theme = new StringBuilder();
        theme.AppendLine("<style>");
        theme.Append(_theme.PseudoCss.Scope);
        theme.AppendLine("{");
        GenerateTheme(theme);
        theme.AppendLine("}");
        theme.AppendLine("</style>");

        return theme.ToString();
    }

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

    protected virtual void GenerateTheme(StringBuilder theme)
    {
        if (_theme is null)
        {
            return;
        }

        Palette palette = _isDarkModeState.Value ? _theme.PaletteDark : _theme.PaletteLight;

        //Palette
        theme.AppendLine($"--{Palette}-black: {palette.Black};");
        theme.AppendLine($"--{Palette}-white: {palette.White};");

        theme.AppendLine($"--{Palette}-primary: {palette.Primary};");
        theme.AppendLine(
            $"--{Palette}-primary-rgb: {palette.Primary.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-primary-text: {palette.PrimaryContrastText};");
        theme.AppendLine($"--{Palette}-primary-darken: {palette.PrimaryDarken};");
        theme.AppendLine($"--{Palette}-primary-lighten: {palette.PrimaryLighten};");
        theme.AppendLine(
            $"--{Palette}-primary-hover: {palette.Primary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-secondary: {palette.Secondary};");
        theme.AppendLine(
            $"--{Palette}-secondary-rgb: {palette.Secondary.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-secondary-text: {palette.SecondaryContrastText};");
        theme.AppendLine($"--{Palette}-secondary-darken: {palette.SecondaryDarken};");
        theme.AppendLine($"--{Palette}-secondary-lighten: {palette.SecondaryLighten};");
        theme.AppendLine(
            $"--{Palette}-secondary-hover: {palette.Secondary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-tertiary: {palette.Tertiary};");
        theme.AppendLine(
            $"--{Palette}-tertiary-rgb: {palette.Tertiary.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-tertiary-text: {palette.TertiaryContrastText};");
        theme.AppendLine($"--{Palette}-tertiary-darken: {palette.TertiaryDarken};");
        theme.AppendLine($"--{Palette}-tertiary-lighten: {palette.TertiaryLighten};");
        theme.AppendLine(
            $"--{Palette}-tertiary-hover: {palette.Tertiary.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-info: {palette.Info};");
        theme.AppendLine(
            $"--{Palette}-info-rgb: {palette.Info.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-info-text: {palette.InfoContrastText};");
        theme.AppendLine($"--{Palette}-info-darken: {palette.InfoDarken};");
        theme.AppendLine($"--{Palette}-info-lighten: {palette.InfoLighten};");
        theme.AppendLine(
            $"--{Palette}-info-hover: {palette.Info.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-success: {palette.Success};");
        theme.AppendLine(
            $"--{Palette}-success-rgb: {palette.Success.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-success-text: {palette.SuccessContrastText};");
        theme.AppendLine($"--{Palette}-success-darken: {palette.SuccessDarken};");
        theme.AppendLine($"--{Palette}-success-lighten: {palette.SuccessLighten};");
        theme.AppendLine(
            $"--{Palette}-success-hover: {palette.Success.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-warning: {palette.Warning};");
        theme.AppendLine(
            $"--{Palette}-warning-rgb: {palette.Warning.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-warning-text: {palette.WarningContrastText};");
        theme.AppendLine($"--{Palette}-warning-darken: {palette.WarningDarken};");
        theme.AppendLine($"--{Palette}-warning-lighten: {palette.WarningLighten};");
        theme.AppendLine(
            $"--{Palette}-warning-hover: {palette.Warning.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-error: {palette.Error};");
        theme.AppendLine(
            $"--{Palette}-error-rgb: {palette.Error.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-error-text: {palette.ErrorContrastText};");
        theme.AppendLine($"--{Palette}-error-darken: {palette.ErrorDarken};");
        theme.AppendLine($"--{Palette}-error-lighten: {palette.ErrorLighten};");
        theme.AppendLine(
            $"--{Palette}-error-hover: {palette.Error.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-dark: {palette.Dark};");
        theme.AppendLine(
            $"--{Palette}-dark-rgb: {palette.Dark.ToString(MudColorOutputFormats.ColorElements)};");
        theme.AppendLine($"--{Palette}-dark-text: {palette.DarkContrastText};");
        theme.AppendLine($"--{Palette}-dark-darken: {palette.DarkDarken};");
        theme.AppendLine($"--{Palette}-dark-lighten: {palette.DarkLighten};");
        theme.AppendLine(
            $"--{Palette}-dark-hover: {palette.Dark.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");

        theme.AppendLine($"--{Palette}-text-primary: {palette.TextPrimary};");
        theme.AppendLine($"--{Palette}-text-secondary: {palette.TextSecondary};");
        theme.AppendLine($"--{Palette}-text-disabled: {palette.TextDisabled};");

        theme.AppendLine($"--{Palette}-action-default: {palette.ActionDefault};");
        theme.AppendLine(
            $"--{Palette}-action-default-hover: {palette.ActionDefault.SetAlpha(palette.HoverOpacity).ToString(MudColorOutputFormats.RGBA)};");
        theme.AppendLine($"--{Palette}-action-disabled: {palette.ActionDisabled};");
        theme.AppendLine(
            $"--{Palette}-action-disabled-background: {palette.ActionDisabledBackground};");

        theme.AppendLine($"--{Palette}-surface: {palette.Surface};");
        theme.AppendLine($"--{Palette}-background: {palette.Background};");
        theme.AppendLine($"--{Palette}-background-gray: {palette.BackgroundGray};");
        theme.AppendLine($"--{Palette}-drawer-background: {palette.DrawerBackground};");
        theme.AppendLine($"--{Palette}-drawer-text: {palette.DrawerText};");
        theme.AppendLine($"--{Palette}-drawer-icon: {palette.DrawerIcon};");
        theme.AppendLine($"--{Palette}-appbar-background: {palette.AppbarBackground};");
        theme.AppendLine($"--{Palette}-appbar-text: {palette.AppbarText};");

        theme.AppendLine($"--{Palette}-lines-default: {palette.LinesDefault};");
        theme.AppendLine($"--{Palette}-lines-inputs: {palette.LinesInputs};");

        theme.AppendLine($"--{Palette}-table-lines: {palette.TableLines};");
        theme.AppendLine($"--{Palette}-table-striped: {palette.TableStriped};");
        theme.AppendLine($"--{Palette}-table-hover: {palette.TableHover};");

        theme.AppendLine($"--{Palette}-divider: {palette.Divider};");
        theme.AppendLine($"--{Palette}-divider-light: {palette.DividerLight};");

        theme.AppendLine($"--{Palette}-gray-default: {palette.GrayDefault};");
        theme.AppendLine($"--{Palette}-gray-light: {palette.GrayLight};");
        theme.AppendLine($"--{Palette}-gray-lighter: {palette.GrayLighter};");
        theme.AppendLine($"--{Palette}-gray-dark: {palette.GrayDark};");
        theme.AppendLine($"--{Palette}-gray-darker: {palette.GrayDarker};");

        theme.AppendLine($"--{Palette}-overlay-dark: {palette.OverlayDark};");
        theme.AppendLine($"--{Palette}-overlay-light: {palette.OverlayLight};");

        //Ripple
        theme.AppendLine($"--{Ripple}-color: var(--{Palette}-text-primary);");
        theme.AppendLine($"--{Ripple}-opacity: {_theme.PaletteLight.RippleOpacity.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Ripple}-opacity-secondary: {_theme.PaletteLight.RippleOpacitySecondary.ToString(CultureInfo.InvariantCulture)};");

        //Elevations
        theme.AppendLine($"--{Elevation}-0: {_theme.Shadows.Elevation.GetValue(0)};");
        theme.AppendLine($"--{Elevation}-1: {_theme.Shadows.Elevation.GetValue(1)};");
        theme.AppendLine($"--{Elevation}-2: {_theme.Shadows.Elevation.GetValue(2)};");
        theme.AppendLine($"--{Elevation}-3: {_theme.Shadows.Elevation.GetValue(3)};");
        theme.AppendLine($"--{Elevation}-4: {_theme.Shadows.Elevation.GetValue(4)};");
        theme.AppendLine($"--{Elevation}-5: {_theme.Shadows.Elevation.GetValue(5)};");
        theme.AppendLine($"--{Elevation}-6: {_theme.Shadows.Elevation.GetValue(6)};");
        theme.AppendLine($"--{Elevation}-7: {_theme.Shadows.Elevation.GetValue(7)};");
        theme.AppendLine($"--{Elevation}-8: {_theme.Shadows.Elevation.GetValue(8)};");
        theme.AppendLine($"--{Elevation}-9: {_theme.Shadows.Elevation.GetValue(9)};");
        theme.AppendLine($"--{Elevation}-10: {_theme.Shadows.Elevation.GetValue(10)};");
        theme.AppendLine($"--{Elevation}-11: {_theme.Shadows.Elevation.GetValue(11)};");
        theme.AppendLine($"--{Elevation}-12: {_theme.Shadows.Elevation.GetValue(12)};");
        theme.AppendLine($"--{Elevation}-13: {_theme.Shadows.Elevation.GetValue(13)};");
        theme.AppendLine($"--{Elevation}-14: {_theme.Shadows.Elevation.GetValue(14)};");
        theme.AppendLine($"--{Elevation}-15: {_theme.Shadows.Elevation.GetValue(15)};");
        theme.AppendLine($"--{Elevation}-16: {_theme.Shadows.Elevation.GetValue(16)};");
        theme.AppendLine($"--{Elevation}-17: {_theme.Shadows.Elevation.GetValue(17)};");
        theme.AppendLine($"--{Elevation}-18: {_theme.Shadows.Elevation.GetValue(18)};");
        theme.AppendLine($"--{Elevation}-19: {_theme.Shadows.Elevation.GetValue(19)};");
        theme.AppendLine($"--{Elevation}-20: {_theme.Shadows.Elevation.GetValue(20)};");
        theme.AppendLine($"--{Elevation}-21: {_theme.Shadows.Elevation.GetValue(21)};");
        theme.AppendLine($"--{Elevation}-22: {_theme.Shadows.Elevation.GetValue(22)};");
        theme.AppendLine($"--{Elevation}-23: {_theme.Shadows.Elevation.GetValue(23)};");
        theme.AppendLine($"--{Elevation}-24: {_theme.Shadows.Elevation.GetValue(24)};");
        theme.AppendLine($"--{Elevation}-25: {_theme.Shadows.Elevation.GetValue(25)};");

        //Layout Properties
        theme.AppendLine(
            $"--{LayoutProperties}-default-borderradius: {_theme.LayoutProperties.DefaultBorderRadius};");
        theme.AppendLine($"--{LayoutProperties}-drawer-width-left: {_theme.LayoutProperties.DrawerWidthLeft};");
        theme.AppendLine($"--{LayoutProperties}-drawer-width-right: {_theme.LayoutProperties.DrawerWidthRight};");
        theme.AppendLine(
            $"--{LayoutProperties}-drawer-width-mini-left: {_theme.LayoutProperties.DrawerMiniWidthLeft};");
        theme.AppendLine(
            $"--{LayoutProperties}-drawer-width-mini-right: {_theme.LayoutProperties.DrawerMiniWidthRight};");
        theme.AppendLine($"--{LayoutProperties}-appbar-height: {_theme.LayoutProperties.AppbarHeight};");

        //Breakpoint
        //theme.AppendLine($"--{Breakpoint}-xs: {Theme.Breakpoints.xs};");
        //theme.AppendLine($"--{Breakpoint}-sm: {Theme.Breakpoints.sm};");
        //theme.AppendLine($"--{Breakpoint}-md: {Theme.Breakpoints.md};");
        //theme.AppendLine($"--{Breakpoint}-lg: {Theme.Breakpoints.lg};");
        //theme.AppendLine($"--{Breakpoint}-xl: {Theme.Breakpoints.xl};");
        //theme.AppendLine($"--{Breakpoint}-xxl: {Theme.Breakpoints.xxl};");

        //Typography
        theme.AppendLine(
            $"--{Typography}-default-family: '{string.Join("','", _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-default-size: {_theme.Typography.Default.FontSize};");
        theme.AppendLine($"--{Typography}-default-weight: {_theme.Typography.Default.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-default-lineheight: {_theme.Typography.Default.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-default-letterspacing: {_theme.Typography.Default.LetterSpacing};");
        theme.AppendLine($"--{Typography}-default-text-transform: {_theme.Typography.Default.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h1-family: '{string.Join("','", _theme.Typography.H1.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h1-size: {_theme.Typography.H1.FontSize};");
        theme.AppendLine($"--{Typography}-h1-weight: {_theme.Typography.H1.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h1-lineheight: {_theme.Typography.H1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h1-letterspacing: {_theme.Typography.H1.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h1-text-transform: {_theme.Typography.H1.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h2-family: '{string.Join("','", _theme.Typography.H2.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h2-size: {_theme.Typography.H2.FontSize};");
        theme.AppendLine($"--{Typography}-h2-weight: {_theme.Typography.H2.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h2-lineheight: {_theme.Typography.H2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h2-letterspacing: {_theme.Typography.H2.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h2-text-transform: {_theme.Typography.H2.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h3-family: '{string.Join("','", _theme.Typography.H3.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h3-size: {_theme.Typography.H3.FontSize};");
        theme.AppendLine($"--{Typography}-h3-weight: {_theme.Typography.H3.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h3-lineheight: {_theme.Typography.H3.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h3-letterspacing: {_theme.Typography.H3.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h3-text-transform: {_theme.Typography.H3.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h4-family: '{string.Join("','", _theme.Typography.H4.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h4-size: {_theme.Typography.H4.FontSize};");
        theme.AppendLine($"--{Typography}-h4-weight: {_theme.Typography.H4.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h4-lineheight: {_theme.Typography.H4.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h4-letterspacing: {_theme.Typography.H4.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h4-text-transform: {_theme.Typography.H4.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h5-family: '{string.Join("','", _theme.Typography.H5.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h5-size: {_theme.Typography.H5.FontSize};");
        theme.AppendLine($"--{Typography}-h5-weight: {_theme.Typography.H5.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h5-lineheight: {_theme.Typography.H5.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h5-letterspacing: {_theme.Typography.H5.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h5-text-transform: {_theme.Typography.H5.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-h6-family: '{string.Join("','", _theme.Typography.H6.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-h6-size: {_theme.Typography.H6.FontSize};");
        theme.AppendLine($"--{Typography}-h6-weight: {_theme.Typography.H6.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-h6-lineheight: {_theme.Typography.H6.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-h6-letterspacing: {_theme.Typography.H6.LetterSpacing};");
        theme.AppendLine($"--{Typography}-h6-text-transform: {_theme.Typography.H6.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-subtitle1-family: '{string.Join("','", _theme.Typography.Subtitle1.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-subtitle1-size: {_theme.Typography.Subtitle1.FontSize};");
        theme.AppendLine($"--{Typography}-subtitle1-weight: {_theme.Typography.Subtitle1.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-subtitle1-lineheight: {_theme.Typography.Subtitle1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-subtitle1-letterspacing: {_theme.Typography.Subtitle1.LetterSpacing};");
        theme.AppendLine($"--{Typography}-subtitle1-text-transform: {_theme.Typography.Subtitle1.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-subtitle2-family: '{string.Join("','", _theme.Typography.Subtitle2.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-subtitle2-size: {_theme.Typography.Subtitle2.FontSize};");
        theme.AppendLine($"--{Typography}-subtitle2-weight: {_theme.Typography.Subtitle2.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-subtitle2-lineheight: {_theme.Typography.Subtitle2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-subtitle2-letterspacing: {_theme.Typography.Subtitle2.LetterSpacing};");
        theme.AppendLine($"--{Typography}-subtitle2-text-transform: {_theme.Typography.Subtitle2.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-body1-family: '{string.Join("','", _theme.Typography.Body1.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-body1-size: {_theme.Typography.Body1.FontSize};");
        theme.AppendLine($"--{Typography}-body1-weight: {_theme.Typography.Body1.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-body1-lineheight: {_theme.Typography.Body1.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-body1-letterspacing: {_theme.Typography.Body1.LetterSpacing};");
        theme.AppendLine($"--{Typography}-body1-text-transform: {_theme.Typography.Body1.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-body2-family: '{string.Join("','", _theme.Typography.Body2.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-body2-size: {_theme.Typography.Body2.FontSize};");
        theme.AppendLine($"--{Typography}-body2-weight: {_theme.Typography.Body2.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-body2-lineheight: {_theme.Typography.Body2.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-body2-letterspacing: {_theme.Typography.Body2.LetterSpacing};");
        theme.AppendLine($"--{Typography}-body2-text-transform: {_theme.Typography.Body2.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-input-family: '{string.Join("','", _theme.Typography.Input.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-input-size: {_theme.Typography.Input.FontSize};");
        theme.AppendLine($"--{Typography}-input-weight: {_theme.Typography.Input.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-input-lineheight: {_theme.Typography.Input.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-input-letterspacing: {_theme.Typography.Input.LetterSpacing};");
        theme.AppendLine($"--{Typography}-input-text-transform: {_theme.Typography.Input.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-button-family: '{string.Join("','", _theme.Typography.Button.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-button-size: {_theme.Typography.Button.FontSize};");
        theme.AppendLine($"--{Typography}-button-weight: {_theme.Typography.Button.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-button-lineheight: {_theme.Typography.Button.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-button-letterspacing: {_theme.Typography.Button.LetterSpacing};");
        theme.AppendLine($"--{Typography}-button-text-transform: {_theme.Typography.Button.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-caption-family: '{string.Join("','", _theme.Typography.Caption.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-caption-size: {_theme.Typography.Caption.FontSize};");
        theme.AppendLine($"--{Typography}-caption-weight: {_theme.Typography.Caption.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-caption-lineheight: {_theme.Typography.Caption.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-caption-letterspacing: {_theme.Typography.Caption.LetterSpacing};");
        theme.AppendLine($"--{Typography}-caption-text-transform: {_theme.Typography.Caption.TextTransform};");

        theme.AppendLine(
            $"--{Typography}-overline-family: '{string.Join("','", _theme.Typography.Overline.FontFamily ?? _theme.Typography.Default.FontFamily ?? Array.Empty<string>())}';");
        theme.AppendLine($"--{Typography}-overline-size: {_theme.Typography.Overline.FontSize};");
        theme.AppendLine($"--{Typography}-overline-weight: {_theme.Typography.Overline.FontWeight};");
        theme.AppendLine(
            $"--{Typography}-overline-lineheight: {_theme.Typography.Overline.LineHeight.ToString(CultureInfo.InvariantCulture)};");
        theme.AppendLine($"--{Typography}-overline-letterspacing: {_theme.Typography.Overline.LetterSpacing};");
        theme.AppendLine($"--{Typography}-overline-text-transform: {_theme.Typography.Overline.TextTransform};");

        //Z-Index
        theme.AppendLine($"--{Zindex}-drawer: {_theme.ZIndex.Drawer};");
        theme.AppendLine($"--{Zindex}-appbar: {_theme.ZIndex.AppBar};");
        theme.AppendLine($"--{Zindex}-dialog: {_theme.ZIndex.Dialog};");
        theme.AppendLine($"--{Zindex}-popover: {_theme.ZIndex.Popover};");
        theme.AppendLine($"--{Zindex}-snackbar: {_theme.ZIndex.Snackbar};");
        theme.AppendLine($"--{Zindex}-tooltip: {_theme.ZIndex.Tooltip};");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _darkLightModeChanged = null;
            if (_lazyDotNetRef.IsValueCreated)
            {
                _lazyDotNetRef.Value.Dispose();
                // When .NET7 is dropped we can use async Dispose, but for now MAUI has bug https://github.com/MudBlazor/MudBlazor/pull/5367#issuecomment-1258649968.
                _ = StopWatchingDarkThemeMedia();
            }
        }
    }

    private async Task OnObserveSystemThemeChangeChanged(ParameterChangedEventArgs<bool> arg)
    {
        // The _observing flag prevents attempting to stop observation when it hasn't been started.
        // For example, ObserveSystemThemeChange is true by default, and if it's set to false in the initial component setup 
        // like <MudThemeProvider ObserveSystemThemeChange="false" />, the ChangeHandler of ParameterState will be invoked.
        // Therefore, it's not desirable to stop an observation that hasn't been started.
        if (arg.Value)
        {
            if (!_observing)
            {
                _observing = true;
                await WatchDarkThemeMedia();
            }
        }
        else
        {
            if (_observing)
            {
                _observing = false;
                await StopWatchingDarkThemeMedia();
            }
        }
    }

    private ValueTask WatchDarkThemeMedia() => JsRuntime.InvokeVoidAsyncIgnoreErrors("watchDarkThemeMedia", _lazyDotNetRef.Value);

    private ValueTask StopWatchingDarkThemeMedia() => JsRuntime.InvokeVoidAsyncIgnoreErrors("stopWatchingDarkThemeMedia");

    private DotNetObjectReference<MudThemeProvider> CreateDotNetObjectReference() => DotNetObjectReference.Create(this);
}
