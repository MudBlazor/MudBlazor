﻿using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class BindingConverterTests
    {
        [Test]
        public void DefaultIntegerConverterTest()
        {
            var c10 = new DefaultConverter<sbyte>();
            c10.Set(123).Should().Be("123");
            c10.Get("123").Should().Be(123);
            var c11 = new DefaultConverter<sbyte?>();
            c11.Set(123).Should().Be("123");
            c11.Get("123").Should().Be(123);
            c11.Set(null).Should().Be(null);
            c11.Get(null).Should().Be(null);
            var c12 = new DefaultConverter<byte>();
            c12.Set(234).Should().Be("234");
            c12.Get("234").Should().Be(234);
            var c13 = new DefaultConverter<byte?>();
            c13.Set(234).Should().Be("234");
            c13.Get("234").Should().Be(234);
            c13.Set(null).Should().Be(null);
            c13.Get(null).Should().Be(null);
            var c14 = new DefaultConverter<short>();
            c14.Set(1234).Should().Be("1234");
            c14.Get("1234").Should().Be(1234);
            var c15 = new DefaultConverter<short?>();
            c15.Set(1234).Should().Be("1234");
            c15.Get("1234").Should().Be(1234);
            c15.Set(null).Should().Be(null);
            c15.Get(null).Should().Be(null);
            var c16 = new DefaultConverter<ushort>();
            c16.Set(12345).Should().Be("12345");
            c16.Get("12345").Should().Be(12345);
            var c17 = new DefaultConverter<ushort?>();
            c17.Set(12345).Should().Be("12345");
            c17.Get("12345").Should().Be(12345);
            c17.Set(null).Should().Be(null);
            c17.Get(null).Should().Be(null);
            var c18 = new DefaultConverter<int>();
            c18.Set(34567).Should().Be("34567");
            c18.Get("34567").Should().Be(34567);
            var c19 = new DefaultConverter<int?>();
            c19.Set(34567).Should().Be("34567");
            c19.Get("34567").Should().Be(34567);
            c19.Set(null).Should().Be(null);
            c19.Get(null).Should().Be(null);
            var c20 = new DefaultConverter<uint>();
            c20.Set(45678).Should().Be("45678");
            c20.Get("45678").Should().Be(45678);
            var c21 = new DefaultConverter<uint?>();
            c21.Set(45678).Should().Be("45678");
            c21.Get("45678").Should().Be(45678);
            c21.Set(null).Should().Be(null);
            c21.Get(null).Should().Be(null);
            var c22 = new DefaultConverter<long>();
            c22.Set(456789).Should().Be("456789");
            c22.Get("456789").Should().Be(456789);
            var c23 = new DefaultConverter<long?>();
            c23.Set(456789).Should().Be("456789");
            c23.Get("456789").Should().Be(456789);
            c23.Set(null).Should().Be(null);
            c23.Get(null).Should().Be(null);
            var c24 = new DefaultConverter<ulong>();
            c24.Set(4567890).Should().Be("4567890");
            c24.Get("4567890").Should().Be(4567890);
            var c25 = new DefaultConverter<ulong?>();
            c25.Set(4567890).Should().Be("4567890");
            c25.Get("4567890").Should().Be(4567890);
            c25.Set(null).Should().Be(null);
            c25.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverterTest()
        {
            var c1 = new DefaultConverter<string>();
            c1.Set("hello").Should().Be("hello");
            c1.Get("hello").Should().Be("hello");
            c1.Set("").Should().Be("");
            c1.Get("").Should().Be("");
            c1.Get(null).Should().Be(null);
            c1.Set(null).Should().Be(null);
            var c3 = new DefaultConverter<double?>() { Culture = CultureInfo.InvariantCulture };
            c3.Set(1.7).Should().Be("1.7");
            c3.Get("1.7").Should().Be(1.7);
            c3.Get("1234567.15").Should().Be(1234567.15);
            c3.Set(1234567.15).Should().Be("1234567.15");
            c3.Set(c3.Get("1234567.15")).Should().Be("1234567.15");
            c3.Get(c3.Set(1234567.15)).Should().Be(1234567.15);
            c3.Set(null).Should().Be(null);
            c3.Get(null).Should().Be(null);
            c3.Culture = CultureInfo.GetCultureInfo("de-AT");
            c3.Set(1.7).Should().Be("1,7");
            c3.Get("1,7").Should().Be(1.7);
            var c4 = new DefaultConverter<ButtonType>();
            c4.Set(ButtonType.Button).Should().Be("Button");
            c4.Get("Button").Should().Be(ButtonType.Button);
            var c5 = new DefaultConverter<DateTime?>();
            var date = DateTime.Today;
            c5.Get(c5.Set(date)).Should().Be(date);
            c5.Set(null).Should().Be(null);
            c5.Get(null).Should().Be(null);
            var c6 = new DefaultConverter<TimeSpan>();
            var time = DateTime.Now.TimeOfDay;
            c6.Get(c6.Set(time)).Should().Be(time);
            var c7 = new DefaultConverter<TimeSpan?>();
            var time2 = DateTime.Now.TimeOfDay;
            c7.Get(c7.Set(time2)).Should().Be(time2);
            c7.Set(null).Should().Be(null);
            c7.Get(null).Should().Be(null);
            var c8 = new DefaultConverter<bool>();
            c8.Set(true).Should().Be("True");
            c8.Set(false).Should().Be("False");
            c8.Get("true").Should().Be(true);
            c8.Get("True").Should().Be(true);
            c8.Get("false").Should().Be(false);
            c8.Get("ON").Should().Be(true);
            c8.Get("off").Should().Be(false);
            c8.Get("").Should().Be(false);
            c8.Get("asdf").Should().Be(false);
            var c9 = new DefaultConverter<bool?>();
            c9.Set(true).Should().Be("True");
            c9.Get("true").Should().Be(true);
            c9.Set(false).Should().Be("False");
            c9.Get("false").Should().Be(false);
            c9.Set(null).Should().Be(null);
            c9.Get(null).Should().Be(null);
        }

        public enum YesNoMaybe { Maybe, Yes, No }

        [Test]
        public void DefaultConverterTest2()
        {
            var c1 = new DefaultConverter<char>();
            c1.Set('x').Should().Be("x");
            c1.Get("a").Should().Be('a');
            c1.Get("").Should().Be(default(char));
            c1.Get(null).Should().Be(default(char));
            var c2 = new DefaultConverter<char?>();
            c2.Set('x').Should().Be("x");
            c2.Get("a").Should().Be('a');
            c2.Get("").Should().Be(null);
            c2.Get(null).Should().Be(null);
            c2.Set(null).Should().Be(null);
            var c3 = new DefaultConverter<Guid>();
            var guid = Guid.NewGuid();
            c3.Set(guid).Should().Be(guid.ToString());
            c3.Get(guid.ToString()).Should().Be(guid);
            c3.Get("").Should().Be(Guid.Empty);
            c3.Get(null).Should().Be(Guid.Empty);
            var c4 = new DefaultConverter<Guid?>();
            Guid? guid2;
            guid2 = Guid.NewGuid();
            c4.Set(guid2).Should().Be(guid2.ToString());
            c4.Set(null).Should().Be(null);
            c4.Get(guid2.ToString()).Should().Be(guid2);
            c4.Get("").Should().Be(null);
            c4.Get(null).Should().Be(null);
            var c5 = new DefaultConverter<YesNoMaybe>();
            c5.Set(YesNoMaybe.Yes).Should().Be("Yes");
            c5.Get("No").Should().Be(YesNoMaybe.No);
            c5.Get("").Should().Be(default(YesNoMaybe));
            c5.Get(null).Should().Be(default(YesNoMaybe));
            var c6 = new DefaultConverter<YesNoMaybe?>();
            c6.Set(YesNoMaybe.Maybe).Should().Be("Maybe");
            c6.Get("Maybe").Should().Be(YesNoMaybe.Maybe);
            c6.Get("").Should().Be(null);
            c6.Get(null).Should().Be(null);
            c6.Set(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverterTestWithCustomFormat()
        {
            var float1 = new DefaultConverter<float>() { Format = "0.00" };
            float1.Culture = new CultureInfo("en-US", false);
            float1.Set(1.7f).Should().Be("1.70");
            float1.Set(1.773f).Should().Be("1.77");
            float1.Get("1.773").Should().Be(1.773f);
            float1.Get("1.77").Should().Be(1.77f);
            float1.Get("1.7").Should().Be(1.7f);
            float1.Culture = new CultureInfo("pt-BR", false);
            float1.Set(1.7f).Should().Be("1,70");
            float1.Set(1.773f).Should().Be("1,77");
            float1.Get("1,773").Should().Be(1.773f);
            float1.Get("1,77").Should().Be(1.77f);
            float1.Get("1,7").Should().Be(1.7f);

            var float2 = new DefaultConverter<float?>() { Format = "0.00" };
            float2.Culture = new CultureInfo("en-US", false);
            float2.Set(1.7f).Should().Be("1.70");
            float2.Set(1.773f).Should().Be("1.77");
            float2.Set(null).Should().Be(null);
            float2.Get("1.773").Should().Be(1.773f);
            float2.Get("1.77").Should().Be(1.77f);
            float2.Get("1.7").Should().Be(1.7f);
            float2.Get(null).Should().Be(null);
            float2.Culture = new CultureInfo("pt-BR", false);
            float2.Set(1.7f).Should().Be("1,70");
            float2.Set(1.773f).Should().Be("1,77");
            float2.Get("1,773").Should().Be(1.773f);
            float2.Get("1,77").Should().Be(1.77f);
            float2.Get("1,7").Should().Be(1.7f);

            var dbl1 = new DefaultConverter<double>() { Format = "0.00" };
            dbl1.Culture = new CultureInfo("en-US", false);
            dbl1.Set(1.7d).Should().Be("1.70");
            dbl1.Set(1.773d).Should().Be("1.77");
            dbl1.Get("1.773").Should().Be(1.773d);
            dbl1.Get("1.77").Should().Be(1.77d);
            dbl1.Get("1.7").Should().Be(1.7d);
            dbl1.Culture = new CultureInfo("pt-BR", false);
            dbl1.Set(1.7d).Should().Be("1,70");
            dbl1.Set(1.773d).Should().Be("1,77");
            dbl1.Get("1,773").Should().Be(1.773d);
            dbl1.Get("1,77").Should().Be(1.77d);
            dbl1.Get("1,7").Should().Be(1.7d);

            var dbl2 = new DefaultConverter<double?>() { Format = "0.00" };
            dbl2.Culture = new CultureInfo("en-US", false);
            dbl2.Set(1.7d).Should().Be("1.70");
            dbl2.Set(1.773d).Should().Be("1.77");
            dbl2.Set(null).Should().Be(null);
            dbl2.Get("1.773").Should().Be(1.773d);
            dbl2.Get("1.77").Should().Be(1.77d);
            dbl2.Get("1.7").Should().Be(1.7d);
            dbl2.Get(null).Should().Be(null);
            dbl2.Culture = new CultureInfo("pt-BR", false);
            dbl2.Set(1.7d).Should().Be("1,70");
            dbl2.Set(1.773d).Should().Be("1,77");
            dbl2.Get("1,773").Should().Be(1.773d);
            dbl2.Get("1,77").Should().Be(1.77d);
            dbl2.Get("1,7").Should().Be(1.7d);

            var dec1 = new DefaultConverter<decimal>() { Format = "0.00" };
            dec1.Culture = new CultureInfo("en-US", false);
            dec1.Set(1.7m).Should().Be("1.70");
            dec1.Set(1.773m).Should().Be("1.77");
            dec1.Get("1.773").Should().Be(1.773m);
            dec1.Get("1.77").Should().Be(1.77m);
            dec1.Get("1.7").Should().Be(1.7m);
            dec1.Culture = new CultureInfo("pt-BR", false);
            dec1.Set(1.7m).Should().Be("1,70");
            dec1.Set(1.773m).Should().Be("1,77");
            dec1.Get("1,773").Should().Be(1.773m);
            dec1.Get("1,77").Should().Be(1.77m);
            dec1.Get("1,7").Should().Be(1.7m);

            var dec2 = new DefaultConverter<decimal?>() { Format = "0.00" };
            dec2.Culture = new CultureInfo("en-US", false);
            dec2.Set(1.7m).Should().Be("1.70");
            dec2.Set(1.773m).Should().Be("1.77");
            dec2.Set(null).Should().Be(null);
            dec2.Get("1.773").Should().Be(1.773m);
            dec2.Get("1.77").Should().Be(1.77m);
            dec2.Get("1.7").Should().Be(1.7m);
            dec2.Get(null).Should().Be(null);
            dec2.Culture = new CultureInfo("pt-BR", false);
            dec2.Set(1.7m).Should().Be("1,70");
            dec2.Set(1.773m).Should().Be("1,77");
            dec2.Get("1,773").Should().Be(1.773m);
            dec2.Get("1,77").Should().Be(1.77m);
            dec2.Get("1,7").Should().Be(1.7m);

            var dt1 = new DefaultConverter<DateTime>() { Format = "MM/dd/yyyy" };
            dt1.Culture = new CultureInfo("en-US", false);
            dt1.Set(new DateTime(2020, 11, 03)).Should().Be("11/03/2020");
            dt1.Get("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt1.Culture = new CultureInfo("pt-BR", false);
            dt1.Format = "dd/MM/yyyy";
            dt1.Set(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt1.Get("03/11/2020").Should().Be(new DateTime(2020, 11, 03));

            var dt2 = new DefaultConverter<DateTime?>() { Format = "MM/dd/yyyy" };
            dt2.Culture = new CultureInfo("en-US", false);
            dt2.Set(new DateTime(2020, 11, 03)).Should().Be("11/03/2020");
            dt2.Set(null).Should().Be(null);
            dt2.Get("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt2.Get(null).Should().Be(null);
            dt2.Culture = new CultureInfo("pt-BR", false);
            dt2.Format = "dd/MM/yyyy";
            dt2.Set(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt2.Get("03/11/2020").Should().Be(new DateTime(2020, 11, 03));
        }

        [Test]
        public void DateTimeConvertersTest()
        {
            var dt1 = new DateConverter("dd/MM/yyyy");
            dt1.Culture = new CultureInfo("pt-BR", false);
            dt1.Set(new DateTime(2020, 11, 2)).Should().Be("02/11/2020");
            dt1.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            var dt2 = new NullableDateConverter("dd/MM/yyyy");
            dt2.Culture = new CultureInfo("pt-BR", false);
            dt2.Set(new DateTime(2020, 11, 2)).Should().Be("02/11/2020");
            dt2.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            dt2.Set(null).Should().Be(null);
            dt2.Get(null).Should().Be(null);

            var dt3 = new DateConverter("dd/MM/yyyy");
            dt3.Culture = new CultureInfo("de-AT", false);
            dt3.Set(new DateTime(2020, 11, 2)).Should().Be("02.11.2020");
            dt3.Get("02/11/2020").Should().Be(new DateTime(2020, 11, 2));
            var dt4 = new NullableDateConverter("dd/MM/yyyy");
            dt4.Culture = new CultureInfo("de-AT", false);
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

            // non-convertible types will be handled without exceptions
            var c6 = new BoolConverter<DateTime>();
            c6.Set(DateTime.Now).Should().Be(null);
            c6.Get(true).Should().Be(default(DateTime));
            c6.Get(false).Should().Be(default(DateTime));
            c6.Get(null).Should().Be(default(DateTime));
        }

        [Test]
        public void ErrorCheckingTest()
        {
            //format exception
            var dt1 = new DefaultConverter<DateTime>();
            dt1.Get("12/34/56").Should().Be(default);
            var dtn1 = new DefaultConverter<DateTime?>();
            dtn1.Get("12/34/56").Should().Be(null);

            // format exception
            var tm1 = new DefaultConverter<TimeSpan>();
            tm1.Get("12:o1").Should().Be(default);
            var tmn1 = new DefaultConverter<TimeSpan?>();
            tmn1.Get("12:o1").Should().Be(null);

            // overflow
            var tm2 = new DefaultConverter<TimeSpan>();
            tm2.Get("25:00").Should().Be(default);
            var tm2n = new DefaultConverter<TimeSpan?>();
            tm2n.Get("25:00").Should().Be(null);

            // not a valid number
            var c1 = new DefaultConverter<sbyte>();
            c1.Get("a-z").Should().Be(default);
            var cn1 = new DefaultConverter<sbyte?>();
            cn1.Get("a-z").Should().Be(null);
            var c2 = new DefaultConverter<byte>();
            c2.Get("a-z").Should().Be(default);
            var cn2 = new DefaultConverter<byte?>();
            cn2.Get("a-z").Should().Be(null);
            var c3 = new DefaultConverter<short>();
            c3.Get("a-z").Should().Be(default);
            var c3n = new DefaultConverter<short?>();
            c3n.Get("a-z").Should().Be(null);
            var c4 = new DefaultConverter<ushort>();
            c4.Get("a-z").Should().Be(default);
            var c4n = new DefaultConverter<ushort?>();
            c4n.Get("a-z").Should().Be(null);
            var c5 = new DefaultConverter<int>();
            c5.Get("a-z").Should().Be(default);
            var c5n = new DefaultConverter<int?>();
            c5n.Get("a-z").Should().Be(null);
            var c6 = new DefaultConverter<uint>();
            c6.Get("a-z").Should().Be(default);
            var c6n = new DefaultConverter<uint?>();
            c6n.Get("a-z").Should().Be(null);
            var c7 = new DefaultConverter<long>();
            c7.Get("a-z").Should().Be(default);
            var c7n = new DefaultConverter<long?>();
            c7n.Get("a-z").Should().Be(null);
            var c8 = new DefaultConverter<ulong>();
            c8.Get("a-z").Should().Be(default);
            var c8n = new DefaultConverter<ulong?>();
            c8n.Get("a-z").Should().Be(null);
            var c9 = new DefaultConverter<float>();
            c9.Get("a-z").Should().Be(default);
            var c9n = new DefaultConverter<float?>();
            c9n.Get("a-z").Should().Be(null);
            var c10 = new DefaultConverter<double>();
            c10.Get("a-z").Should().Be(default);
            var c10n = new DefaultConverter<double?>();
            c10n.Get("a-z").Should().Be(null);
            var c11 = new DefaultConverter<decimal>();
            c11.Get("a-z").Should().Be(default);
            var c11n = new DefaultConverter<decimal?>();
            c11n.Get("a-z").Should().Be(null);
            var c12 = new DefaultConverter<Guid>();
            c12.Get("a-z").Should().Be(Guid.Empty);
            var c12n = new DefaultConverter<Guid?>();
            c12n.Get("a-z").Should().Be(null);
            var c13 = new DefaultConverter<YesNoMaybe>();
            c13.Get("a-z").Should().Be(YesNoMaybe.Maybe);
            var c14 = new DefaultConverter<YesNoMaybe?>();
            c14.Get("a-z").Should().Be(null);

            // invalid format for type supplied
            var c16 = new DefaultConverter<int?>();
            c16.Format = "dd/mm/yy";
            c16.Get(c16.Set(22)).Should().Be(null);
        }

        [Test]
        public void NumericBoundariesConverterTest()
        {
            Func<int, int> funcInt = (int i) => i;//Not testing test the function, return the parameter
            Func<double?, double?> funcDbl = (double? d) => d;//Not testing test the function, return the parameter

            //Note: Set doesn't do anything. The Get can change the value
            var c1 = new NumericBoundariesConverter<int>(funcInt);
            c1.Set("hello").Should().Be("hello");
            c1.Get("hello").Should().Be(null);
            c1.GetError.Should().Be(true);
            c1.GetErrorMessage.Should().Be("Not a valid number");
            c1.Set("").Should().Be("");
            c1.Get("").Should().Be(null);
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get(null).Should().Be(null);
            c1.Set(null).Should().Be(null);
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get("0").Should().Be("0");
            c1.Set("0").Should().Be("0");
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get("-2").Should().Be("-2");
            c1.Set("-2").Should().Be("-2");
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Set("1.5").Should().Be("1.5");
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get("1.5").Should().Be(null);
            c1.GetError.Should().Be(true);
            c1.GetErrorMessage.Should().Be("Not a valid number");
            var c3 = new NumericBoundariesConverter<double?>(funcDbl) { Culture = CultureInfo.InvariantCulture };
            c3.Set("1.7").Should().Be("1.7");
            c3.Get("1.7").Should().Be("1.7");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Get("1234567.15").Should().Be("1234567.15");
            c3.Set("1234567.15").Should().Be("1234567.15");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Set(c3.Get("1,234,567.15")).Should().Be("1234567.15");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Get(c3.Set("1,234,567.15")).Should().Be("1234567.15");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Set(".5").Should().Be(".5");
            c3.Get(".5").Should().Be("0.5");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Set("1.").Should().Be("1.");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Get("1.").Should().Be("1");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Culture = CultureInfo.GetCultureInfo("de-AT");
            c3.Set("1,7").Should().Be("1,7");
            c3.Get("1,7").Should().Be("1,7");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Set("1.7").Should().Be("1.7");
            c3.Get("1.7").Should().Be("17");//Changes from 1.7 to 17 because the dot is a valid grouping symbol
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Get("1,").Should().Be("1");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            c3.Get(c3.Set("1.234.567,15")).Should().Be("1234567,15");
            c3.GetError.Should().Be(false);
            c3.GetErrorMessage.Should().BeNull();
            var c4 = new NumericBoundariesConverter<bool>((b) => b);
            c4.Set("true").Should().Be("true");
            c4.Get("true").Should().Be(null);
            c4.GetError.Should().Be(true);
            c4.GetErrorMessage.Should().Be($"Conversion to type {typeof(bool)} not implemented");
        }

        [Test]
        public void NumericBoundariesConverter_FunctionTest()
        {
            var c10 = new NumericBoundariesConverter<sbyte>((i) => (sbyte)-i) { Culture = CultureInfo.InvariantCulture };
            c10.Set("123").Should().Be("123");
            c10.Get("123").Should().Be("-123");
            var c11 = new NumericBoundariesConverter<sbyte?>((i) => (sbyte?)-i) { Culture = CultureInfo.InvariantCulture };
            c11.Set("123").Should().Be("123");
            c11.Get("123").Should().Be("-123");
            c11.Set(null).Should().Be(null);
            c11.Get(null).Should().Be(null);
            var c12 = new NumericBoundariesConverter<byte>((i) => (byte)(i - 1)) { Culture = CultureInfo.InvariantCulture };
            c12.Set("234").Should().Be("234");
            c12.Get("234").Should().Be("233");
            var c13 = new NumericBoundariesConverter<byte?>((i) => (byte?)(i - 1)) { Culture = CultureInfo.InvariantCulture };
            c13.Set("234").Should().Be("234");
            c13.Get("234").Should().Be("233");
            c13.Set(null).Should().Be(null);
            c13.Get(null).Should().Be(null);
            var c14 = new NumericBoundariesConverter<short>((i) => (short)-i) { Culture = CultureInfo.InvariantCulture };
            c14.Set("1234").Should().Be("1234");
            c14.Get("1234").Should().Be("-1234");
            var c15 = new NumericBoundariesConverter<short?>((i) => (short?)-i) { Culture = CultureInfo.InvariantCulture };
            c15.Set("1234").Should().Be("1234");
            c15.Get("1234").Should().Be("-1234");
            c15.Set(null).Should().Be(null);
            c15.Get(null).Should().Be(null);
            var c16 = new NumericBoundariesConverter<ushort>((i) => (ushort)(i - 1)) { Culture = CultureInfo.InvariantCulture };
            c16.Set("12345").Should().Be("12345");
            c16.Get("12345").Should().Be("12344");
            var c17 = new NumericBoundariesConverter<ushort?>((i) => (ushort?)(i - 1)) { Culture = CultureInfo.InvariantCulture };
            c17.Set("12345").Should().Be("12345");
            c17.Get("12345").Should().Be("12344");
            c17.Set(null).Should().Be(null);
            c17.Get(null).Should().Be(null);
            var c18 = new NumericBoundariesConverter<int>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c18.Set("34567").Should().Be("34567");
            c18.Get("34567").Should().Be("-34567");
            var c19 = new NumericBoundariesConverter<int?>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c19.Set("34567").Should().Be("34567");
            c19.Get("34567").Should().Be("-34567");
            c19.Set(null).Should().Be(null);
            c19.Get(null).Should().Be(null);
            var c20 = new NumericBoundariesConverter<uint>((i) => i - 1) { Culture = CultureInfo.InvariantCulture };
            c20.Set("45678").Should().Be("45678");
            c20.Get("45678").Should().Be("45677");
            var c21 = new NumericBoundariesConverter<uint?>((i) => i - 1) { Culture = CultureInfo.InvariantCulture };
            c21.Set("45678").Should().Be("45678");
            c21.Get("45678").Should().Be("45677");
            c21.Set(null).Should().Be(null);
            c21.Get(null).Should().Be(null);
            var c22 = new NumericBoundariesConverter<long>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c22.Set("456789").Should().Be("456789");
            c22.Get("456789").Should().Be("-456789");
            var c23 = new NumericBoundariesConverter<long?>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c23.Set("456789").Should().Be("456789");
            c23.Get("456789").Should().Be("-456789");
            c23.Set(null).Should().Be(null);
            c23.Get(null).Should().Be(null);
            var c24 = new NumericBoundariesConverter<ulong>((i) => i - 1) { Culture = CultureInfo.InvariantCulture };
            c24.Set("4567890").Should().Be("4567890");
            c24.Get("4567890").Should().Be("4567889");
            var c25 = new NumericBoundariesConverter<ulong?>((i) => i - 1) { Culture = CultureInfo.InvariantCulture };
            c25.Set("4567890").Should().Be("4567890");
            c25.Get("4567890").Should().Be("4567889");
            c25.Set(null).Should().Be(null);
            c25.Get(null).Should().Be(null);
            var c26 = new NumericBoundariesConverter<float>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c26.Set("456789").Should().Be("456789");
            c26.Get("456789").Should().Be("-456789");
            var c27 = new NumericBoundariesConverter<float?>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c27.Set("45,678.9").Should().Be("45,678.9");
            c27.Get("45,678.9").Should().Be("-45678.9");
            c27.Set(null).Should().Be(null);
            c27.Get(null).Should().Be(null);
            var c28 = new NumericBoundariesConverter<double>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c28.Set("45,678.9").Should().Be("45,678.9");
            c28.Get("45678.9").Should().Be("-45678.9");
            var c29 = new NumericBoundariesConverter<double?>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c29.Set("45,678.9").Should().Be("45,678.9");
            c29.Get("45,678.9").Should().Be("-45678.9");
            c29.Set(null).Should().Be(null);
            c29.Get(null).Should().Be(null);
            var c30 = new NumericBoundariesConverter<decimal>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c30.Set("45,678.9").Should().Be("45,678.9");
            c30.Get("45678.9").Should().Be("-45678.9");
            var c31 = new NumericBoundariesConverter<decimal?>((i) => -i) { Culture = CultureInfo.InvariantCulture };
            c31.Set("45,678.9").Should().Be("45,678.9");
            c31.Get("45,678.9").Should().Be("-45678.9");
            c31.Set(null).Should().Be(null);
            c31.Get(null).Should().Be(null);
        }

        [Test]
        public void NumericBoundariesConverter_FormatTest()
        {
            var c1 = new NumericBoundariesConverter<double>((i) => i)
            {
                Culture = CultureInfo.InvariantCulture,
                Format = "F3"
            };
            c1.Set("1,234,567.15").Should().Be("1,234,567.15");
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get("1,234,567.15").Should().Be("1234567.150");
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
            c1.Get(Math.PI.ToString(c1.Culture)).Should().Be(Math.PI.ToString("F3", c1.Culture));
            c1.GetError.Should().Be(false);
            c1.GetErrorMessage.Should().BeNull();
        }

        [Test]
        public void DefaultConverterOverrideTest()
        {
            var conv = new MyTestConverter();
            conv.Set(null).Should().Be("nada");
            conv.Get("nada").Should().Be(null);
            conv.Set(18).Should().Be("18");
            conv.Get("18").Should().Be(18);
        }

        // a custom converter used only in test cases
        private class MyTestConverter : DefaultConverter<int?>
        {
            protected override int? ConvertFromString(string value)
            {
                if (value == "nada")
                    return null;
                return base.ConvertFromString(value);
            }

            protected override string ConvertToString(int? arg)
            {
                if (arg == null)
                    return "nada";
                return base.ConvertToString(arg);
            }
        }
    }
}
