// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    public class RandomColorSelectorTests
    {
        [Test]
        public void GetRandomColor()
        {
            String color = RandomColorSelector.GetRandomColor();

            color.Should().NotBeNull().And.HaveLength(7);
        }
    }
}
