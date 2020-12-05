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
            var c4 = new DefaultConverter<double?>() {Culture=CultureInfo.InvariantCulture };
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
        public void DefaultConverterTestWithCustomFormat()
        {
            var float1 = new DefaultConverter<float>() { Format = "0.00" };
            float1.Culture = new CultureInfo("en-US");
            float1.Set(1.7f).Should().Be("1.70");
            float1.Set(1.773f).Should().Be("1.77");
            float1.Get("1.773").Should().Be(1.773f);
            float1.Get("1.77").Should().Be(1.77f);
            float1.Get("1.7").Should().Be(1.7f);
            float1.Culture = new CultureInfo("pt-BR");
            float1.Set(1.7f).Should().Be("1,70");
            float1.Set(1.773f).Should().Be("1,77");
            float1.Get("1,773").Should().Be(1.773f);
            float1.Get("1,77").Should().Be(1.77f);
            float1.Get("1,7").Should().Be(1.7f);

            var dbl1 = new DefaultConverter<double>() { Format = "0.00" };
            dbl1.Culture = new CultureInfo("en-US");
            dbl1.Set(1.7d).Should().Be("1.70");
            dbl1.Set(1.773d).Should().Be("1.77");
            dbl1.Get("1.773").Should().Be(1.773d);
            dbl1.Get("1.77").Should().Be(1.77d);
            dbl1.Get("1.7").Should().Be(1.7d);
            dbl1.Culture = new CultureInfo("pt-BR");
            dbl1.Set(1.7d).Should().Be("1,70");
            dbl1.Set(1.773d).Should().Be("1,77");
            dbl1.Get("1,773").Should().Be(1.773d);
            dbl1.Get("1,77").Should().Be(1.77d);
            dbl1.Get("1,7").Should().Be(1.7d);

            var dbl2 = new DefaultConverter<double?>() { Format = "0.00" };
            dbl2.Culture = new CultureInfo("en-US");
            dbl2.Set(1.7d).Should().Be("1.70");
            dbl2.Set(1.773d).Should().Be("1.77");
            dbl2.Set(null).Should().Be(null);
            dbl2.Get("1.773").Should().Be(1.773d);
            dbl2.Get("1.77").Should().Be(1.77d);
            dbl2.Get("1.7").Should().Be(1.7d);
            dbl2.Get(null).Should().Be(null);
            dbl2.Culture = new CultureInfo("pt-BR");
            dbl2.Set(1.7d).Should().Be("1,70");
            dbl2.Set(1.773d).Should().Be("1,77");
            dbl2.Get("1,773").Should().Be(1.773d);
            dbl2.Get("1,77").Should().Be(1.77d);
            dbl2.Get("1,7").Should().Be(1.7d);

            var dec1 = new DefaultConverter<decimal>() { Format = "0.00" };
            dec1.Culture = new CultureInfo("en-US");
            dec1.Set(1.7m).Should().Be("1.70");
            dec1.Set(1.773m).Should().Be("1.77");
            dec1.Get("1.773").Should().Be(1.773m);
            dec1.Get("1.77").Should().Be(1.77m);
            dec1.Get("1.7").Should().Be(1.7m);
            dec1.Culture = new CultureInfo("pt-BR");
            dec1.Set(1.7m).Should().Be("1,70");
            dec1.Set(1.773m).Should().Be("1,77");
            dec1.Get("1,773").Should().Be(1.773m);
            dec1.Get("1,77").Should().Be(1.77m);
            dec1.Get("1,7").Should().Be(1.7m);

            var dec2 = new DefaultConverter<decimal?>() { Format = "0.00" };
            dec2.Culture = new CultureInfo("en-US");
            dec2.Set(1.7m).Should().Be("1.70");
            dec2.Set(1.773m).Should().Be("1.77");
            dec2.Set(null).Should().Be(null);
            dec2.Get("1.773").Should().Be(1.773m);
            dec2.Get("1.77").Should().Be(1.77m);
            dec2.Get("1.7").Should().Be(1.7m);
            dec2.Get(null).Should().Be(null);
            dec2.Culture = new CultureInfo("pt-BR");
            dec2.Set(1.7m).Should().Be("1,70");
            dec2.Set(1.773m).Should().Be("1,77");
            dec2.Get("1,773").Should().Be(1.773m);
            dec2.Get("1,77").Should().Be(1.77m);
            dec2.Get("1,7").Should().Be(1.7m);

            var dt1 = new DefaultConverter<DateTime>() { Format = "MM/dd/yyyy" };
            dt1.Culture = new CultureInfo("en-US");
            dt1.Set(new DateTime(2020,11,03)).Should().Be("11/03/2020");
            dt1.Get("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt1.Culture = new CultureInfo("pt-BR");
            dt1.Format = "dd/MM/yyyy";
            dt1.Set(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt1.Get("03/11/2020").Should().Be(new DateTime(2020, 11, 03));

            var dt2 = new DefaultConverter<DateTime?>() { Format = "MM/dd/yyyy" };
            dt2.Culture = new CultureInfo("en-US");
            dt2.Set(new DateTime(2020, 11, 03)).Should().Be("11/03/2020");
            dt2.Set(null).Should().Be(null);
            dt2.Get("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt2.Get(null).Should().Be(null);
            dt2.Culture = new CultureInfo("pt-BR");
            dt2.Format = "dd/MM/yyyy";
            dt2.Set(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt2.Get("03/11/2020").Should().Be(new DateTime(2020, 11, 03));
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

        [Test]
        public void BoolConverterTest()
        {
            var c1 = new BoolConverter<bool>();
            c1.Set(true).Should().Be(true);
            c1.Set(false).Should().Be(false);
            c1.Get(null).Should().BeFalse();
            c1.Get(false).Should().BeFalse();
            c1.Get(true).Should().BeTrue();
            var c2 = new BoolConverter<bool?>();
            c2.Set(true).Should().Be(true);
            c2.Set(false).Should().Be(false);
            c2.Set(null).Should().BeNull();
            c2.Get(null).Should().BeNull();
            c2.Get(false).Should().BeFalse();
            c2.Get(true).Should().BeTrue();
            var c3 = new BoolConverter<string>();
            c3.Set("true").Should().Be(true);
            c3.Set("false").Should().Be(false);
            c3.Set("on").Should().Be(true);
            c3.Set("off").Should().Be(false);
            c3.Set(null).Should().BeNull();
            c3.Get(null).Should().BeNull();
            c3.Get(false).Should().Be("off");
            c3.Get(true).Should().Be("on");
            var c4 = new BoolConverter<int>();
            c4.Set(1).Should().Be(true);
            c4.Set(0).Should().Be(false);
            c4.Get(null).Should().Be(0);
            c4.Get(false).Should().Be(0);
            c4.Get(true).Should().Be(1);
            var c5 = new BoolConverter<int?>();
            c5.Set(17).Should().Be(true);
            c5.Set(-1).Should().Be(false);
            c5.Set(null).Should().BeNull();
            c5.Get(null).Should().BeNull();
            c5.Get(false).Should().Be(0);
            c5.Get(true).Should().Be(1);

            // non-convertable types will be handled without exceptions
            var c6 = new BoolConverter<DateTime>();
            c6.Set(DateTime.Now).Should().Be(null);
            c6.Get(true).Should().Be(default(DateTime));
            c6.Get(false).Should().Be(default(DateTime));
            c6.Get(null).Should().Be(default(DateTime));
        }
    }
}