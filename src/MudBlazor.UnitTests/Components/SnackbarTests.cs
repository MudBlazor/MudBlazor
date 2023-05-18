
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
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
            foreach (var closeButton in _provider.FindAll("button"))
            {
                closeButton?.Click();
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
        public async Task SimpleTestWithHtmlInMessageString()
        {
            await _provider.InvokeAsync(() => _service.Add("Hello <span>World</span>"));
            var messageText = HttpUtility.HtmlDecode(_provider.Find("div.mud-snackbar-content-message").InnerHtml.Trim());
            messageText.Should().Be("Hello <span>World</span>");
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
        public void TestStringMessageShouldAutofillKey()
        {
            var bar = _service.Add("Oh no!");
            Assert.AreEqual("Oh no!", bar.Message);
            Assert.AreEqual("Oh no!", bar.SnackbarMessage.Key);
        }

        [Test]
        public void TestStringMessageWithDifferentKey()
        {
            var bar = _service.Add("Oh no!", key:"zzz");
            Assert.AreEqual("Oh no!", bar.Message);
            Assert.AreEqual("zzz", bar.SnackbarMessage.Key);
        }

        [Test]
        public void TestKeyPreventsDuplication()
        {
            var key = "This is the key";

            _service.Add("A string message", key: key);
            _service.Add(new RenderFragment(builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddContent(1, "A renderfragment message");
                builder.CloseElement();

            }), Severity.Normal, key: key);
            _service.Add<SnackbarCustomComponent>(null, key: key);

            Assert.AreEqual(1, _service.ShownSnackbars.Count());
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

            Assert.AreEqual(1, _service.ShownSnackbars.Count());
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

            Assert.AreEqual(2, _service.ShownSnackbars.Count());
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

            Assert.AreEqual(1, _service.ShownSnackbars.Count());
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

            Assert.AreEqual(2, _service.ShownSnackbars.Count());
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
            Assert.IsFalse(hasIcon);
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

            Assert.AreEqual(3, _service.ShownSnackbars.Count());

            await _provider.InvokeAsync(() => _service.RemoveByKey(Key));

            Assert.AreEqual(1, _service.ShownSnackbars.Count());
        }
    }
}
