using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Time.Testing;
using MudBlazor.UnitTests.TestComponents.Snackbar;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SnackbarTests : BunitTest
    {
        private IRenderedComponent<MudSnackbarProvider> _provider;
        private ISnackbar _service;
        private FakeTimeProvider _timeProvider;

        [SetUp]
        public void SnackbarSetUp()
        {
            _timeProvider = Context.AddFakeTimeProvider();
            _service = Context.Services.GetService<ISnackbar>();
            _provider = Context.Render<MudSnackbarProvider>();
            _provider.Find("#mud-snackbar-container").InnerHtml.Trimmed().Should().BeEmpty();
        }

        [TearDown]
        public async Task SnackbarTearDown()
        {
            await _provider.InvokeAsync(() => _service.Clear());
            _service.ShownSnackbars.Should().BeEmpty();
        }

        private Task AdvanceTimeAsync(int milliseconds)
        {
            return AdvanceTimeAsync(TimeSpan.FromMilliseconds(milliseconds));
        }

        private Task AdvanceTimeAsync(TimeSpan time)
        {
            return _provider.InvokeAsync(() => _timeProvider.Advance(time));
        }

        [Test]
        public async Task Simple()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!"));
            _provider.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be("Boom, big reveal. Im a pickle!");
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
        public async Task TestWithRenderFragmentLiteral()
        {
            var testComponent = Context.Render<SnackbarRenderFragmentMessageTest>();

            await testComponent.Find("button").ClickAsync();
            await _provider.WaitForAssertionAsync(() =>
                _provider.Find("div.mud-snackbar-content-message").Should().NotBe(null)
            );
            _provider.Find("div.mud-snackbar-content-message").TrimmedText().Replace(" ", "").Should().Be("Here'saregularitem\nHere'sabolditem\nHere'sanitalicizeditem");
        }

        [Test]
        public async Task TestWithCustomComponent()
        {
            var testComponent = Context.Render<SnackbarCustomComponentMessageTest>();

            await testComponent.Find("button").ClickAsync();
            await _provider.WaitForAssertionAsync(() =>
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
        [TestCase(Severity.Normal, Icons.Material.Outlined.EventNote)]
        [TestCase(Severity.Info, Icons.Material.Outlined.Info)]
        [TestCase(Severity.Success, Icons.Custom.Uncategorized.AlertSuccess)]
        [TestCase(Severity.Warning, Icons.Material.Outlined.ReportProblem)]
        [TestCase(Severity.Error, Icons.Material.Filled.ErrorOutline)]
        public async Task DefaultIconMatchesSeverity(Severity severity, string expectedIcon)
        {
            Snackbar bar = null;
            await _provider.InvokeAsync(() => bar = _service.Add("Boom, big reveal. Im a pickle!", severity));

            bar.Severity.Should().Be(severity);
            _provider.FindComponents<MudIcon>().Select(i => i.Instance.Icon).Should().Contain(expectedIcon);
        }

        [Test]
        public async Task CustomIconOverridesSeverityDefault()
        {
            var customIcon = Icons.Material.Filled.Star;
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Error, c => c.Icon = customIcon));

            _provider.FindComponents<MudIcon>().Select(i => i.Instance.Icon).Should().Contain(customIcon);
        }

        [Test]
        public async Task GlobalSeverityIconOverrideIsApplied()
        {
            // A severity icon and size set on the global configuration should flow through to the rendered icon.
            var customErrorIcon = Icons.Material.Filled.ReportGmailerrorred;
            _service.Configuration.ErrorIcon = customErrorIcon;
            _service.Configuration.IconSize = Size.Large;

            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Error));

            var icon = _provider.FindComponents<MudIcon>().Single(i => i.Instance.Icon == customErrorIcon);
            icon.Instance.Size.Should().Be(Size.Large);
        }

        [Test]
        [TestCase(false, null, true)]  // Default: the icon is shown.
        [TestCase(true, null, false)]  // Hidden globally.
        [TestCase(true, false, true)]  // Re-enabled per snackbar.
        [TestCase(false, true, false)] // Hidden per snackbar.
        public async Task IconVisibilityRespectsGlobalAndPerSnackbarSettings(bool globalHideIcon, bool? perSnackbarHideIcon, bool expectIcon)
        {
            _service.Configuration.HideIcon = globalHideIcon;

            await _provider.InvokeAsync(() => _service.Add("Hello world", Severity.Success, opts =>
            {
                if (perSnackbarHideIcon.HasValue)
                {
                    opts.HideIcon = perSnackbarHideIcon.Value;
                }
            }));

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(1));
            _provider.FindAll(".mud-snackbar-icon").Count.Should().Be(expectIcon ? 1 : 0);
        }

        [Test]
        public async Task CustomIconColorAndSize()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Success, config => { config.IconColor = Color.Tertiary; config.IconSize = Size.Large; }));

            // The severity icon is the first MudIcon; the close button renders a separate one.
            var icon = _provider.FindComponent<MudIcon>().Instance;
            icon.Color.Should().Be(Color.Tertiary);
            icon.Size.Should().Be(Size.Large);
        }

        [Test]
        public async Task CustomIconDefaultValues()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!", Severity.Success));

            var icon = _provider.FindComponent<MudIcon>().Instance;
            icon.Color.Should().Be(Color.Inherit);
            icon.Size.Should().Be(Size.Medium);
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
        public async Task NonFilledVariantWithoutBlurUsesSurfaceClass()
        {
            // A non-filled variant without blur falls back to the surface background.
            await _provider.InvokeAsync(() =>
                _service.Add("Boom, big reveal. Im a pickle!", Severity.Success, c =>
                {
                    c.SnackbarVariant = Variant.Outlined;
                    c.BackgroundBlurred = false;
                })
            );

            var snackbarClassList = _provider.Find(".mud-snackbar").ClassList;
            snackbarClassList.Should().Contain("mud-snackbar-surface");
            snackbarClassList.Should().NotContain("mud-snackbar-blurred");
        }

        [Test]
        public void AddWithInvalidSeverityThrows()
        {
            var act = () => _service.Add("Boom, big reveal. Im a pickle!", (Severity)int.MaxValue);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Test]
        [TestCase(Defaults.Classes.Position.TopStart, false, Defaults.Classes.Position.TopLeft)]
        [TestCase(Defaults.Classes.Position.TopStart, true, Defaults.Classes.Position.TopRight)]
        [TestCase(Defaults.Classes.Position.TopEnd, false, Defaults.Classes.Position.TopRight)]
        [TestCase(Defaults.Classes.Position.TopEnd, true, Defaults.Classes.Position.TopLeft)]
        [TestCase(Defaults.Classes.Position.BottomStart, false, Defaults.Classes.Position.BottomLeft)]
        [TestCase(Defaults.Classes.Position.BottomStart, true, Defaults.Classes.Position.BottomRight)]
        [TestCase(Defaults.Classes.Position.BottomEnd, false, Defaults.Classes.Position.BottomRight)]
        [TestCase(Defaults.Classes.Position.BottomEnd, true, Defaults.Classes.Position.BottomLeft)]
        [TestCase(Defaults.Classes.Position.TopRight, true, Defaults.Classes.Position.TopRight)] // Fixed positions are unaffected by direction.
        public void PositionClassResolvesStartAndEndForRightToLeft(string positionClass, bool rightToLeft, string expectedClass)
        {
            _service.Configuration.PositionClass = positionClass;

            var provider = Context.Render<MudSnackbarProvider>(ps => ps.AddCascadingValue("RightToLeft", rightToLeft));

            provider.Find("#mud-snackbar-container").ClassList.Should().Contain(expectedClass);
        }

        [Test]
        [TestCase(false, new[] { "First", "Second" })]
        [TestCase(true, new[] { "Second", "First" })]
        public async Task NewestOnTopControlsDisplayOrder(bool newestOnTop, string[] expectedOrder)
        {
            _service.Configuration.NewestOnTop = newestOnTop;

            await _provider.InvokeAsync(() =>
            {
                _service.Add("First");
                _service.Add("Second");
            });

            var messages = _provider.FindAll("div.mud-snackbar-content-message").Select(e => e.TrimmedText());
            messages.Should().Equal(expectedOrder);
        }

        [Test]
        public async Task MaxDisplayedSnackbarsLimitsWhatIsShown()
        {
            _service.Configuration.MaxDisplayedSnackbars = 2;
            var allowDuplicates = (SnackbarOptions o) => { o.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow; };

            await _provider.InvokeAsync(() =>
            {
                _service.Add("Message 1", configure: allowDuplicates);
                _service.Add("Message 2", configure: allowDuplicates);
                _service.Add("Message 3", configure: allowDuplicates);
            });

            _service.ShownSnackbars.Count().Should().Be(2);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(2);
        }

        [Test]
        [TestCase("Undo")] // An action button still renders even with the close icon hidden.
        [TestCase(null)]   // With neither a close icon nor an action, the action area is omitted entirely.
        public async Task ShowCloseIconFalseHidesTheCloseButton(string action)
        {
            await _provider.InvokeAsync(() =>
                _service.Add("Boom, big reveal. Im a pickle!", Severity.Normal, c =>
                {
                    c.ShowCloseIcon = false;
                    c.Action = action;
                })
            );

            _provider.FindAll(".mud-snackbar-close-button").Should().BeEmpty();
            _provider.FindAll(".mud-snackbar-action-button").Count.Should().Be(action is null ? 0 : 1);
            if (action is null)
            {
                _provider.FindAll(".mud-snackbar-content-action").Should().BeEmpty();
            }
        }

        [Test]
        public async Task ClickingBodyWithoutOnClickIsIgnored()
        {
            Snackbar primary = null;
            await _provider.InvokeAsync(() =>
                primary = _service.Add("Boom, big reveal. Im a pickle!", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.VisibleStateDuration = int.MaxValue;
                    c.HideTransitionDuration = int.MaxValue;
                })
            );
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // No OnClick is configured, so clicking the body must not begin the hide transition.
            await _provider.Find(".mud-snackbar").ClickAsync();

            // Still Visible (not Hiding); a count assertion alone cannot tell these apart here.
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
        }

        [Test]
        public async Task ShowTransitionCompletesNaturallyViaTimer()
        {
            // Exercises the full timer-driven lifecycle without interrupting any transition.
            Snackbar primary = null;
            await _provider.InvokeAsync(() =>
                primary = _service.Add("Boom, big reveal. Im a pickle!", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 100;
                    c.VisibleStateDuration = 100;
                    c.HideTransitionDuration = 0;
                })
            );

            primary.State.SnackbarState.Should().Be(SnackbarState.Showing);

            await AdvanceTimeAsync(100); // Show transition elapses on its own.
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            await AdvanceTimeAsync(100); // Visible duration elapses; hide is instant.
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task DisposingSnackbarDirectlyDoesNotRemoveItFromService()
        {
            // Disposing the Snackbar instance only stops its timer; the service still owns it until removed.
            Snackbar snackbar = null;
            await _provider.InvokeAsync(() => snackbar = _service.Add("Boom, big reveal. Im a pickle!"));

            snackbar.Dispose();

            _service.ShownSnackbars.Should().ContainSingle().Which.Should().Be(snackbar);
        }

        [Test]
        public async Task DisposingServiceRemovesAllSnackbars()
        {
            await _provider.InvokeAsync(() => _service.Add("Boom, big reveal. Im a pickle!"));
            _service.ShownSnackbars.Should().NotBeEmpty();

            _service.Dispose();

            _service.ShownSnackbars.Should().BeEmpty();
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

            await _provider.Find(".mud-snackbar-close-button").ClickAsync();

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

            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration * 2);

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test resume.

            primary.PauseTransitions(false);
            await AdvanceTimeAsync(primary.State.Options.HideTransitionDuration);

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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
                    c.OnClick = _ => Task.CompletedTask;
                })
            );
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that clicking the snackbar will trigger onclick to close despite pointer over and touch start pausing it.

            _provider.Find(".mud-snackbar").TouchStart();
            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            await _provider.Find(".mud-snackbar").ClickAsync();

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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

            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            await _provider.FindAll(".mud-snackbar-close-button").Single().ClickAsync();

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task CloseButtonInvokesCustomTask()
        {
            var counter = 0;
            Task Count(Snackbar s)
            {
                counter++;
                return Task.CompletedTask;
            }
            // Set up the snackbar.
            await _provider.InvokeAsync(() =>
                _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.CloseButtonClickFunc = Count;
                    c.RequireInteraction = true;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            counter.Should().Be(0);

            await _provider.FindAll(".mud-snackbar-close-button").Single().ClickAsync();

            counter.Should().Be(1);
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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
                    c.OnClick = _ => Task.CompletedTask;
                })
            );

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Test that clicking the action button will actually close the snackbar even with the pointer over.

            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            await _provider.Find(".mud-snackbar-action-button").ClickAsync();

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task ActionRequiresInteractionByDefault()
        {
            Snackbar snackbar = null;

            await _provider.InvokeAsync(() =>
                snackbar = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = 10;
                    c.Action = "Close";
                    c.OnClick = _ => Task.CompletedTask;
                })
            );

            snackbar.Should().NotBeNull();
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            await AdvanceTimeAsync(200);

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            await _provider.Find(".mud-snackbar-action-button").ClickAsync();

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
        }

        [Test]
        public async Task ActionAllowsAutoDismissWhenDisabled()
        {
            Snackbar snackbar = null;

            await _provider.InvokeAsync(() =>
                snackbar = _service.Add("ah, ah, ah, ah, stayin' alive", Severity.Normal, c =>
                {
                    c.ShowTransitionDuration = 0;
                    c.HideTransitionDuration = 0;
                    c.VisibleStateDuration = 0;
                    c.Action = "Close";
                    c.RequireInteraction = false;
                })
            );

            snackbar.Should().NotBeNull();
            snackbar.State.Options.RequiresInteraction.Should().BeFalse();

            await _provider.WaitForAssertionAsync(() => _service.ShownSnackbars.Should().BeEmpty());
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

            await _provider.Find(".mud-snackbar-close-button").ClickAsync();
            _provider.Find(".mud-snackbar").TouchStart();
            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());

            await AdvanceTimeAsync(100);
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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

            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());

            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration * 2);

            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            await _provider.Find(".mud-snackbar").PointerLeaveAsync(new PointerEventArgs());
            await AdvanceTimeAsync(primary.State.Options.HideTransitionDuration);

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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

            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration * 2);

            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            _provider.Find(".mud-snackbar").TouchEnd();
            await AdvanceTimeAsync(primary.State.Options.HideTransitionDuration);

            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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
            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Pointer is still over and the state should still be visible.
            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration * 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // Leave pointer and let the hide transition that's been pending start.
            await _provider.Find(".mud-snackbar").PointerLeaveAsync(new PointerEventArgs());
            await AdvanceTimeAsync(primary.State.Options.HideTransitionDuration / 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Hiding);

            // Re-enter halfway through hide transition.
            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Finally make the pointer leave and let it hide.
            await _provider.Find(".mud-snackbar").PointerLeaveAsync(new PointerEventArgs());
            await AdvanceTimeAsync(primary.State.Options.HideTransitionDuration);
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));
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
            _provider.Find(".mud-snackbar").PointerEnter(new PointerEventArgs());
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);

            // Ensure that leaving with the pointer does not trigger a hide transition by itself, like if the timer was not properly utilized.

            _provider.Find(".mud-snackbar").PointerLeave(new PointerEventArgs());
            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration / 2);
            primary.State.SnackbarState.Should().Be(SnackbarState.Visible);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(1);

            // The snackbar should naturally leave the visibility state after the configured duration.
            await AdvanceTimeAsync(primary.State.Options.VisibleStateDuration);
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

            await AdvanceTimeAsync(60); // 60% through the visible duration.
            await _provider.Find(".mud-snackbar").PointerEnterAsync(new PointerEventArgs());
            await _provider.Find(".mud-snackbar").PointerLeaveAsync(new PointerEventArgs());
            _provider.Find(".mud-snackbar").TouchStart();
            _provider.Find(".mud-snackbar").TouchEnd();

            // It should close after the original remaining duration; if the duration was reset this assertion will fail.
            await AdvanceTimeAsync(40);
            _provider.FindAll(".mud-snackbar").Count.Should().Be(0);
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
                    c.OnClick = _ =>
                    {
                        successfulClicks++;
                        return Task.CompletedTask;
                    };
                })
            );

            await _provider.Find(".mud-snackbar-action-button").ClickAsync();
            clickAttempts++;

            await _provider.Find(".mud-snackbar-action-button").ClickAsync();
            clickAttempts++;

            await AdvanceTimeAsync(300);
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));

            successfulClicks.Should().Be(1);
            clickAttempts.Should().Be(2);
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
                    c.OnClick = _ =>
                    {
                        successfulClicks++;
                        return Task.CompletedTask;
                    };
                })
            );

            await _provider.Find(".mud-snackbar").ClickAsync();
            clickAttempts++;

            await _provider.Find(".mud-snackbar").ClickAsync();
            clickAttempts++;

            await AdvanceTimeAsync(300);
            await _provider.WaitForAssertionAsync(() => _provider.FindAll(".mud-snackbar").Count.Should().Be(0));

            successfulClicks.Should().Be(1);
            clickAttempts.Should().Be(2);
        }

    }
}
