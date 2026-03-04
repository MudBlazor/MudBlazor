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
        public void InputTabIndex_RespectsCaseInsensitiveTabIndex(string key, string value)
        {
            var comp = Context.Render<TestBooleanInput>(p =>
                p.AddUnmatched(key, value)
            );

            comp.Instance.TabIndex.Should().Be(int.Parse(value));
            comp.Instance.Attributes.Should().BeNull();
        }

        [Test]
        [TestCase("tabindex")]
        [TestCase("tabIndex")]
        [TestCase("TABINDEX")]
        public void InputUserAttributes_RemovesAllTabIndexVariants(string key)
        {
            var comp = Context.Render<TestBooleanInput>(p =>
            {
                p.AddUnmatched(key, "5");
                p.AddUnmatched("other", "value");
            });

            comp.Instance.TabIndex.Should().Be(5);
            comp.Instance.Attributes.Should().NotContainKey(key);
            comp.Instance.Attributes.Should().ContainKey("other");
        }

        [Test]
        public void InputTabIndex_DefaultsToZero_WhenNoUserTabIndex()
        {
            var comp = Context.Render<TestBooleanInput>(p =>
                p.AddUnmatched("other", "value")
            );

            comp.Instance.TabIndex.Should().Be(0);
            comp.Instance.Attributes.Should().ContainKey("other");
        }

        [Test]
        public void InputTabIndex_Disabled_ForcesMinusOne()
        {
            var comp = Context.Render<TestBooleanInput>(p =>
            {
                p.Add(x => x.Disabled, true);
                p.AddUnmatched("tabindex", "5");
            });

            comp.Instance.TabIndex.Should().Be(-1);
        }

        private class TestBooleanInput : MudBooleanInput<bool>
        {
            public int TabIndex => InputTabIndex;
            public IReadOnlyDictionary<string, object> Attributes => InputUserAttributes?.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
