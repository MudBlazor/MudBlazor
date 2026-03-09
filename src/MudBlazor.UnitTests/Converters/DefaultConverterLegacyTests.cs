// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using AwesomeAssertions;
using NUnit.Framework;

#nullable enable
namespace MudBlazor.UnitTests.Converters
{
    [TestFixture]
    public class DefaultConverterLegacyTests
    {
        [Test]
        public void DefaultConverter_String_ValidCases()
        {
            var c1 = new DefaultConverter<string>();
            c1.Convert("hello").Should().Be("hello");
            c1.ConvertBack("hello").Should().Be("hello");
            c1.Convert("").Should().Be("");
            c1.ConvertBack("").Should().Be("");
            c1.ConvertBack(null).Should().Be(null);
            c1.Convert(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Char_ValidCases()
        {
            var c1 = new DefaultConverter<char>();
            c1.Convert('x').Should().Be("x");
            c1.ConvertBack("a").Should().Be('a');
            c1.ConvertBack("").Should().Be('\0');
            c1.ConvertBack(null).Should().Be('\0');

            var c2 = new DefaultConverter<char?>();
            c2.Convert('x').Should().Be("x");
            c2.ConvertBack("a").Should().Be('a');
            c2.ConvertBack("").Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);
            c2.Convert(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Bool_ValidCases()
        {
            var c1 = new DefaultConverter<bool>();
            c1.Convert(true).Should().Be("True");
            c1.Convert(false).Should().Be("False");
            c1.ConvertBack("true").Should().Be(true);
            c1.ConvertBack("True").Should().Be(true);
            c1.ConvertBack("false").Should().Be(false);
            c1.ConvertBack("ON").Should().Be(true);
            c1.ConvertBack("off").Should().Be(false);
            c1.ConvertBack("").Should().Be(false);
            c1.ConvertBack("asdf").Should().Be(false);

            var c2 = new DefaultConverter<bool?>();
            c2.Convert(true).Should().Be("True");
            c2.ConvertBack("true").Should().Be(true);
            c2.Convert(false).Should().Be("False");
            c2.ConvertBack("false").Should().Be(false);
            c2.Convert(null).Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Numeric_ValidCases()
        {
            var c1 = new DefaultConverter<sbyte>();
            c1.Convert(123).Should().Be("123");
            c1.ConvertBack("123").Should().Be(123);

            var c2 = new DefaultConverter<sbyte?>();
            c2.Convert(123).Should().Be("123");
            c2.ConvertBack("123").Should().Be(123);
            c2.Convert(null).Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);

            var c3 = new DefaultConverter<byte>();
            c3.Convert(234).Should().Be("234");
            c3.ConvertBack("234").Should().Be(234);

            var c4 = new DefaultConverter<byte?>();
            c4.Convert(234).Should().Be("234");
            c4.ConvertBack("234").Should().Be(234);
            c4.Convert(null).Should().Be(null);
            c4.ConvertBack(null).Should().Be(null);

            var c5 = new DefaultConverter<short>();
            c5.Convert(1234).Should().Be("1234");
            c5.ConvertBack("1234").Should().Be(1234);

            var c6 = new DefaultConverter<short?>();
            c6.Convert(1234).Should().Be("1234");
            c6.ConvertBack("1234").Should().Be(1234);
            c6.Convert(null).Should().Be(null);
            c6.ConvertBack(null).Should().Be(null);

            var c7 = new DefaultConverter<ushort>();
            c7.Convert(12345).Should().Be("12345");
            c7.ConvertBack("12345").Should().Be(12345);

            var c8 = new DefaultConverter<ushort?>();
            c8.Convert(12345).Should().Be("12345");
            c8.ConvertBack("12345").Should().Be(12345);
            c8.Convert(null).Should().Be(null);
            c8.ConvertBack(null).Should().Be(null);

            var c9 = new DefaultConverter<int>();
            c9.Convert(34567).Should().Be("34567");
            c9.ConvertBack("34567").Should().Be(34567);

            var c10 = new DefaultConverter<int?>();
            c10.Convert(34567).Should().Be("34567");
            c10.ConvertBack("34567").Should().Be(34567);
            c10.Convert(null).Should().Be(null);
            c10.ConvertBack(null).Should().Be(null);

            var c11 = new DefaultConverter<uint>();
            c11.Convert(45678).Should().Be("45678");
            c11.ConvertBack("45678").Should().Be(45678);

            var c12 = new DefaultConverter<uint?>();
            c12.Convert(45678).Should().Be("45678");
            c12.ConvertBack("45678").Should().Be(45678);
            c12.Convert(null).Should().Be(null);
            c12.ConvertBack(null).Should().Be(null);

            var c13 = new DefaultConverter<long>();
            c13.Convert(456789).Should().Be("456789");
            c13.ConvertBack("456789").Should().Be(456789);

            var c14 = new DefaultConverter<long?>();
            c14.Convert(456789).Should().Be("456789");
            c14.ConvertBack("456789").Should().Be(456789);
            c14.Convert(null).Should().Be(null);
            c14.ConvertBack(null).Should().Be(null);

            var c15 = new DefaultConverter<ulong>();
            c15.Convert(4567890).Should().Be("4567890");
            c15.ConvertBack("4567890").Should().Be(4567890);

            var c16 = new DefaultConverter<ulong?>();
            c16.Convert(4567890).Should().Be("4567890");
            c16.ConvertBack("4567890").Should().Be(4567890);
            c16.Convert(null).Should().Be(null);
            c16.ConvertBack(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Numeric_Culture_AffectsConversion()
        {
            var c3 = new DefaultConverter<double?>() { Culture = () => CultureInfo.InvariantCulture };
            c3.Convert(1.7).Should().Be("1.7");
            c3.ConvertBack("1.7").Should().Be(1.7);
            c3.ConvertBack("1234567.15").Should().Be(1234567.15);
            c3.Convert(1234567.15).Should().Be("1234567.15");
            c3.Convert(c3.ConvertBack("1234567.15")).Should().Be("1234567.15");
            c3.ConvertBack(c3.Convert(1234567.15)).Should().Be(1234567.15);
            c3.Convert(null).Should().Be(null);
            c3.ConvertBack(null).Should().Be(null);
            c3.Culture = () => CultureInfo.GetCultureInfo("de-AT");
            c3.Convert(1.7).Should().Be("1,7");
            c3.ConvertBack("1,7").Should().Be(1.7);
        }

        [Test]
        public void DefaultConverter_Numeric_Format_AffectsConversion()
        {
            var float1 = new DefaultConverter<float> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            float1.Convert(1.7f).Should().Be("1.70");
            float1.Convert(1.773f).Should().Be("1.77");
            float1.ConvertBack("1.773").Should().Be(1.773f);
            float1.ConvertBack("1.77").Should().Be(1.77f);
            float1.ConvertBack("1.7").Should().Be(1.7f);
            float1.Culture = () => new CultureInfo("pt-BR", false);
            float1.Convert(1.7f).Should().Be("1,70");
            float1.Convert(1.773f).Should().Be("1,77");
            float1.ConvertBack("1,773").Should().Be(1.773f);
            float1.ConvertBack("1,77").Should().Be(1.77f);
            float1.ConvertBack("1,7").Should().Be(1.7f);

            var float2 = new DefaultConverter<float?> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            float2.Convert(1.7f).Should().Be("1.70");
            float2.Convert(1.773f).Should().Be("1.77");
            float2.Convert(null).Should().Be(null);
            float2.ConvertBack("1.773").Should().Be(1.773f);
            float2.ConvertBack("1.77").Should().Be(1.77f);
            float2.ConvertBack("1.7").Should().Be(1.7f);
            float2.ConvertBack(null).Should().Be(null);
            float2.Culture = () => new CultureInfo("pt-BR", false);
            float2.Convert(1.7f).Should().Be("1,70");
            float2.Convert(1.773f).Should().Be("1,77");
            float2.ConvertBack("1,773").Should().Be(1.773f);
            float2.ConvertBack("1,77").Should().Be(1.77f);
            float2.ConvertBack("1,7").Should().Be(1.7f);

            var dbl1 = new DefaultConverter<double> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            dbl1.Convert(1.7d).Should().Be("1.70");
            dbl1.Convert(1.773d).Should().Be("1.77");
            dbl1.ConvertBack("1.773").Should().Be(1.773d);
            dbl1.ConvertBack("1.77").Should().Be(1.77d);
            dbl1.ConvertBack("1.7").Should().Be(1.7d);
            dbl1.Culture = () => new CultureInfo("pt-BR", false);
            dbl1.Convert(1.7d).Should().Be("1,70");
            dbl1.Convert(1.773d).Should().Be("1,77");
            dbl1.ConvertBack("1,773").Should().Be(1.773d);
            dbl1.ConvertBack("1,77").Should().Be(1.77d);
            dbl1.ConvertBack("1,7").Should().Be(1.7d);

            var dbl2 = new DefaultConverter<double?> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            dbl2.Convert(1.7d).Should().Be("1.70");
            dbl2.Convert(1.773d).Should().Be("1.77");
            dbl2.Convert(null).Should().Be(null);
            dbl2.ConvertBack("1.773").Should().Be(1.773d);
            dbl2.ConvertBack("1.77").Should().Be(1.77d);
            dbl2.ConvertBack("1.7").Should().Be(1.7d);
            dbl2.ConvertBack(null).Should().Be(null);
            dbl2.Culture = () => new CultureInfo("pt-BR", false);
            dbl2.Convert(1.7d).Should().Be("1,70");
            dbl2.Convert(1.773d).Should().Be("1,77");
            dbl2.ConvertBack("1,773").Should().Be(1.773d);
            dbl2.ConvertBack("1,77").Should().Be(1.77d);
            dbl2.ConvertBack("1,7").Should().Be(1.7d);

            var dec1 = new DefaultConverter<decimal> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            dec1.Convert(1.7m).Should().Be("1.70");
            dec1.Convert(1.773m).Should().Be("1.77");
            dec1.ConvertBack("1.773").Should().Be(1.773m);
            dec1.ConvertBack("1.77").Should().Be(1.77m);
            dec1.ConvertBack("1.7").Should().Be(1.7m);
            dec1.Culture = () => new CultureInfo("pt-BR", false);
            dec1.Convert(1.7m).Should().Be("1,70");
            dec1.Convert(1.773m).Should().Be("1,77");
            dec1.ConvertBack("1,773").Should().Be(1.773m);
            dec1.ConvertBack("1,77").Should().Be(1.77m);
            dec1.ConvertBack("1,7").Should().Be(1.7m);

            var dec2 = new DefaultConverter<decimal?> { Format = () => "0.00", Culture = () => new CultureInfo("en-US", false) };
            dec2.Convert(1.7m).Should().Be("1.70");
            dec2.Convert(1.773m).Should().Be("1.77");
            dec2.Convert(null).Should().Be(null);
            dec2.ConvertBack("1.773").Should().Be(1.773m);
            dec2.ConvertBack("1.77").Should().Be(1.77m);
            dec2.ConvertBack("1.7").Should().Be(1.7m);
            dec2.ConvertBack(null).Should().Be(null);
            dec2.Culture = () => new CultureInfo("pt-BR", false);
            dec2.Convert(1.7m).Should().Be("1,70");
            dec2.Convert(1.773m).Should().Be("1,77");
            dec2.ConvertBack("1,773").Should().Be(1.773m);
            dec2.ConvertBack("1,77").Should().Be(1.77m);
            dec2.ConvertBack("1,7").Should().Be(1.7m);
        }

        [Test]
        public void DefaultConverter_Guid_ValidCases()
        {
            var c1 = new DefaultConverter<Guid>();
            var guid = Guid.NewGuid();
            c1.Convert(guid).Should().Be(guid.ToString());
            c1.ConvertBack(guid.ToString()).Should().Be(guid);
            c1.ConvertBack("").Should().Be(Guid.Empty);
            c1.ConvertBack(null).Should().Be(Guid.Empty);

            var c2 = new DefaultConverter<Guid?>();
            Guid? guid2 = Guid.NewGuid();
            c2.Convert(guid2).Should().Be(guid2.ToString());
            c2.Convert(null).Should().Be(null);
            c2.ConvertBack(guid2.ToString()).Should().Be(guid2);
            c2.ConvertBack("").Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Enum_ValidCases()
        {
            var c1 = new DefaultConverter<ButtonType>();
            c1.Convert(ButtonType.Button).Should().Be("Button");
            c1.ConvertBack("Button").Should().Be(ButtonType.Button);

            var c2 = new DefaultConverter<YesNoMaybe>();
            c2.Convert(YesNoMaybe.Yes).Should().Be("Yes");
            c2.ConvertBack("No").Should().Be(YesNoMaybe.No);
            c2.ConvertBack(string.Empty).Should().Be(default);
            c2.ConvertBack(null).Should().Be(default);

            var c3 = new DefaultConverter<YesNoMaybe?>();
            c3.Convert(YesNoMaybe.Maybe).Should().Be("Maybe");
            c3.ConvertBack("Maybe").Should().Be(YesNoMaybe.Maybe);
            c3.ConvertBack(string.Empty).Should().Be(null);
            c3.ConvertBack(null).Should().Be(null);
            c3.Convert(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_DateTime_ValidCases()
        {
            var c1 = new DefaultConverter<DateTime>();
            var date = DateTime.Today;
            c1.ConvertBack(c1.Convert(date)).Should().Be(date);

            var c2 = new DefaultConverter<DateTime?>();
            var date2 = DateTime.Today;
            c2.ConvertBack(c2.Convert(date2)).Should().Be(date2);
            c2.Convert(null).Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_DateTime_Format_AffectsConversion()
        {
            var dt1 = new DefaultConverter<DateTime> { Format = () => "MM/dd/yyyy", Culture = () => new CultureInfo("en-US", false) };
            dt1.Convert(new DateTime(2020, 11, 03)).Should().Be("11/03/2020");
            dt1.ConvertBack("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt1.Culture = () => new CultureInfo("pt-BR", false);
            dt1.Format = () => "dd/MM/yyyy";
            dt1.Convert(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt1.ConvertBack("03/11/2020").Should().Be(new DateTime(2020, 11, 03));

            var dt2 = new DefaultConverter<DateTime?> { Format = () => "MM/dd/yyyy", Culture = () => new CultureInfo("en-US", false) };
            dt2.Convert(new DateTime(2020, 11, 03)).Should().Be("11/03/2020");
            dt2.Convert(null).Should().Be(null);
            dt2.ConvertBack("11/03/2020").Should().Be(new DateTime(2020, 11, 03));
            dt2.ConvertBack(null).Should().Be(null);
            dt2.Culture = () => new CultureInfo("pt-BR", false);
            dt2.Format = () => "dd/MM/yyyy";
            dt2.Convert(new DateTime(2020, 11, 03)).Should().Be("03/11/2020");
            dt2.ConvertBack("03/11/2020").Should().Be(new DateTime(2020, 11, 03));
        }

        [Test]
        public void DefaultConverter_TimeSpan_ValidCases()
        {
            var c1 = new DefaultConverter<TimeSpan>();
            var time = DateTime.Now.TimeOfDay;
            c1.ConvertBack(c1.Convert(time)).Should().Be(time);

            var c2 = new DefaultConverter<TimeSpan?>();
            var time2 = DateTime.Now.TimeOfDay;
            c2.ConvertBack(c2.Convert(time2)).Should().Be(time2);
            c2.Convert(null).Should().Be(null);
            c2.ConvertBack(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_TimeSpan_Format_AffectsConversion()
        {
            var converter = new DefaultConverter<TimeSpan>();
            var time = new TimeSpan(1, 2, 3);

            converter.Format = () => @"hh\:mm";
            converter.Convert(time).Should().Be("01:02");
            converter.ConvertBack("01:02").Should().Be(new TimeSpan(1, 2, 0));

            converter.Format = () => @"hh\:mm\:ss\.fff";
            var preciseTime = new TimeSpan(0, 1, 2, 3, 456);
            converter.Convert(preciseTime).Should().Be("01:02:03.456");
            converter.ConvertBack("01:02:03.456").Should().Be(preciseTime);
        }

        [Test]
        public void DefaultConverter_NotImplementedType_ValidCases()
        {
            var notImplementedType = new object();

            var c1 = new DefaultConverter<object>();
            c1.Convert(notImplementedType).Should().Be(notImplementedType.ToString());
        }

        private enum YesNoMaybe { Maybe, Yes, No }
    }
}
