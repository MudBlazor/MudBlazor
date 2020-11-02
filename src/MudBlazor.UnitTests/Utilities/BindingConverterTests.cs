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

        [Test]
        public void DateTimeConvertersTest()
        {
            var dt1 = new DateConverter("dd/MM/yyyy");
            dt1.Culture = new CultureInfo("pt-BR");
            dt1.Set(new DateTime(2020, 11, 2)).Should().Be("02/11/2020");
            dt1.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            var dt2 = new NullableDateConverter("dd/MM/yyyy");
            dt2.Culture = new CultureInfo("pt-BR");
            dt2.Set(new DateTime(2020, 11, 2)).Should().Be("02/11/2020");
            dt2.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            dt2.Set(null).Should().Be(null);
            dt2.Get(null).Should().Be(null);

            var dt3 = new DateConverter("dd/MM/yyyy");
            dt3.Culture = new CultureInfo("de-AT");
            dt3.Set(new DateTime(2020, 11, 2)).Should().Be("02.11.2020");
            dt3.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            var dt4 = new NullableDateConverter("dd/MM/yyyy");
            dt4.Culture = new CultureInfo("de-AT");
            dt4.Set(new DateTime(2020, 11, 2)).Should().Be("02.11.2020");
            dt4.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            dt4.Set(null).Should().Be(null);
            dt4.Get(null).Should().Be(null);
        }

    }
}