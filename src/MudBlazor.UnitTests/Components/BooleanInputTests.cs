using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BooleanInputTests : BunitTest
    {
        [Test]
        [TestCase("tabindex", "5")]
        [TestCase("tabIndex", "7")]
        [TestCase("TABINDEX", "9")]
        public void ResolveTabIndexAndAttributes_RespectsCaseInsensitiveTabIndex(string key, string value)
        {
            var comp = Context.Render<TestBooleanInput>(p =>
                p.AddUnmatched(key, value)
            );

            var result = comp.Instance.Resolve();

            result.TabIndex.Should().Be(int.Parse(value));
            result.Attributes.Should().BeNull();
        }

        [Test]
        [TestCase("tabindex")]
        [TestCase("tabIndex")]
        [TestCase("TABINDEX")]
        public void ResolveTabIndexAndAttributes_RemovesAllTabIndexVariants(string key)
        {
            var comp = Context.Render<TestBooleanInput>(p =>
            {
                p.AddUnmatched(key, "5");
                p.AddUnmatched("other", "value");
            });

            var result = comp.Instance.Resolve();

            result.TabIndex.Should().Be(5);
            result.Attributes.Should().NotContainKey(key);
            result.Attributes.Should().ContainKey("other");
        }

        [Test]
        public void ResolveTabIndexAndAttributes_DefaultsToZero_WhenNoUserTabIndex()
        {
            var comp = Context.Render<TestBooleanInput>(p =>
                p.AddUnmatched("other", "value")
            );

            var result = comp.Instance.Resolve();

            result.TabIndex.Should().Be(0);
            result.Attributes.Should().ContainKey("other");
        }

        [Test]
        public void ResolveTabIndexAndAttributes_Disabled_ForcesMinusOne()
        {
            var comp = Context.Render<TestBooleanInput>(p =>
            {
                p.Add(x => x.Disabled, true);
                p.AddUnmatched("tabindex", "5");
            });

            var result = comp.Instance.Resolve();

            result.TabIndex.Should().Be(-1);
        }

        private class TestBooleanInput : MudBooleanInput<bool>
        {
            public (int TabIndex, IReadOnlyDictionary<string, object> Attributes) Resolve()
                => ResolveTabIndexAndAttributes();
        }
    }
}
