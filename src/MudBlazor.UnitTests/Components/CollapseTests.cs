// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CollapseTests : BunitTest
    {
        [Test]
        public void Collapse_TwoWayBinding_Test1()
        {
            // This is to check the behaviour that ExpandedChanged fires when Expanded is changed outside.
            // Perhaps this behaviour is not correct as mentioned here https://github.com/MudBlazor/MudBlazor/pull/8041#discussion_r1504480792
            // Native and MS components don't do that, but to keep the old behaviour for now otherwise this would be a breaking change.
            var currentValue = false;
            var comp = Context
                .RenderComponent<MudCollapse>(parameters => parameters
                    .Bind(component => component.Expanded, currentValue, newValue => currentValue = newValue));

            SetExpanded(true);
            currentValue.Should().BeTrue();
            SetExpanded(false);
            currentValue.Should().BeFalse();

            return;

            void SetExpanded(bool expanded)
            {
                var expandedParameter = Parameter(nameof(MudCollapse.Expanded), expanded);
                comp.SetParametersAndRender(expandedParameter);
            }
        }
    }
}
