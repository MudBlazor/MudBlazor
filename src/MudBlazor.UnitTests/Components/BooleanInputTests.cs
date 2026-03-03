// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        public void GetResolvedTabIndex_RespectsCaseInsensitiveTabIndex(string key, string value)
        {
            var input = new TestBooleanInput();
            input.SetUserAttributes(new Dictionary<string, object> { { key, value } });
            input.GetResolvedTabIndex().Should().Be(int.Parse(value));
        }

        [Test]
        [TestCase("tabindex")]
        [TestCase("tabIndex")]
        [TestCase("TABINDEX")]
        public void GetResolvedUserAttributes_RemovesAllTabIndexVariants(string key)
        {
            var input = new TestBooleanInput();
            input.SetUserAttributes(new Dictionary<string, object>
            {
                { key, "5" },
                { "other", "value" }
            });
            var attrs = input.GetResolvedUserAttributes();
            attrs.Should().NotContainKey(key);
            attrs.Should().ContainKey("other");
        }

        private class TestBooleanInput : MudBooleanInput<bool>
        {
            public void SetUserAttributes(Dictionary<string, object> attrs)
            {
                base.UserAttributes = attrs;
            }
        }            
    }
}
