
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{

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
                "--mud-palette-black: #272c34ff;",
                "--mud-palette-white: #ffffffff;",
                "--mud-palette-primary: #594ae2ff;",
                "--mud-palette-primary-rgb: 89,74,226;",
                "--mud-palette-primary-text: #ffffffff;",
                "--mud-palette-primary-darken: rgb(62,44,221);",
                "--mud-palette-primary-lighten: rgb(118,106,231);",
                "--mud-palette-primary-hover: rgba(89,74,226,0.058823529411764705);",
                "--mud-palette-secondary: #ff4081ff;",
                "--mud-palette-secondary-rgb: 255,64,129;",
                "--mud-palette-secondary-text: #ffffffff;",
                "--mud-palette-secondary-darken: rgb(255,31,105);",
                "--mud-palette-secondary-lighten: rgb(255,102,153);",
                "--mud-palette-secondary-hover: rgba(255,64,129,0.058823529411764705);",
                "--mud-palette-tertiary: #1ec8a5ff;",
                "--mud-palette-tertiary-rgb: 30,200,165;",
                "--mud-palette-tertiary-text: #ffffffff;",
                "--mud-palette-tertiary-darken: rgb(25,169,140);",
                "--mud-palette-tertiary-lighten: rgb(42,223,187);",
                "--mud-palette-tertiary-hover: rgba(30,200,165,0.058823529411764705);",
                "--mud-palette-info: #2196f3ff;",
                "--mud-palette-info-rgb: 33,150,243;",
                "--mud-palette-info-text: #ffffffff;",
                "--mud-palette-info-darken: rgb(12,128,223);",
                "--mud-palette-info-lighten: rgb(71,167,245);",
                "--mud-palette-info-hover: rgba(33,150,243,0.058823529411764705);",
                "--mud-palette-success: #00c853ff;",
                "--mud-palette-success-rgb: 0,200,83;",
                "--mud-palette-success-text: #ffffffff;",
                "--mud-palette-success-darken: rgb(0,163,68);",
                "--mud-palette-success-lighten: rgb(0,235,98);",
                "--mud-palette-success-hover: rgba(0,200,83,0.058823529411764705);",
                "--mud-palette-warning: #ff9800ff;",
                "--mud-palette-warning-rgb: 255,152,0;",
                "--mud-palette-warning-text: #ffffffff;",
                "--mud-palette-warning-darken: rgb(214,129,0);",
                "--mud-palette-warning-lighten: rgb(255,167,36);",
                "--mud-palette-warning-hover: rgba(255,152,0,0.058823529411764705);",
                "--mud-palette-error: #f44336ff;",
                "--mud-palette-error-rgb: 244,67,54;",
                "--mud-palette-error-text: #ffffffff;",
                "--mud-palette-error-darken: rgb(242,28,13);",
                "--mud-palette-error-lighten: rgb(246,96,85);",
                "--mud-palette-error-hover: rgba(244,67,54,0.058823529411764705);",
                "--mud-palette-dark: #424242ff;",
                "--mud-palette-dark-rgb: 66,66,66;",
                "--mud-palette-dark-text: #ffffffff;",
                "--mud-palette-dark-darken: rgb(46,46,46);",
                "--mud-palette-dark-lighten: rgb(87,87,87);",
                "--mud-palette-dark-hover: rgba(66,66,66,0.058823529411764705);",
                "--mud-palette-text-primary: #424242ff;",
                "--mud-palette-text-secondary: #00000089;",
                "--mud-palette-text-disabled: #00000060;",
                "--mud-palette-action-default: #00000089;",
                "--mud-palette-action-default-hover: rgba(0,0,0,0.058823529411764705);",
                "--mud-palette-action-disabled: #00000042;",
                "--mud-palette-action-disabled-background: #0000001e;",
                "--mud-palette-surface: #ffffffff;",
                "--mud-palette-background: #ffffffff;",
                "--mud-palette-background-grey: #f5f5f5ff;",
                "--mud-palette-drawer-background: #ffffffff;",
                "--mud-palette-drawer-text: #424242ff;",
                "--mud-palette-drawer-icon: #616161ff;",
                "--mud-palette-appbar-background: #594ae2ff;",
                "--mud-palette-appbar-text: #ffffffff;",
                "--mud-palette-lines-default: #0000001e;",
                "--mud-palette-lines-inputs: #bdbdbdff;",
                "--mud-palette-table-lines: #e0e0e0ff;",
                "--mud-palette-table-striped: #00000005;",
                "--mud-palette-table-hover: #0000000a;",
                "--mud-palette-divider: #e0e0e0ff;",
                "--mud-palette-divider-light: #000000cc;",
                "--mud-palette-grey-default: #9E9E9E;",
                "--mud-palette-grey-light: #BDBDBD;",
                "--mud-palette-grey-lighter: #E0E0E0;",
                "--mud-palette-grey-dark: #757575;",
                "--mud-palette-grey-darker: #616161;",
                "--mud-palette-overlay-dark: rgba(33,33,33,0.4980392156862745);",
                "--mud-palette-overlay-light: rgba(255,255,255,0.4980392156862745);",
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
            var comp = Context.RenderComponent<MudThemeProvider>();
            comp.Should().NotBeNull();
#pragma warning disable BL0005
            comp.Instance.IsDarkMode = true;
            comp.Instance._isDarkMode.Should().BeTrue();
        }

        [Test]
        public void CustomThemeDarkModeTest()
        {
            var myCustomTheme = new MudTheme()
            {
                PaletteDark = new PaletteDark()
                {
                    Primary = Colors.Blue.Lighten1,
                    Secondary = "#F50057"
                }
            };
            Assert.AreEqual(new MudColor(Colors.Blue.Lighten1), myCustomTheme.PaletteDark.Primary);// Set by user
            Assert.AreEqual(new MudColor("#f64e62"), myCustomTheme.PaletteDark.Error);// Default dark overwritten from light
            Assert.AreEqual(new MudColor(Colors.Shades.White), myCustomTheme.PaletteDark.White);// Equal in dark and light.
            Assert.AreEqual(new MudColor("#F50057"), myCustomTheme.PaletteDark.Secondary);// Setting not in PaletteDark()
        }

        [Test]
        public void CustomThemeDarkModeBackwardsCompatibleTest()
        {
            // ensure it is backwards compatible by setting Palette() instead of PaletteDark()
            var myCustomTheme = new MudTheme()
            {
                PaletteDark = new Palette()
                {
                    Primary = Colors.Blue.Lighten1,
                    Secondary = "#F50057"
                }
            };
            Assert.AreEqual(new MudColor(Colors.Blue.Lighten1), myCustomTheme.PaletteDark.Primary);// Set by user
            Assert.AreEqual(new MudColor(Colors.Red.Default), myCustomTheme.PaletteDark.Error);// Default from light not overwritten by dark theme 
            Assert.AreEqual(new MudColor(Colors.Shades.White), myCustomTheme.PaletteDark.White);// Equal in dark and light.
            Assert.AreEqual(new MudColor("#F50057"), myCustomTheme.PaletteDark.Secondary);// Setting not in PaletteDark()
        }

        [Test]
        public void CustomThemeDefaultTest()
        {
            var DefaultTheme = new MudTheme();

            //Dark theme
            Assert.IsInstanceOf(typeof(PaletteDark), DefaultTheme.PaletteDark);
            Assert.AreEqual(new MudColor("#776be7"), DefaultTheme.PaletteDark.Primary);
            Assert.AreEqual(new MudColor("#f64e62"), DefaultTheme.PaletteDark.Error);
            Assert.AreEqual(new MudColor(Colors.Shades.White), DefaultTheme.PaletteDark.White);

            //Light theme
            Assert.IsInstanceOf(typeof(Palette), DefaultTheme.Palette);
            Assert.AreEqual(new MudColor("#594AE2"), DefaultTheme.Palette.Primary);
            Assert.AreEqual(new MudColor(Colors.Red.Default), DefaultTheme.Palette.Error);
            Assert.AreEqual(new MudColor(Colors.Shades.White), DefaultTheme.Palette.White);
        }

        private bool _systemMockValue;
        private async Task SystemChangedResult(bool newValue)
        {
            _systemMockValue = newValue;
        }
        [Test]
        public async Task WatchSystemTest()
        {
            Assert.IsFalse(_systemMockValue);
            var comp = Context.RenderComponent<MudThemeProvider>();
            await comp.Instance.WatchSystemPreference(SystemChangedResult);
            await comp.Instance.SystemPreferenceChanged(true);
            Assert.IsTrue(_systemMockValue);
        }

        [Test]
        [TestCase("")]
        [TestCase("root")]
        [TestCase("host")]
        [TestCase(":root")]
        [TestCase(":host")]
        public void PseudoCssScope_Test(string scope)
        {
            var mudTheme = new MudTheme();
            mudTheme.PseudoCss = new PseudoCss()
            {
                Scope = scope
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
    }
}
