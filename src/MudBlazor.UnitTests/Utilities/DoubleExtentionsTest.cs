// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class DoubleExtentionsTest
    {
        [TestCase(0.0, 0.0)]
        [TestCase(180.0, Math.PI)]
        [TestCase(-180.0, -Math.PI)]
        [TestCase(360.0, 2 * Math.PI)]
        [TestCase(-360.0, -(2 * Math.PI))]
        [TestCase(90, Math.PI / 2.0)]
        public void ToRad(Double input, Double expectedOutput)
        {
            Double output = input.ToRad();
            output.Should().Be(expectedOutput);
        }
    }
}
