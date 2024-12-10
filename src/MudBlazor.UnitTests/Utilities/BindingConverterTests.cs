// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class BindingConverterTests
    {

        [Test]
        public void GlobalConverterTests()
        {
            var c10 = new DefaultConverter<Point>();
            DefaultConverter<Point>.GlobalGetFunc = x => $"[{x.X},{x.Y}]";
            DefaultConverter<Point>.GlobalSetFunc = x => { var tmp = JsonSerializer.Deserialize<int[]>(x); return new Point(tmp[0], tmp[1]); };

            c10.Set(new Point(1, 2)).Should().Be("[1,2]");
            c10.Get("[1,2]").Should().Be(new Point(1, 2));
        }
        [Test]
        public void GlobalConverterTestsErrorHandling()
        {
            var c10 = new DefaultConverter<Point>();
            DefaultConverter<Point>.GlobalSetFunc = x => { var tmp = JsonSerializer.Deserialize<int[]>(x); return new Point(tmp[0], tmp[1]); };

            c10.Get("[1,2").Should().Be(Point.Empty);
            c10.GetErrorMessage.Should().Be("Not a valid Point");
        }

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
            // boolean format exception
            string booleanErrorMessage = "Not a valid boolean";
            var b1 = new DefaultConverter<bool>();
            b1.Get("a-z").Should().Be(default);
            b1.GetErrorMessage.Should().Be(booleanErrorMessage);
            var bn1 = new DefaultConverter<bool?>();
            bn1.Get("a-z").Should().Be(default);
            bn1.GetErrorMessage.Should().Be(booleanErrorMessage);

            // datetime format exception
            string dtErrorMessage = "Not a valid date time";
            var dt1 = new DefaultConverter<DateTime>();
            dt1.Get("12/34/56").Should().Be(default);
            dt1.GetErrorMessage.Should().Be(dtErrorMessage);
            var dtn1 = new DefaultConverter<DateTime?>();
            dtn1.Get("12/34/56").Should().Be(null);
            dtn1.GetErrorMessage.Should().Be(dtErrorMessage);

            // timespan format exception
            string tmErrorMessage = "Not a valid time span";
            var tm1 = new DefaultConverter<TimeSpan>();
            tm1.Get("12:o1").Should().Be(default);
            tm1.GetErrorMessage.Should().Be(tmErrorMessage);
            var tmn1 = new DefaultConverter<TimeSpan?>();
            tmn1.Get("12:o1").Should().Be(null);
            tmn1.GetErrorMessage.Should().Be(tmErrorMessage);

            // timespan overflow exception
            var tm2 = new DefaultConverter<TimeSpan>();
            tm2.Get("25:00").Should().Be(default);
            tm2.GetErrorMessage.Should().Be(tmErrorMessage);
            var tmn2 = new DefaultConverter<TimeSpan?>();
            tmn2.Get("25:00").Should().Be(null);
            tmn2.GetErrorMessage.Should().Be(tmErrorMessage);

            // not a valid number
            string numberErrorMessage = "Not a valid number";
            var c1 = new DefaultConverter<sbyte>();
            c1.Get("a-z").Should().Be(default);
            c1.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn1 = new DefaultConverter<sbyte?>();
            cn1.Get("a-z").Should().Be(null);
            cn1.GetErrorMessage.Should().Be(numberErrorMessage);
            var c2 = new DefaultConverter<byte>();
            c2.Get("a-z").Should().Be(default);
            c2.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn2 = new DefaultConverter<byte?>();
            cn2.Get("a-z").Should().Be(null);
            cn2.GetErrorMessage.Should().Be(numberErrorMessage);
            var c3 = new DefaultConverter<short>();
            c3.Get("a-z").Should().Be(default);
            c3.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn3 = new DefaultConverter<short?>();
            cn3.Get("a-z").Should().Be(null);
            cn3.GetErrorMessage.Should().Be(numberErrorMessage);
            var c4 = new DefaultConverter<ushort>();
            c4.Get("a-z").Should().Be(default);
            c4.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn4 = new DefaultConverter<ushort?>();
            cn4.Get("a-z").Should().Be(null);
            cn4.GetErrorMessage.Should().Be(numberErrorMessage);
            var c5 = new DefaultConverter<int>();
            c5.Get("a-z").Should().Be(default);
            c5.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn5 = new DefaultConverter<int?>();
            cn5.Get("a-z").Should().Be(null);
            cn5.GetErrorMessage.Should().Be(numberErrorMessage);
            var c6 = new DefaultConverter<uint>();
            c6.Get("a-z").Should().Be(default);
            c6.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn6 = new DefaultConverter<uint?>();
            cn6.Get("a-z").Should().Be(null);
            cn6.GetErrorMessage.Should().Be(numberErrorMessage);
            var c7 = new DefaultConverter<long>();
            c7.Get("a-z").Should().Be(default);
            c7.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn7 = new DefaultConverter<long?>();
            cn7.Get("a-z").Should().Be(null);
            cn7.GetErrorMessage.Should().Be(numberErrorMessage);
            var c8 = new DefaultConverter<ulong>();
            c8.Get("a-z").Should().Be(default);
            c8.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn8 = new DefaultConverter<ulong?>();
            cn8.Get("a-z").Should().Be(null);
            cn8.GetErrorMessage.Should().Be(numberErrorMessage);
            var c9 = new DefaultConverter<float>();
            c9.Get("a-z").Should().Be(default);
            c9.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn9 = new DefaultConverter<float?>();
            cn9.Get("a-z").Should().Be(null);
            cn9.GetErrorMessage.Should().Be(numberErrorMessage);
            var c10 = new DefaultConverter<double>();
            c10.Get("a-z").Should().Be(default);
            c10.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn10 = new DefaultConverter<double?>();
            cn10.Get("a-z").Should().Be(null);
            cn10.GetErrorMessage.Should().Be(numberErrorMessage);
            var c11 = new DefaultConverter<decimal>();
            c11.Get("a-z").Should().Be(default);
            c11.GetErrorMessage.Should().Be(numberErrorMessage);
            var cn11 = new DefaultConverter<decimal?>();
            cn11.Get("a-z").Should().Be(null);
            cn11.GetErrorMessage.Should().Be(numberErrorMessage);

            // not a valid GUID
            string guidErrorMessage = "Not a valid GUID";
            var c12 = new DefaultConverter<Guid>();
            c12.Get("a-z").Should().Be(Guid.Empty);
            c12.GetErrorMessage.Should().Be(guidErrorMessage);
            var cn12 = new DefaultConverter<Guid?>();
            cn12.Get("a-z").Should().Be(null);
            cn12.GetErrorMessage.Should().Be(guidErrorMessage);

            // not a valid enum
            string enumErrorMessage = $"Not a value of {typeof(YesNoMaybe).Name}";
            var c13 = new DefaultConverter<YesNoMaybe>();
            c13.Get("a-z").Should().Be(YesNoMaybe.Maybe);
            c13.GetErrorMessage.Should().Be(enumErrorMessage);
            var cn13 = new DefaultConverter<YesNoMaybe?>();
            cn13.Get("a-z").Should().Be(null);
            cn13.GetErrorMessage.Should().Be(enumErrorMessage);

            // invalid format for type supplied
            var c16 = new DefaultConverter<int?>();
            c16.Format = "dd/mm/yy";
            c16.Get(c16.Set(22)).Should().Be(null);
            c16.GetErrorMessage.Should().Be(numberErrorMessage);

            // invalid type
            DefaultConverter<object>.GlobalSetFunc = (input) =>
            {
                throw new FormatException("Invalid input.");
            };
            var c17 = new DefaultConverter<object>();
            c17.Get("a-z").Should().Be(null);
            c17.GetErrorMessage.Should().Be($"Not a valid {typeof(object).Name}");
            DefaultConverter<object>.GlobalSetFunc = null;

            // no conversion
            var c18 = new DefaultConverter<object>();
            c18.Get("a-z").Should().Be(null);
            c18.GetErrorMessage.Should().Be($"Conversion to type {typeof(object)} not implemented");

            string c19Exception = "test";
            var c19 = new DefaultConverter<object>();
            c19.OnError = (msg) =>
            {
                // The goal is to trigger the catch block in DefaultConverter without propagating the exception to the Converter.Get method.
                if (msg == $"Conversion to type System.Object not implemented")
                {
                    throw new Exception(c19Exception);
                }
            };
            c19.Get("a-z").Should().Be(null);
            c19.GetErrorMessage.Should().Be($"Conversion error: {c19Exception}");

            var c20Exception = "Invalid input.";
            DefaultConverter<object>.GlobalGetFunc = (input) =>
            {
                throw new FormatException(c20Exception);
            };
            var c20ErrorMessage = $"Conversion error: {c20Exception}";
            var c20 = new DefaultConverter<object>();
            c20.Set("a-z").Should().Be(null);
            c20.SetErrorMessage.Should().Be(c20ErrorMessage);
            DefaultConverter<object>.GlobalGetFunc = null;

            var c21Exception = "Conversion failed.";
            var c21 = new Converter<object>
            {
                GetFunc = (value) => { throw new Exception(c21Exception); },
                SetFunc = (value) => { throw new Exception(c21Exception); }
            };
            c21.Get("a-z").Should().Be(null);
            c21.GetErrorMessage.Should().Be($"Conversion from {typeof(string).Name} to {typeof(object).Name} failed: {c21Exception}");
            c21.Set("a-z").Should().Be(null);
            c21.SetErrorMessage.Should().Be($"Conversion from {typeof(object).Name} to {typeof(string).Name} failed: {c21Exception}");
        }

        [Test]
        public void ErrorCheckingWithInternalMudLocalizerTest()
        {
            var defaultLocalizationInterceptor = new Mock<AbstractLocalizationInterceptor>(NullLoggerFactory.Instance, null);
            var internalMudLocalizer = new InternalMudLocalizer(defaultLocalizationInterceptor.Object);

            // boolean format exception
            string booleanErrorMessage = LanguageResource.DefaultConverter_InvalidBoolean;
            var b1 = new DefaultConverter<bool>
            {
                Localizer = internalMudLocalizer
            };
            b1.Get("a-z").Should().Be(default);
            b1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(booleanErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var bn1 = new DefaultConverter<bool?>
            {
                Localizer = internalMudLocalizer
            };
            bn1.Get("a-z").Should().Be(default);
            bn1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(booleanErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            
            // datetime format exception
            string dtErrorMessage = LanguageResource.DefaultConverter_InvalidDateTime;
            var dt1 = new DefaultConverter<DateTime>
            {
                Localizer = internalMudLocalizer
            };
            dt1.Get("12/34/56").Should().Be(default);
            dt1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(dtErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var dtn1 = new DefaultConverter<DateTime?>
            {
                Localizer = internalMudLocalizer
            };
            dtn1.Get("12/34/56").Should().Be(null);
            dtn1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(dtErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // timespan format exception
            string tmErrorMessage = LanguageResource.DefaultConverter_InvalidTimeSpan;
            var tm1 = new DefaultConverter<TimeSpan>
            {
                Localizer = internalMudLocalizer
            };
            tm1.Get("12:o1").Should().Be(default);
            tm1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(tmErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var tmn1 = new DefaultConverter<TimeSpan?>
            {
                Localizer = internalMudLocalizer
            };
            tmn1.Get("12:o1").Should().Be(null);
            tmn1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(tmErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // timespan overflow exception
            var tm2 = new DefaultConverter<TimeSpan>
            {
                Localizer = internalMudLocalizer
            };
            tm2.Get("25:00").Should().Be(default);
            tm2.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(tmErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var tmn2 = new DefaultConverter<TimeSpan?>
            {
                Localizer = internalMudLocalizer
            };
            tmn2.Get("25:00").Should().Be(null);
            tmn2.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(tmErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // not a valid number
            string numberErrorMessage = LanguageResource.DefaultConverter_InvalidNumber;
            var c1 = new DefaultConverter<sbyte>
            {
                Localizer = internalMudLocalizer
            };
            c1.Get("a-z").Should().Be(default);
            c1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn1 = new DefaultConverter<sbyte?>
            {
                Localizer = internalMudLocalizer
            };
            cn1.Get("a-z").Should().Be(null);
            cn1.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c2 = new DefaultConverter<byte>
            {
                Localizer = internalMudLocalizer
            };
            c2.Get("a-z").Should().Be(default);
            c2.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn2 = new DefaultConverter<byte?>
            {
                Localizer = internalMudLocalizer
            };
            cn2.Get("a-z").Should().Be(null);
            cn2.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c3 = new DefaultConverter<short>
            {
                Localizer = internalMudLocalizer
            };
            c3.Get("a-z").Should().Be(default);
            c3.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn3 = new DefaultConverter<short?>
            {
                Localizer = internalMudLocalizer
            };
            cn3.Get("a-z").Should().Be(null);
            cn3.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c4 = new DefaultConverter<ushort>
            {
                Localizer = internalMudLocalizer
            };
            c4.Get("a-z").Should().Be(default);
            c4.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn4 = new DefaultConverter<ushort?>
            {
                Localizer = internalMudLocalizer
            };
            cn4.Get("a-z").Should().Be(null);
            cn4.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c5 = new DefaultConverter<int>
            {
                Localizer = internalMudLocalizer
            };
            c5.Get("a-z").Should().Be(default);
            c5.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn5 = new DefaultConverter<int?>
            {
                Localizer = internalMudLocalizer
            };
            cn5.Get("a-z").Should().Be(null);
            cn5.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c6 = new DefaultConverter<uint>
            {
                Localizer = internalMudLocalizer
            };
            c6.Get("a-z").Should().Be(default);
            c6.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn6 = new DefaultConverter<uint?>
            {
                Localizer = internalMudLocalizer
            };
            cn6.Get("a-z").Should().Be(null);
            cn6.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c7 = new DefaultConverter<long>
            {
                Localizer = internalMudLocalizer
            };
            c7.Get("a-z").Should().Be(default);
            c7.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn7 = new DefaultConverter<long?>
            {
                Localizer = internalMudLocalizer
            };
            cn7.Get("a-z").Should().Be(null);
            cn7.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c8 = new DefaultConverter<ulong>
            {
                Localizer = internalMudLocalizer
            };
            c8.Get("a-z").Should().Be(default);
            c8.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn8 = new DefaultConverter<ulong?>
            {
                Localizer = internalMudLocalizer
            };
            cn8.Get("a-z").Should().Be(null);
            cn8.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c9 = new DefaultConverter<float>
            {
                Localizer = internalMudLocalizer
            };
            c9.Get("a-z").Should().Be(default);
            c9.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn9 = new DefaultConverter<float?>
            {
                Localizer = internalMudLocalizer
            };
            cn9.Get("a-z").Should().Be(null);
            cn9.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c10 = new DefaultConverter<double>
            {
                Localizer = internalMudLocalizer
            };
            c10.Get("a-z").Should().Be(default);
            c10.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn10 = new DefaultConverter<double?>
            {
                Localizer = internalMudLocalizer
            };
            cn10.Get("a-z").Should().Be(null);
            cn10.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var c11 = new DefaultConverter<decimal>
            {
                Localizer = internalMudLocalizer
            };
            c11.Get("a-z").Should().Be(default);
            c11.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn11 = new DefaultConverter<decimal?>
            {
                Localizer = internalMudLocalizer
            };
            cn11.Get("a-z").Should().Be(null);
            cn11.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // not a valid GUID
            string guidErrorMessage = LanguageResource.DefaultConverter_InvalidGUID;
            var c12 = new DefaultConverter<Guid>
            {
                Localizer = internalMudLocalizer
            };
            c12.Get("a-z").Should().Be(Guid.Empty);
            c12.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(guidErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn12 = new DefaultConverter<Guid?>
            {
                Localizer = internalMudLocalizer
            };
            cn12.Get("a-z").Should().Be(null);
            cn12.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(guidErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // not a valid enum
            string enumErrorMessage = LanguageResource.DefaultConverter_NotValueOf;
            var c13 = new DefaultConverter<YesNoMaybe>
            {
                Localizer = internalMudLocalizer
            };
            c13.Get("a-z").Should().Be(YesNoMaybe.Maybe);
            c13.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(enumErrorMessage, typeof(YesNoMaybe).Name), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            var cn13 = new DefaultConverter<YesNoMaybe?>
            {
                Localizer = internalMudLocalizer
            };
            cn13.Get("a-z").Should().Be(null);
            cn13.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(enumErrorMessage, typeof(YesNoMaybe).Name), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // invalid format for type supplied
            var c16 = new DefaultConverter<int?>
            {
                Localizer = internalMudLocalizer
            };
            c16.Format = "dd/mm/yy";
            c16.Get(c16.Set(22)).Should().Be(null);
            c16.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(numberErrorMessage, It.IsAny<object[]>()), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            // invalid type
            DefaultConverter<object>.GlobalSetFunc = (input) =>
            {
                throw new FormatException("Invalid input.");
            };
            var c17 = new DefaultConverter<object>
            {
                Localizer = internalMudLocalizer
            };
            c17.Get("a-z").Should().Be(null);
            c17.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(LanguageResource.DefaultConverter_InvalidType, typeof(object).Name), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            DefaultConverter<object>.GlobalSetFunc = null;

            // no conversion
            var c18 = new DefaultConverter<object>
            {
                Localizer = internalMudLocalizer
            };
            c18.Get("a-z").Should().Be(null);
            c18.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(LanguageResource.DefaultConverter_ConversionNotImplemented, typeof(object)), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();

            var c20Exception = "Invalid input.";
            DefaultConverter<object>.GlobalGetFunc = (input) =>
            {
                throw new FormatException(c20Exception);
            };
            var c20 = new DefaultConverter<object>
            {
                Localizer = internalMudLocalizer
            };
            c20.Set("a-z").Should().Be(null);
            c20.SetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(LanguageResource.DefaultConverter_ConversionError, c20Exception), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            DefaultConverter<object>.GlobalGetFunc = null;

            var c21Exception = "Conversion failed.";
            var c21 = new Converter<object>
            {
                Localizer = internalMudLocalizer,
                GetFunc = (value) => { throw new Exception(c21Exception); },
                SetFunc = (value) => { throw new Exception(c21Exception); }
            };
            c21.Get("a-z").Should().Be(null);
            c21.GetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(LanguageResource.Converter_ConversionFailed, new object[] { typeof(string).Name, typeof(object).Name, c21Exception }), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
            c21.Set("a-z").Should().Be(null);
            c21.SetErrorMessage.Should().NotBeEmpty();
            defaultLocalizationInterceptor.Verify(_ => _.Handle(LanguageResource.Converter_ConversionFailed, new object[] { typeof(object).Name, typeof(string).Name, c21Exception }), Times.Once);
            defaultLocalizationInterceptor.Invocations.Clear();
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
