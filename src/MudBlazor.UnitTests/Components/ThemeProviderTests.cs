using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class ThemeProviderTests : BunitTest
    {
        [Test]
        [TestCase("en-us")]
        [TestCase("de-DE")]
        [TestCase("he-IL")]
        [TestCase("ar-ER")]
        public void DifferentCultures(string cultureString)
        {
            var culture = new CultureInfo(cultureString, false);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var comp = Context.RenderComponent<MudThemeProvider>();

            var styleNodes = comp.Nodes.OfType<IHtmlStyleElement>().ToArray();
            styleNodes.Should().HaveCount(3);

            var rootStyleNode = styleNodes[2];

            var expectedLines = new[] {
                ":root{",
                "--mud-palette-black: rgba(39,44,52,1);",
                "--mud-palette-white: rgba(255,255,255,1);",
                "--mud-palette-primary: rgba(89,74,226,1);",
                "--mud-palette-primary-rgb: 89,74,226;",
                "--mud-palette-primary-text: rgba(255,255,255,1);",
                "--mud-palette-primary-darken: rgb(62,44,221);",
                "--mud-palette-primary-lighten: rgb(118,106,231);",
                "--mud-palette-primary-hover: rgba(89,74,226,0.058823529411764705);",
                "--mud-palette-secondary: rgba(255,64,129,1);",
                "--mud-palette-secondary-rgb: 255,64,129;",
                "--mud-palette-secondary-text: rgba(255,255,255,1);",
                "--mud-palette-secondary-darken: rgb(255,31,105);",
                "--mud-palette-secondary-lighten: rgb(255,102,153);",
                "--mud-palette-secondary-hover: rgba(255,64,129,0.058823529411764705);",
                "--mud-palette-tertiary: rgba(30,200,165,1);",
                "--mud-palette-tertiary-rgb: 30,200,165;",
                "--mud-palette-tertiary-text: rgba(255,255,255,1);",
                "--mud-palette-tertiary-darken: rgb(25,169,140);",
                "--mud-palette-tertiary-lighten: rgb(42,223,187);",
                "--mud-palette-tertiary-hover: rgba(30,200,165,0.058823529411764705);",
                "--mud-palette-info: rgba(33,150,243,1);",
                "--mud-palette-info-rgb: 33,150,243;",
                "--mud-palette-info-text: rgba(255,255,255,1);",
                "--mud-palette-info-darken: rgb(12,128,223);",
                "--mud-palette-info-lighten: rgb(71,167,245);",
                "--mud-palette-info-hover: rgba(33,150,243,0.058823529411764705);",
                "--mud-palette-success: rgba(0,200,83,1);",
                "--mud-palette-success-rgb: 0,200,83;",
                "--mud-palette-success-text: rgba(255,255,255,1);",
                "--mud-palette-success-darken: rgb(0,163,68);",
                "--mud-palette-success-lighten: rgb(0,235,98);",
                "--mud-palette-success-hover: rgba(0,200,83,0.058823529411764705);",
                "--mud-palette-warning: rgba(255,152,0,1);",
                "--mud-palette-warning-rgb: 255,152,0;",
                "--mud-palette-warning-text: rgba(255,255,255,1);",
                "--mud-palette-warning-darken: rgb(214,129,0);",
                "--mud-palette-warning-lighten: rgb(255,167,36);",
                "--mud-palette-warning-hover: rgba(255,152,0,0.058823529411764705);",
                "--mud-palette-error: rgba(244,67,54,1);",
                "--mud-palette-error-rgb: 244,67,54;",
                "--mud-palette-error-text: rgba(255,255,255,1);",
                "--mud-palette-error-darken: rgb(242,28,13);",
                "--mud-palette-error-lighten: rgb(246,96,85);",
                "--mud-palette-error-hover: rgba(244,67,54,0.058823529411764705);",
                "--mud-palette-dark: rgba(66,66,66,1);",
                "--mud-palette-dark-rgb: 66,66,66;",
                "--mud-palette-dark-text: rgba(255,255,255,1);",
                "--mud-palette-dark-darken: rgb(46,46,46);",
                "--mud-palette-dark-lighten: rgb(87,87,87);",
                "--mud-palette-dark-hover: rgba(66,66,66,0.058823529411764705);",
                "--mud-palette-text-primary: rgba(66,66,66,1);",
                "--mud-palette-text-secondary: rgba(0,0,0,0.5372549019607843);",
                "--mud-palette-text-disabled: rgba(0,0,0,0.3764705882352941);",
                "--mud-palette-action-default: rgba(0,0,0,0.5372549019607843);",
                "--mud-palette-action-default-hover: rgba(0,0,0,0.058823529411764705);",
                "--mud-palette-action-disabled: rgba(0,0,0,0.25882352941176473);",
                "--mud-palette-action-disabled-background: rgba(0,0,0,0.11764705882352941);",
                "--mud-palette-surface: rgba(255,255,255,1);",
                "--mud-palette-background: rgba(255,255,255,1);",
                "--mud-palette-background-gray: rgba(245,245,245,1);",
                "--mud-palette-drawer-background: rgba(255,255,255,1);",
                "--mud-palette-drawer-text: rgba(66,66,66,1);",
                "--mud-palette-drawer-icon: rgba(97,97,97,1);",
                "--mud-palette-appbar-background: rgba(89,74,226,1);",
                "--mud-palette-appbar-text: rgba(255,255,255,1);",
                "--mud-palette-lines-default: rgba(0,0,0,0.11764705882352941);",
                "--mud-palette-lines-inputs: rgba(189,189,189,1);",
                "--mud-palette-table-lines: rgba(224,224,224,1);",
                "--mud-palette-table-striped: rgba(0,0,0,0.0196078431372549);",
                "--mud-palette-table-hover: rgba(0,0,0,0.0392156862745098);",
                "--mud-palette-divider: rgba(224,224,224,1);",
                "--mud-palette-divider-light: rgba(0,0,0,0.8);",
                "--mud-palette-gray-default: #9E9E9E;",
                "--mud-palette-gray-light: #BDBDBD;",
                "--mud-palette-gray-lighter: #E0E0E0;",
                "--mud-palette-gray-dark: #757575;",
                "--mud-palette-gray-darker: #616161;",
                "--mud-palette-overlay-dark: rgba(33,33,33,0.4980392156862745);",
                "--mud-palette-overlay-light: rgba(255,255,255,0.4980392156862745);",
                "--mud-ripple-color: var(--mud-palette-text-primary);",
                "--mud-ripple-opacity: 0.1;",
                "--mud-ripple-opacity-secondary: 0.2;",
                "--mud-elevation-0: none;",
                "--mud-elevation-1: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-2: 0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-3: 0px 3px 3px -2px rgba(0,0,0,0.2),0px 3px 4px 0px rgba(0,0,0,0.14),0px 1px 8px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-4: 0px 2px 4px -1px rgba(0,0,0,0.2),0px 4px 5px 0px rgba(0,0,0,0.14),0px 1px 10px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-5: 0px 3px 5px -1px rgba(0,0,0,0.2),0px 5px 8px 0px rgba(0,0,0,0.14),0px 1px 14px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-6: 0px 3px 5px -1px rgba(0,0,0,0.2),0px 6px 10px 0px rgba(0,0,0,0.14),0px 1px 18px 0px rgba(0,0,0,0.12);",
                "--mud-elevation-7: 0px 4px 5px -2px rgba(0,0,0,0.2),0px 7px 10px 1px rgba(0,0,0,0.14),0px 2px 16px 1px rgba(0,0,0,0.12);",
                "--mud-elevation-8: 0px 5px 5px -3px rgba(0,0,0,0.2),0px 8px 10px 1px rgba(0,0,0,0.14),0px 3px 14px 2px rgba(0,0,0,0.12);",
                "--mud-elevation-9: 0px 5px 6px -3px rgba(0,0,0,0.2),0px 9px 12px 1px rgba(0,0,0,0.14),0px 3px 16px 2px rgba(0,0,0,0.12);",
                "--mud-elevation-10: 0px 6px 6px -3px rgba(0,0,0,0.2),0px 10px 14px 1px rgba(0,0,0,0.14),0px 4px 18px 3px rgba(0,0,0,0.12);",
                "--mud-elevation-11: 0px 6px 7px -4px rgba(0,0,0,0.2),0px 11px 15px 1px rgba(0,0,0,0.14),0px 4px 20px 3px rgba(0,0,0,0.12);",
                "--mud-elevation-12: 0px 7px 8px -4px rgba(0,0,0,0.2),0px 12px 17px 2px rgba(0,0,0,0.14),0px 5px 22px 4px rgba(0,0,0,0.12);",
                "--mud-elevation-13: 0px 7px 8px -4px rgba(0,0,0,0.2),0px 13px 19px 2px rgba(0,0,0,0.14),0px 5px 24px 4px rgba(0,0,0,0.12);",
                "--mud-elevation-14: 0px 7px 9px -4px rgba(0,0,0,0.2),0px 14px 21px 2px rgba(0,0,0,0.14),0px 5px 26px 4px rgba(0,0,0,0.12);",
                "--mud-elevation-15: 0px 8px 9px -5px rgba(0,0,0,0.2),0px 15px 22px 2px rgba(0,0,0,0.14),0px 6px 28px 5px rgba(0,0,0,0.12);",
                "--mud-elevation-16: 0px 8px 10px -5px rgba(0,0,0,0.2),0px 16px 24px 2px rgba(0,0,0,0.14),0px 6px 30px 5px rgba(0,0,0,0.12);",
                "--mud-elevation-17: 0px 8px 11px -5px rgba(0,0,0,0.2),0px 17px 26px 2px rgba(0,0,0,0.14),0px 6px 32px 5px rgba(0,0,0,0.12);",
                "--mud-elevation-18: 0px 9px 11px -5px rgba(0,0,0,0.2),0px 18px 28px 2px rgba(0,0,0,0.14),0px 7px 34px 6px rgba(0,0,0,0.12);",
                "--mud-elevation-19: 0px 9px 12px -6px rgba(0,0,0,0.2),0px 19px 29px 2px rgba(0,0,0,0.14),0px 7px 36px 6px rgba(0,0,0,0.12);",
                "--mud-elevation-20: 0px 10px 13px -6px rgba(0,0,0,0.2),0px 20px 31px 3px rgba(0,0,0,0.14),0px 8px 38px 7px rgba(0,0,0,0.12);",
                "--mud-elevation-21: 0px 10px 13px -6px rgba(0,0,0,0.2),0px 21px 33px 3px rgba(0,0,0,0.14),0px 8px 40px 7px rgba(0,0,0,0.12);",
                "--mud-elevation-22: 0px 10px 14px -6px rgba(0,0,0,0.2),0px 22px 35px 3px rgba(0,0,0,0.14),0px 8px 42px 7px rgba(0,0,0,0.12);",
                "--mud-elevation-23: 0px 11px 14px -7px rgba(0,0,0,0.2),0px 23px 36px 3px rgba(0,0,0,0.14),0px 9px 44px 8px rgba(0,0,0,0.12);",
                "--mud-elevation-24: 0px 11px 15px -7px rgba(0,0,0,0.2),0px 24px 38px 3px rgba(0,0,0,0.14),0px 9px 46px 8px rgba(0,0,0,0.12);",
                "--mud-elevation-25: 0 5px 5px -3px rgba(0,0,0,.06), 0 8px 10px 1px rgba(0,0,0,.042), 0 3px 14px 2px rgba(0,0,0,.036);",
                "--mud-default-borderradius: 4px;",
                "--mud-drawer-width-left: 240px;",
                "--mud-drawer-width-right: 240px;",
                "--mud-drawer-width-mini-left: 56px;",
                "--mud-drawer-width-mini-right: 56px;",
                "--mud-appbar-height: 64px;",
                "--mud-typography-default-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-default-size: .875rem;",
                "--mud-typography-default-weight: 400;",
                "--mud-typography-default-lineheight: 1.43;",
                "--mud-typography-default-letterspacing: .01071em;",
                "--mud-typography-default-text-transform: none;",
                "--mud-typography-h1-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h1-size: 6rem;",
                "--mud-typography-h1-weight: 300;",
                "--mud-typography-h1-lineheight: 1.167;",
                "--mud-typography-h1-letterspacing: -.01562em;",
                "--mud-typography-h1-text-transform: none;",
                "--mud-typography-h2-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h2-size: 3.75rem;",
                "--mud-typography-h2-weight: 300;",
                "--mud-typography-h2-lineheight: 1.2;",
                "--mud-typography-h2-letterspacing: -.00833em;",
                "--mud-typography-h2-text-transform: none;",
                "--mud-typography-h3-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h3-size: 3rem;",
                "--mud-typography-h3-weight: 400;",
                "--mud-typography-h3-lineheight: 1.167;",
                "--mud-typography-h3-letterspacing: 0;",
                "--mud-typography-h3-text-transform: none;",
                "--mud-typography-h4-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h4-size: 2.125rem;",
                "--mud-typography-h4-weight: 400;",
                "--mud-typography-h4-lineheight: 1.235;",
                "--mud-typography-h4-letterspacing: .00735em;",
                "--mud-typography-h4-text-transform: none;",
                "--mud-typography-h5-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h5-size: 1.5rem;",
                "--mud-typography-h5-weight: 400;",
                "--mud-typography-h5-lineheight: 1.334;",
                "--mud-typography-h5-letterspacing: 0;",
                "--mud-typography-h5-text-transform: none;",
                "--mud-typography-h6-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-h6-size: 1.25rem;",
                "--mud-typography-h6-weight: 500;",
                "--mud-typography-h6-lineheight: 1.6;",
                "--mud-typography-h6-letterspacing: .0075em;",
                "--mud-typography-h6-text-transform: none;",
                "--mud-typography-subtitle1-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-subtitle1-size: 1rem;",
                "--mud-typography-subtitle1-weight: 400;",
                "--mud-typography-subtitle1-lineheight: 1.75;",
                "--mud-typography-subtitle1-letterspacing: .00938em;",
                "--mud-typography-subtitle1-text-transform: none;",
                "--mud-typography-subtitle2-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-subtitle2-size: .875rem;",
                "--mud-typography-subtitle2-weight: 500;",
                "--mud-typography-subtitle2-lineheight: 1.57;",
                "--mud-typography-subtitle2-letterspacing: .00714em;",
                "--mud-typography-subtitle2-text-transform: none;",
                "--mud-typography-body1-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-body1-size: 1rem;",
                "--mud-typography-body1-weight: 400;",
                "--mud-typography-body1-lineheight: 1.5;",
                "--mud-typography-body1-letterspacing: .00938em;",
                "--mud-typography-body1-text-transform: none;",
                "--mud-typography-body2-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-body2-size: .875rem;",
                "--mud-typography-body2-weight: 400;",
                "--mud-typography-body2-lineheight: 1.43;",
                "--mud-typography-body2-letterspacing: .01071em;",
                "--mud-typography-body2-text-transform: none;",
                "--mud-typography-input-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-input-size: 1rem;",
                "--mud-typography-input-weight: 400;",
                "--mud-typography-input-lineheight: 1.1876;",
                "--mud-typography-input-letterspacing: .00938em;",
                "--mud-typography-input-text-transform: none;",
                "--mud-typography-button-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-button-size: .875rem;",
                "--mud-typography-button-weight: 500;",
                "--mud-typography-button-lineheight: 1.75;",
                "--mud-typography-button-letterspacing: .02857em;",
                "--mud-typography-button-text-transform: uppercase;",
                "--mud-typography-caption-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-caption-size: .75rem;",
                "--mud-typography-caption-weight: 400;",
                "--mud-typography-caption-lineheight: 1.66;",
                "--mud-typography-caption-letterspacing: .03333em;",
                "--mud-typography-caption-text-transform: none;",
                "--mud-typography-overline-family: 'Roboto','Helvetica','Arial','sans-serif';",
                "--mud-typography-overline-size: .75rem;",
                "--mud-typography-overline-weight: 400;",
                "--mud-typography-overline-lineheight: 2.66;",
                "--mud-typography-overline-letterspacing: .08333em;",
                "--mud-typography-overline-text-transform: none;",
                "--mud-zindex-drawer: 1100;",
                "--mud-zindex-appbar: 1300;",
                "--mud-zindex-dialog: 1400;",
                "--mud-zindex-popover: 1200;",
                "--mud-zindex-snackbar: 1500;",
                "--mud-zindex-tooltip: 1600;",
                "}"
            };

            var styleLines = rootStyleNode.InnerHtml.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            styleLines.Should().BeEquivalentTo(expectedLines);
        }

        [Test]
        public void DarkMode_Test()
        {
            var comp = Context.RenderComponent<MudThemeProvider>(parameters => parameters
                .Add(p => p.IsDarkMode, true));
            comp.Should().NotBeNull();
            comp.Instance.GetState(x => x.IsDarkMode).Should().BeTrue();
        }

        [Test]
        public void CustomThemeDarkModeTest()
        {
            var myCustomTheme = new MudTheme
            {
                PaletteDark = new PaletteDark
                {
                    Primary = Colors.Blue.Lighten1,
                    Secondary = "#F50057"
                }
            };
            myCustomTheme.PaletteDark.Primary.Should().Be(new MudColor(Colors.Blue.Lighten1));// Set by user
            myCustomTheme.PaletteDark.Error.Should().Be(new MudColor("#f64e62"));// Default dark overwritten from light
            myCustomTheme.PaletteDark.White.Should().Be(new MudColor(Colors.Shades.White));// Equal in dark and light.
            myCustomTheme.PaletteDark.Secondary.Should().Be(new MudColor("#F50057"));// Setting not in PaletteDark()
        }

        [Test]
        public void CustomThemeDarkModePrimaryDerivateColorTest()
        {
            // ensure it is backwards compatible by setting Palette() instead of PaletteDark()
            var myCustomTheme = new MudTheme()
            {
                PaletteDark = new PaletteDark()
                {
                    Primary = Colors.Green.Darken1,
                }
            };
            var expectedDarkerColor = new MudColor(Colors.Green.Darken1).ColorRgbDarken();
            myCustomTheme.PaletteDark.Primary.Should().Be(new MudColor(Colors.Green.Darken1));// Set by user
            myCustomTheme.PaletteDark.PrimaryDarken.Should().Be(expectedDarkerColor.ToString(MudColorOutputFormats.RGB));// Set by user

        }

        [Test]
        public void CustomThemeDefaultTest()
        {
            var defaultTheme = new MudTheme();

            //Dark theme
            defaultTheme.PaletteDark.Should().BeOfType<PaletteDark>();
            defaultTheme.PaletteDark.Primary.Should().Be(new MudColor("#776be7"));
            defaultTheme.PaletteDark.Error.Should().Be(new MudColor("#f64e62"));
            defaultTheme.PaletteDark.White.Should().Be(new MudColor(Colors.Shades.White));

            //Light theme
            // Note we're testing against the base type
            defaultTheme.PaletteLight.Should().BeAssignableTo<Palette>();
            defaultTheme.PaletteLight.Primary.Should().Be(new MudColor("#594AE2"));
            defaultTheme.PaletteLight.Error.Should().Be(new MudColor(Colors.Red.Default));
            defaultTheme.PaletteLight.White.Should().Be(new MudColor(Colors.Shades.White));
        }

        [Test]
        public async Task WatchSystemTest()
        {
            var systemMockValue = false;
            Task SystemChangedResult(bool newValue)
            {
                systemMockValue = newValue;
                return Task.CompletedTask;
            }
            var comp = Context.RenderComponent<MudThemeProvider>();
            await comp.Instance.WatchSystemPreference(SystemChangedResult);
            await comp.Instance.SystemPreferenceChanged(true);
            systemMockValue.Should().BeTrue();
        }

        [Test]
        [TestCase("")]
        [TestCase("root")]
        [TestCase("host")]
        [TestCase(":root")]
        [TestCase(":host")]
        public void PseudoCssScope_Test(string scope)
        {
            var mudTheme = new MudTheme
            {
                PseudoCss = new PseudoCss
                {
                    Scope = scope
                }
            };
            var comp = Context.RenderComponent<MudThemeProvider>(parameters => parameters.Add(p => p.Theme, mudTheme));
            comp.Should().NotBeNull();

            var styleNodes = comp.Nodes.OfType<IHtmlStyleElement>().ToArray();

            var rootStyleNode = styleNodes[2];

            var styleLines = rootStyleNode.InnerHtml.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrEmpty(scope))
                scope = ":root";
            if (!scope.StartsWith(':'))
                scope = $":{scope}";
            styleLines.Should().Contain($"{scope}{{");
        }

        [Test]
        public void PseudoCssRootColor_Test()
        {
            const string Scope = ":root";
            var mudTheme = new MudTheme
            {
                PaletteDark = new PaletteDark
                {
                    Primary = Colors.Green.Darken1,
                },
                PseudoCss = new PseudoCss
                {
                    Scope = Scope
                }
            };
            var comp = Context.RenderComponent<MudThemeProvider>(
                parameters =>
                    parameters.Add(p => p.Theme, mudTheme)
                        .Add(p => p.IsDarkMode, true)
            );
            comp.Should().NotBeNull();

            var styleNodes = comp.Nodes.OfType<IHtmlStyleElement>().ToArray();

            var rootStyleNode = styleNodes[2];

            var styleLines = rootStyleNode.InnerHtml.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            styleLines.Should().Contain($"{Scope}{{");

            var expectedPrimaryColor = Colors.Green.Darken1;
            var expectedPrimaryColorAsRgba = new MudColor(expectedPrimaryColor).ToString(MudColorOutputFormats.RGBA);
            var expectedPrimaryLine = $"--mud-palette-primary: {expectedPrimaryColorAsRgba};";
            styleLines.Should().Contain(expectedPrimaryLine);
            var expectedPrimaryDarkenColor = new MudColor(expectedPrimaryColor).ColorRgbDarken();
            var expectedPrimaryDarkenColorAsRgb = expectedPrimaryDarkenColor.ToString(MudColorOutputFormats.RGB);
            var expectedPrimaryDarkenLine = $"--mud-palette-primary-darken: {expectedPrimaryDarkenColorAsRgb};";
            styleLines.Should().Contain(expectedPrimaryDarkenLine);
        }

        [Test]
        public async Task ObserveSystemThemeChange()
        {
            // Arrange & Act
            Context.JSInterop.SetupVoid("stopWatchingDarkThemeMedia");
            Context.JSInterop.SetupVoid("watchDarkThemeMedia");
            var themeProvider = Context.RenderComponent<ThemeProviderObserveSystemThemeChangeTest>();

            // Assert
            Context.JSInterop.VerifyNotInvoke("watchDarkThemeMedia");
            Context.JSInterop.VerifyNotInvoke("stopWatchingDarkThemeMedia");

            // Act
            await themeProvider.InvokeAsync(themeProvider.Instance.EnableObserve);

            // Assert
            Context.JSInterop.VerifyInvoke("watchDarkThemeMedia", 1);
            Context.JSInterop.VerifyNotInvoke("stopWatchingDarkThemeMedia");

            // Act
            await themeProvider.InvokeAsync(themeProvider.Instance.DisableObserve);

            // Assert
            Context.JSInterop.VerifyInvoke("watchDarkThemeMedia", 1);
            Context.JSInterop.VerifyInvoke("stopWatchingDarkThemeMedia", 1);
        }

        [Test]
        public void Dispose_ShouldInvokeJs()
        {
            // Arrange
            Context.JSInterop.SetupVoid("stopWatchingDarkThemeMedia");
            Context.RenderComponent<MudThemeProvider>();

            //Act
            Context.DisposeComponents();

            // Assert
            Context.JSInterop.VerifyInvoke("stopWatchingDarkThemeMedia");
        }

        [Test]
        public void RenderComponent_ShouldInvokeJs()
        {
            // Act & Arrange
            Context.JSInterop.SetupVoid("watchDarkThemeMedia");
            Context.RenderComponent<MudThemeProvider>();

            // Assert
            Context.JSInterop.VerifyInvoke("watchDarkThemeMedia");
        }
    }
}
