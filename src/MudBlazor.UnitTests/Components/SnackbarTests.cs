
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SnackbarTests : BunitTest
    {
        private IRenderedComponent<MudSnackbarProvider> _provider;
        private ISnackbar _service;

        [SetUp]
        public void SnackbarSetUp()
        {
            _service = Context.Services.GetService<ISnackbar>();
            _provider = Context.RenderComponent<MudSnackbarProvider>();
            _provider.Find("#mud-snackbar-container").InnerHtml.Trimmed().Should().BeEmpty();
        }

        [TearDown]
        public void SnackbarTearDown()
        {
            // Force close all snackbars directly from their class.
            // We used to simulate clicking the close button but this is quicker because it skips transitions.
            // We keep checking instead of using a cached list because new snackbars could be spawned from the close event.
            while (_service.ShownSnackbars.Any())
            {
                _service.ShownSnackbars.First().ForceClose();
            }

            _provider.WaitForAssertion(() => _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty(), TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task SimpleTest()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!"));
            _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be("Boom, big reveal. Im a pickle!");
        }

        [Test]
        public async Task SimpleTestWithRenderFragment()
        {
            var testText = "Boom, big reveal. Im a pickle!";
            var renderFragment = new RenderFragment(builder =>
            {
                builder.AddContent(0, testText);
            });

            await _provider.InvokeAsync(() => _service.Add(renderFragment));
            _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be(testText);
        }

        [Test]
        public async Task SimpleTestWithHtmlInMessageMarkupString()
        {
            await _provider.InvokeAsync(() => _service.Add(new MarkupString("Hello <span>World</span>")));
            var messageText = _provider.Find("div.mud-snackbar-content-message").InnerHtml.Trim();
            messageText.Should().Be("Hello <span>World</span>");
        }

        [Test]
        public async Task HtmlInMessageStringShouldBeEncoded()
        {
            await _provider.InvokeAsync(() => _service.Add("Hello <span>World</span>"));
            var messageText = _provider.Find("div.mud-snackbar-content-message").InnerHtml.Trim();
            messageText.Should().Be("Hello &lt;span&gt;World&lt;/span&gt;");
        }

        [Test]
        public async Task TestWithHierarchicalRenderFragment()
        {
            var testText = "Boom, big reveal. Im a pickle!";
            var renderFragment = new RenderFragment(builder =>
            {
                builder.OpenElement(0, "ul");
                builder.OpenElement(1, "li");
                builder.AddContent(2, testText);
                builder.CloseElement();
                builder.CloseElement();
            });
            // shoot out a snackbar
            await _provider.InvokeAsync(() => _service.Add(renderFragment));
            _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            _provider.Find("div.mud-snackbar-content-message").InnerHtml.Should().Be($"<ul><li>{testText}</li></ul>");
        }

        [Test]
        public void TestWithRenderFragmentLiteral()
        {
            var testComponent = Context.RenderComponent<SnackbarRenderFragmentMessageTest>();

            testComponent.Find("button").Click();
            _provider.WaitForAssertion(() =>
                _provider.Find("div.mud-snackbar-content-message").Should().NotBe(null)
            );
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Replace(" ", "").Should().Be("Here'saregularitem\nHere'sabolditem\nHere'sanitalicizeditem");
        }

        [Test]
        public void TestWithCustomComponent()
        {
            var testComponent = Context.RenderComponent<SnackbarCustomComponentMessageTest>();

            testComponent.Find("button").Click();
            _provider.WaitForAssertion(() =>
                _provider.Find("div.mud-snackbar-content-message").Should().NotBe(null)
            );
            _provider.Find("div.mud-snackbar-content-message .mud-chip").Should().NotBe(null);
        }

        [Test]
        public void TestEmptyStringIsIgnored()
        {
            var bar = _service.Add("");
            bar.Should().BeNull();
            _service.ShownSnackbars.Count().Should().Be(0);
        }

        [Test]
        public void TestEmptyMarkupStringIsIgnored()
        {
            var bar = _service.Add(new MarkupString(""));
            bar.Should().BeNull();
            _service.ShownSnackbars.Count().Should().Be(0);
        }

        [Test]
        public void TestStringMessageShouldAutofillKey()
        {
            var bar = _service.Add("Oh no!");
            bar.Message.Should().Be("Oh no!");
            bar.SnackbarMessage.Key.Should().Be("Oh no!");
        }

        [Test]
        public void TestStringMessageWithDifferentKey()
        {
            var bar = _service.Add("Oh no!", key: "zzz");
            bar.Message.Should().Be("Oh no!");
            bar.SnackbarMessage.Key.Should().Be("zzz");
        }

        [Test]
        public void TestKeyPreventsDuplication()
        {
            var key = "This is the key";

            _service.Add("A string message", key: key);
            _service.Add(key); // Test leaving key default
            _service.Add(new MarkupString("A <b>markupstring</b> message"), key: key);
            _service.Add(new MarkupString(key)); // Test leaving key default
            _service.Add(new RenderFragment(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, "A renderfragment message");
                builder.CloseElement();

            }), Severity.Normal, key: key);
            _service.Add<SnackbarCustomComponent>(null, key: key);

            _service.ShownSnackbars.Count().Should().Be(1);
        }

        [Test]
        public void TestPerBarPreventDuplicatesWorks()
        {
            var key = "This is the key";
            var config = (SnackbarOptions opts) =>
            {
                opts.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
            };
            _service.Configuration.PreventDuplicates = false;

            _service.Add("Message 1", configure: config, key: key);
            _service.Add("Message 2", configure: config, key: key);

            _service.ShownSnackbars.Count().Should().Be(1);
        }

        [Test]
        public void TestPerBarAllowDuplicatesWorks()
        {
            var key = "This is the key";
            var config = (SnackbarOptions opts) =>
            {
                opts.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow;
            };
            _service.Configuration.PreventDuplicates = true;

            _service.Add("Message 1", configure: config, key: key);
            _service.Add("Message 2", configure: config, key: key);

            _service.ShownSnackbars.Count().Should().Be(2);
        }

        [Test]
        public void PerBarGlobalDefaultFallsThroughToGlobalTrue()
        {
            var key = "This is the key";
            var config = (SnackbarOptions opts) =>
            {
                opts.DuplicatesBehavior = SnackbarDuplicatesBehavior.GlobalDefault;
            };
            _service.Configuration.PreventDuplicates = true;

            _service.Add("Message 1", configure: config, key: key);
            _service.Add("Message 2", configure: config, key: key);

            _service.ShownSnackbars.Count().Should().Be(1);
        }

        [Test]
        public void PerBarGlobalDefaultFallsThroughToGlobalFalse()
        {
            var key = "This is the key";
            var config = (SnackbarOptions opts) =>
            {
                opts.DuplicatesBehavior = SnackbarDuplicatesBehavior.GlobalDefault;
            };
            _service.Configuration.PreventDuplicates = false;

            _service.Add("Message 1", configure: config, key: key);
            _service.Add("Message 2", configure: config, key: key);

            _service.ShownSnackbars.Count().Should().Be(2);
        }

        [Test]
        public async Task IconTest()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!"));
            _provider.Find("#mud-snackbar-container .mud-snackbar .mud-snackbar-icon").InnerHtml.Trim().Should().NotBeEmpty();
        }

        [Test]
        public async Task HideIconTest()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Success, config => { config.HideIcon = true; }));
            var hasIcon = _provider.Find("#mud-snackbar-container .mud-snackbar").FirstElementChild.ClassName.Contains("mud-snackbar-icon");
            hasIcon.Should().BeFalse();
        }

        [Test]
        public async Task CustomIconTest()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Success, config => { config.IconColor = Color.Tertiary; config.IconSize = Size.Large; }));

            var svgClassNames = _provider.Find("#mud-snackbar-container .mud-snackbar").FirstElementChild.FirstElementChild.ClassName;
            svgClassNames.Should().Contain("mud-icon-size-large");
            svgClassNames.Should().Contain("mud-tertiary-text");
        }

        [Test]
        public async Task CustomIconDefaultValuesTest()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Success));

            var svgClassNames = _provider.Find("#mud-snackbar-container .mud-snackbar").FirstElementChild.FirstElementChild.ClassName;
            svgClassNames.Should().Contain("mud-icon-size-medium");

            // Ensure no color classes are present, like "mud-primary-text", "mud-error-text", etc.
            var classNames = svgClassNames.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var className in classNames)
                Regex.IsMatch(className, "^mud-[a-z]+-text$", RegexOptions.IgnoreCase).Should().BeFalse();
        }

        [Test]
        public async Task PerSnackbarClassTypes()
        {
            // https://github.com/MudBlazor/MudBlazor/issues/5027.

            await _provider.InvokeAsync(() =>
                _service.Add("Boom, big reveal. Im a pickle!",
                    Severity.Success,
                    c =>
                    {
                        // Non-default settings.
                        c.SnackbarVariant = Variant.Outlined;
                        c.BackgroundBlurred = true;
                    }
                )
            );

            var snackbarClassList = _provider.Find(".mud-snackbar").ClassList;
            snackbarClassList.Should().Contain("mud-snackbar-blurred");
            snackbarClassList.Should().Contain("mud-alert-outlined-success");
        }

        [Test]
        public async Task DisposeTest()
        {
            // shoot out a snackbar
            Snackbar snackbar = null;
            await _provider.InvokeAsync(() => snackbar = _service.Add("Boom, big reveal. Im a pickle!"));

            snackbar?.Dispose();

            _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be("Boom, big reveal. Im a pickle!");
        }

        [Test]
        public async Task TestSnackBarRemoveByKey()
        {
            const string TestText = "Boom, big reveal. Im a pickle!";
            const string Key = "c8916cd2-dcbb-41b5-9125-cceafa4354ba";

            var config = (SnackbarOptions options) =>
            {
                options.VisibleStateDuration = int.MaxValue;
                options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow;
            };

            await _provider.InvokeAsync(() => _service.Add(TestText, Severity.Normal, config, Key));
            await _provider.InvokeAsync(() => _service.Add(TestText, Severity.Normal, config, Key));
            //Without key to make sure it doesn't gets removed.
            await _provider.InvokeAsync(() => _service.Add(TestText, Severity.Normal, config));

            _service.ShownSnackbars.Count().Should().Be(3);

            await _provider.InvokeAsync(() => _service.RemoveByKey(Key));

            _service.ShownSnackbars.Count().Should().Be(1);
        }

        [Test]
        public async Task ForceCloseSkipsTransition()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("Bye Felicia", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = int.MaxValue;
                    c.VisibleStateDuration = int.MaxValue;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            _provider.Find(".mud-snackbar-close-button").Click();

            // Test that the hide transition from clicking the close button will be forcibly ended, skipping the max value duration.
            primary.ForceClose();

            _provider.FindAll(".mud-snackbar").Count.Should().Be(0);
        }

        [Test]
        public async Task PauseTransitionsManually()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 100;
                    c.VisibleStateDuration = 100;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test pause.

            primary.PauseTransitions(true);

            await Task.Delay(primary.State.Options.VisibleStateDuration * 2);

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test resume.

            primary.PauseTransitions(false);

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task OnClickClosesWithPointerOver()
        {
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = int.MaxValue;
                    c.Onclick = _ => Task.CompletedTask;
                })
            );
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that clicking the snackbar will trigger onclick to close despite pointer over and touch start pausing it.

            _provider.Find(".mud-snackbar").TouchStart();
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            _provider.Find(".mud-snackbar").Click();

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task CloseButtonClosesWithPointerOver()
        {
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = int.MaxValue;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that clicking the close button will actually close the snackbar even with the pointer over.

            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            _provider.FindAll(".mud-snackbar-close-button").Single().Click();

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task ActionButtonClosesWithPointerOver()
        {
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = int.MaxValue;
                    c.Action = "Close";
                    c.Onclick = _ => Task.CompletedTask;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that clicking the action button will actually close the snackbar even with the pointer over.

            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            _provider.Find(".mud-snackbar-action-button").Click();

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task CannotStopCloseTransition()
        {
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 100;
                    c.VisibleStateDuration = int.MaxValue;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that the hide transition from clicking the close button cannot be stopped by hovering back over the snackbar.

            _provider.Find(".mud-snackbar-close-button").Click();
            _provider.Find(".mud-snackbar").TouchStart();
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task StayVisibleWithPointer()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 100;
                    c.VisibleStateDuration = 100;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that the snackbar will stay visible.

            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());

            await Task.Delay(primary.State.Options.VisibleStateDuration * 2);

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            _provider.Find(".mud-snackbar").TriggerEvent("onpointerleave", new PointerEventArgs());

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task StayVisibleWithTouch()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 100;
                    c.VisibleStateDuration = 100;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that the snackbar will stay visible.

            _provider.Find(".mud-snackbar").TouchStart();

            await Task.Delay(primary.State.Options.VisibleStateDuration * 2);

            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            _provider.Find(".mud-snackbar").TouchEnd();

            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task InterruptTransitions()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = int.MaxValue;
                    c.VisibleStateDuration = 50;
                    c.HideTransitionDuration = 100;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Interrupting show transition should instantly go to visible state.

            primary.State.SnackbarState.Should().Be(SnackbarState.Showing);
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Pointer is still over and the state should still be visible.
            await Task.Delay(primary.State.Options.VisibleStateDuration * 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Leave pointer and let the hide transition that's been pending start.
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerleave", new PointerEventArgs());
            await Task.Delay(primary.State.Options.HideTransitionDuration / 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Hiding);

            // Re-enter halfway through hide transition.
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Finally make the pointer leave and let it hide.
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerleave", new PointerEventArgs());
            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task PointerOverDoesNotTriggerHideTransition()
        {
            // Set up the snackbar.

            Snackbar primary = null;

            await _provider.InvokeAsync(() =>
                primary = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 100;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = 100;
                })
            );

            primary.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Force it out of the show transition.

            primary.State.SnackbarState.Should().Be(SnackbarState.Showing);
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Ensure that leaving with the pointer does not trigger a hide transition by itself, like if the timer was not properly utilized.

            _provider.Find(".mud-snackbar").TriggerEvent("onpointerleave", new PointerEventArgs());
            await Task.Delay(primary.State.Options.VisibleStateDuration / 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // The snackbar should naturally leave the visibility state after the configured duration.
            await Task.Delay(primary.State.Options.VisibleStateDuration);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(0);
        }

        [Test]
        public async Task PointerOverDoesNotRestartVisibleDuration()
        {
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = 100;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Prove that the pointer entering the snackbar does not restart the duration from zero.

            await Task.Delay(60); // 60% through the visible duration.
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerenter", new PointerEventArgs());
            _provider.Find(".mud-snackbar").TriggerEvent("onpointerleave", new PointerEventArgs());
            _provider.Find(".mud-snackbar").TouchStart();
            _provider.Find(".mud-snackbar").TouchEnd();

            // It should close within another 60ms if it's behaving correctly; If the duration was reset this assertion will fail.
            _provider.WaitForAssertion(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0), TimeSpan.FromMilliseconds(60));
        }

        [Test]
        public async Task OnClickFromActionButtonOnlyOnce()
        {
            var clickAttempts = 0;
            var successfulClicks = 0;

            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("It's all just cornflakes", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 300;
                    c.VisibleStateDuration = int.MaxValue;
                    c.Action = "Click me";
                    c.Onclick = _ =>
                    {
                        successfulClicks++;
                        return Task.CompletedTask;
                    };
                })
            );

            // Click as many times as possible during the hide transition.
            while (true)
            {
                var clicked = false;

                // Click the action button if one was found.
                await _provider.InvokeAsync(() =>
                {
                    if (_provider.FindAll(".mud-snackbar-action-button").Count == 1)
                    {
                        _provider.Find(".mud-snackbar-action-button").Click();
                        clicked = true;
                    }
                });

                if (clicked)
                    clickAttempts++;
                else
                    break;
            }

            // Only one click should have been successful and multiple clicks should have been attempted.
            successfulClicks.Should().Be(1).And.BeLessThan(clickAttempts);
        }

        [Test]
        public async Task OnClickFromBodyOnlyOnce()
        {
            var clickAttempts = 0;
            var successfulClicks = 0;

            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("It's all just cornflakes", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 300;
                    c.VisibleStateDuration = int.MaxValue;
                    c.Onclick = _ =>
                    {
                        successfulClicks++;
                        return Task.CompletedTask;
                    };
                })
            );

            // Click as many times as possible during the hide transition.
            while (true)
            {
                var clicked = false;

                // Click the snackbar if one was found.
                await _provider.InvokeAsync(() =>
                {
                    if (_provider.FindAll(".mud-snackbar").Count == 1)
                    {
                        _provider.Find(".mud-snackbar").Click();
                        clicked = true;
                    }
                });

                if (clicked)
                    clickAttempts++;
                else
                    break;
            }

            // Only one click should have been successful and multiple clicks should have been attempted.
            successfulClicks.Should().Be(1).And.BeLessThan(clickAttempts);
        }
    }
}
