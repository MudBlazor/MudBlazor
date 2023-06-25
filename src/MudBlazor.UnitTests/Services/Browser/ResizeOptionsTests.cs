// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Browser
{
#nullable enable
    [TestFixture]
    public class ResizeOptionsTests
    {
        [Test]
        public void DefaultValues()
        {
            // Arrange
            var option = new ResizeOptions();

            // Assert
            option.ReportRate.Should().Be(100);
            option.EnableLogging.Should().BeFalse();
            option.SuppressInitEvent.Should().BeTrue();
            option.NotifyOnBreakpointOnly.Should().BeTrue();
            option.BreakpointDefinitions.Should().NotBeNull().And.BeEmpty();
        }

        [Test]
        public void Equals_Same_Instance()
        {
            var option = new ResizeOptions();
            option.Should().Be(option);
        }

        [Test]
        public void Equals_Same_DefaultInstance()
        {
            var option1 = new ResizeOptions();
            var option2 = new ResizeOptions();

            option1.Should().Be(option2);
            option2.Should().Be(option1);
        }

        [Test]
        public void Equals_Same_NonDefaultValues()
        {
            var option1 = new ResizeOptions
            {
                EnableLogging = true,
                NotifyOnBreakpointOnly = false,
                ReportRate = 125,
                SuppressInitEvent = false,
            };

            var option2 = new ResizeOptions
            {
                EnableLogging = true,
                NotifyOnBreakpointOnly = false,
                ReportRate = 125,
                SuppressInitEvent = false,
            };

            option1.Should().Be(option2);
            option2.Should().Be(option1);
        }

        [Test]
        public void Equals_TheSame_WithBreakpointDefinitions()
        {
            var option1 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                    { Breakpoint.Lg, 24 },
                    { Breakpoint.Md, 36 },

                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                    { Breakpoint.Lg, 24 },
                    { Breakpoint.Md, 36 },
                }
            };

            option1.Should().Be(option2);
            option2.Should().Be(option1);
        }

        [Test]
        public void Equals_TheSame_BreakpointDefinitions_BothNull()
        {
            var option1 = new ResizeOptions { BreakpointDefinitions = null };
            var option2 = new ResizeOptions { BreakpointDefinitions = null };

            option1.Should().Be(option2);
            option2.Should().Be(option1);
        }

        [Test]
        public void Equals_NotTheSame_OtherNull()
        {
            // Arrange
            var option1 = new ResizeOptions();
            ResizeOptions? option2 = null;

            // Act
            var result = option1.Equals(option2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_NotTheSame_DiffersInReportRate()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                ReportRate = option1.ReportRate - 10,
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInEnableLogging()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                EnableLogging = !option1.EnableLogging
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInSuppressInitEvent()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                SuppressInitEvent = !option1.SuppressInitEvent
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInNotifyOnBreakpointOnly()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                NotifyOnBreakpointOnly = !option1.NotifyOnBreakpointOnly
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_EmptyAndNotEmpty()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }


        [Test]
        public void Equals_NotTheSame_BreakpointDefinitions_EmptyAndNull()
        {
            var option1 = new ResizeOptions { BreakpointDefinitions = new Dictionary<Breakpoint, int>() };
            var option2 = new ResizeOptions { BreakpointDefinitions = null };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_UnequalCount()
        {
            var option1 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                    { Breakpoint.Lg, 24 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_NotSameKeys()
        {
            var option1 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Lg, 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_DifferentValues()
        {
            var option1 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 12 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<Breakpoint, int>
                {
                    { Breakpoint.Xl, 23 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void OperatorEquals_EqualInstances_ReturnsTrue()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions();

            // Act
            var result = options1 == options2;

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void OperatorEquals_NullLeftOperand_ReturnsFalse()
        {
            // Arrange
            ResizeOptions? options = null;
            var otherOptions = new ResizeOptions();

            // Act
            var result = options == otherOptions;

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void OperatorEquals_NullRightOperand_ReturnsFalse()
        {
            // Arrange
            var options = new ResizeOptions();
            ResizeOptions? otherOptions = null;

            // Act
            var result = options == otherOptions;

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void OperatorEquals_SameInstances_ReturnsTrue()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = options1;

            // Act
            var result = options1 == options2;

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void OperatorEquals_DifferentInstances_ReturnsTrue()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions();

            // Act
            var result = options1 == options2;

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void OperatorEquals_BothNulls_ReturnsTrue()
        {
            // Arrange
            ResizeOptions? options1 = null;
            ResizeOptions? options2 = null;

            // Act
            var result = options1 == options2;

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void NotEqualOperator_WhenObjectsAreNotEqual_ReturnsTrue()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions { ReportRate = 200 };

            // Act
            var result = options1 != options2;

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void NotEqualOperator_WhenObjectsAreEqual_ReturnsFalse()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions();

            // Act
            var result = options1 != options2;

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void GetHashCode_WhenObjectsAreEqual_ReturnsSameHashCode()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions();

            // Act
            var hashCode1 = options1.GetHashCode();
            var hashCode2 = options2.GetHashCode();

            // Assert
            hashCode1.Should().Be(hashCode2);
        }

        [Test]
        public void GetHashCode_WhenObjectsAreNotEqual_ReturnsDifferentHashCode()
        {
            // Arrange
            var options1 = new ResizeOptions();
            var options2 = new ResizeOptions { ReportRate = 200 };

            // Act
            var hashCode1 = options1.GetHashCode();
            var hashCode2 = options2.GetHashCode();

            // Assert
            hashCode1.Should().NotBe(hashCode2);
        }
    }
}
