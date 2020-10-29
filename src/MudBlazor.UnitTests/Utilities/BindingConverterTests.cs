using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class BindingConverterTests
    {
        [Test]
        public void DefaultConverterTest()
        {
            var c1 = new DefaultConverter<string>();
            c1.Set("hello").Should().Be("hello");
            c1.Get("hello").Should().Be("hello");
            var c2 = new DefaultConverter<int>();
            c2.Set(17).Should().Be("17");
            c2.Get("17").Should().Be(17);
            var c3 = new DefaultConverter<int?>();
            c3.Set(17).Should().Be("17");
            c3.Get("17").Should().Be(17);
            c3.Set(null).Should().Be(null);
            c3.Get(null).Should().Be(null);
            var c4 = new DefaultConverter<double?>();
            c4.Set(1.7).Should().Be("1.7");
            c4.Get("1.7").Should().Be(1.7);
            c4.Set(null).Should().Be(null);
            c4.Get(null).Should().Be(null);
            c4.Culture = CultureInfo.GetCultureInfo("de-AT");
            c4.Set(1.7).Should().Be("1,7");
            c4.Get("1,7").Should().Be(1.7);
            var c5 = new DefaultConverter<ButtonType>();
            c5.Set(ButtonType.Button).Should().Be("Button");
            c5.Get("Button").Should().Be(ButtonType.Button);
            var c6 = new DefaultConverter<DateTime?>();
            var date = DateTime.Today;
            c6.Get(c6.Set(date)).Should().Be(date);
            c6.Set(null).Should().Be(null);
            c6.Get(null).Should().Be(null);
            var c7 = new DefaultConverter<TimeSpan?>();
            var time = DateTime.Now.TimeOfDay;
            c7.Get(c7.Set(time)).Should().Be(time);
            c7.Set(null).Should().Be(null);
            c7.Get(null).Should().Be(null);
        }
    }
}