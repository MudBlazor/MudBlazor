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
    [Obsolete]
    [TestFixture]
    public class SubscriptionInfoTests
    {
        [Test]
        public void Constructor()
        {
            var option = new ResizeOptions();
            var info = new ResizeServiceSubscriptionInfo(option);

            info.Option.Should().Be(option);
        }

        [Test]
        public void AddSubscription()
        {
            var info = new ResizeServiceSubscriptionInfo(new ResizeOptions());

            var id = info.AddSubscription((x) => { });
            info.ContainsSubscription(id).Should().BeTrue();
        }

        [Test]
        public void ContainsSubscription_NotFound()
        {
            var info = new ResizeServiceSubscriptionInfo(new ResizeOptions());

            info.ContainsSubscription(Guid.NewGuid()).Should().BeFalse();
        }

        [Test]
        public void RemoveSubscription_IsNotLast()
        {
            var info = new ResizeServiceSubscriptionInfo(new ResizeOptions());

            var firstId = info.AddSubscription((x) => { });
            var secondId = info.AddSubscription((x) => { });

            info.ContainsSubscription(firstId).Should().BeTrue();
            info.RemoveSubscription(firstId).Should().BeFalse();
            info.ContainsSubscription(firstId).Should().BeFalse();

        }

        [Test]
        public void RemoveSubscription_IsLast()
        {
            var info = new ResizeServiceSubscriptionInfo(new ResizeOptions());

            var firstId = info.AddSubscription((x) => { });
            var secondId = info.AddSubscription((x) => { });

            info.ContainsSubscription(firstId).Should().BeTrue();
            info.RemoveSubscription(firstId).Should().BeFalse();
            info.ContainsSubscription(firstId).Should().BeFalse();

            info.ContainsSubscription(secondId).Should().BeTrue();
            info.RemoveSubscription(secondId).Should().BeTrue();
            info.ContainsSubscription(secondId).Should().BeFalse();
        }

    }
}
