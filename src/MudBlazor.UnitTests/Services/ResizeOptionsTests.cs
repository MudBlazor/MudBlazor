// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class ResizeOptionsTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DefaultValues()
        {
            var option = new ResizeOptions();

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
            var option1 = new ResizeOptions()
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                    { "someKey2", 24 },
                    { "someKey3", 36 },

                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                    { "someKey2", 24 },
                    { "someKey3", 36 },
                }
            };

            option1.Should().Be(option2);
            option2.Should().Be(option1);
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
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_NullAndNotNull()
        {
            var option1 = new ResizeOptions();

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_UnequalCount()
        {
            var option1 = new ResizeOptions()
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                    { "someKey2", 24 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_NotSameKeys()
        {
            var option1 = new ResizeOptions()
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey1", 12 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void Equals_NotTheSame_DiffersInBreakpointDefinitions_DifferentValues()
        {
            var option1 = new ResizeOptions()
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 12 },
                }
            };

            var option2 = new ResizeOptions
            {
                BreakpointDefinitions = new Dictionary<string, int>
                {
                    { "someKey", 23 },
                }
            };

            option1.Should().NotBe(option2);
            option2.Should().NotBe(option1);
        }

        [Test]
        public void ComparingWithNull_ShouldNot_Fail()
        {
            var option = new ResizeOptions();
            // this should not cause nullref
            (option == null).Should().Be(false);
            (option != null).Should().Be(true);
        }
    }

}
