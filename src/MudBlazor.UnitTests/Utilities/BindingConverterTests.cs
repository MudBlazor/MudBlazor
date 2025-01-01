// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class BindingConverterTests
    {
        [Test]
        public void BaseConverter_ErrorCases()
        {
            string errorMessage = LanguageResource.Converter_ConversionFailed;

            // Test null
            var c1 = new Converter<object, string>
            {
                GetFunc = null,
                SetFunc = null
            };
            c1.Get("a-z").Should().Be(default(object));
            c1.Set("a-z").Should().Be(default(string));

            // Test invalid format
            var exception = "Conversion failed.";
            var c2 = new Converter<object, string>
            {
                GetFunc = (value) => { throw new FormatException(exception); },
                SetFunc = (value) => { throw new FormatException(exception); }
            };
            c2.Get("a-z").Should().Be(null);
            c2.GetError.Should().BeTrue();
            c2.GetErrorMessage.HasValue.Should().BeTrue();
            c2.GetErrorMessage.Value.Item1.Should().Be(errorMessage);
            c2.GetErrorMessage.Value.Item2.Should().BeEquivalentTo([typeof(string).Name, typeof(object).Name, exception]);

            c2.Set("a-z").Should().Be(null);
            c2.SetError.Should().BeTrue();
            c2.SetErrorMessage.HasValue.Should().BeTrue();
            c2.SetErrorMessage.Value.Item1.Should().Be(errorMessage);
            c2.SetErrorMessage.Value.Item2.Should().BeEquivalentTo([typeof(object).Name, typeof(string).Name, exception]);
        }

        [Test]
        public void DefaultConverter_GlobalFunc_ValidCases()
        {
            var c1 = new DefaultConverter<Point>();
            DefaultConverter<Point>.GlobalGetFunc = x => $"[{x.X},{x.Y}]";
            DefaultConverter<Point>.GlobalSetFunc = x => { var tmp = JsonSerializer.Deserialize<int[]>(x); return new Point(tmp[0], tmp[1]); };

            c1.Set(new Point(1, 2)).Should().Be("[1,2]");
            c1.Get("[1,2]").Should().Be(new Point(1, 2));
        }

        [Test]
        public void DefaultConverter_GlobalFunc_ErrorCases()
        {
            string errorMessage = LanguageResource.Converter_InvalidType;
            object[] errorArgs = [typeof(Point).Name];

            var c1 = new DefaultConverter<Point>();
            DefaultConverter<Point>.GlobalSetFunc = x => { var tmp = JsonSerializer.Deserialize<int[]>(x); return new Point(tmp[0], tmp[1]); };

            c1.Get("[1,2").Should().Be(Point.Empty);
            c1.GetError.Should().BeTrue();
            c1.GetErrorMessage.HasValue.Should().BeTrue();
            c1.GetErrorMessage.Value.Item1.Should().Be(errorMessage);
            c1.GetErrorMessage.Value.Item2.Should().BeEquivalentTo(errorArgs);
        }

        [Test]
        public void DefaultConverter_String_ValidCases()
        {
            var c1 = new DefaultConverter<string>();
            c1.Set("hello").Should().Be("hello");
            c1.Get("hello").Should().Be("hello");
            c1.Set("").Should().Be("");
            c1.Get("").Should().Be("");
            c1.Get(null).Should().Be(null);
            c1.Set(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Char_ValidCases()
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
        }

        [Test]
        public void DefaultConverter_Bool_ValidCases()
        {
            var c1 = new DefaultConverter<bool>();
            c1.Set(true).Should().Be("True");
            c1.Set(false).Should().Be("False");
            c1.Get("true").Should().Be(true);
            c1.Get("True").Should().Be(true);
            c1.Get("false").Should().Be(false);
            c1.Get("ON").Should().Be(true);
            c1.Get("off").Should().Be(false);
            c1.Get("").Should().Be(false);
            c1.Get("asdf").Should().Be(false);

            var c2 = new DefaultConverter<bool?>();
            c2.Set(true).Should().Be("True");
            c2.Get("true").Should().Be(true);
            c2.Set(false).Should().Be("False");
            c2.Get("false").Should().Be(false);
            c2.Set(null).Should().Be(null);
            c2.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Bool_ErrorCases()
        {
            var invalidInput = "a-z";
            var booleanErrorMessage = LanguageResource.Converter_InvalidBoolean;

            // Test invalid format for bool
            TestInvalidFormat<bool>(invalidInput, booleanErrorMessage);

            // Test invalid format for nullable bool
            TestInvalidFormat<bool?>(invalidInput, booleanErrorMessage);
        }

        [Test]
        public void DefaultConverter_Numeric_ValidCases()
        {
            var c1 = new DefaultConverter<sbyte>();
            c1.Set(123).Should().Be("123");
            c1.Get("123").Should().Be(123);

            var c2 = new DefaultConverter<sbyte?>();
            c2.Set(123).Should().Be("123");
            c2.Get("123").Should().Be(123);
            c2.Set(null).Should().Be(null);
            c2.Get(null).Should().Be(null);

            var c3 = new DefaultConverter<byte>();
            c3.Set(234).Should().Be("234");
            c3.Get("234").Should().Be(234);

            var c4 = new DefaultConverter<byte?>();
            c4.Set(234).Should().Be("234");
            c4.Get("234").Should().Be(234);
            c4.Set(null).Should().Be(null);
            c4.Get(null).Should().Be(null);

            var c5 = new DefaultConverter<short>();
            c5.Set(1234).Should().Be("1234");
            c5.Get("1234").Should().Be(1234);

            var c6 = new DefaultConverter<short?>();
            c6.Set(1234).Should().Be("1234");
            c6.Get("1234").Should().Be(1234);
            c6.Set(null).Should().Be(null);
            c6.Get(null).Should().Be(null);

            var c7 = new DefaultConverter<ushort>();
            c7.Set(12345).Should().Be("12345");
            c7.Get("12345").Should().Be(12345);

            var c8 = new DefaultConverter<ushort?>();
            c8.Set(12345).Should().Be("12345");
            c8.Get("12345").Should().Be(12345);
            c8.Set(null).Should().Be(null);
            c8.Get(null).Should().Be(null);

            var c9 = new DefaultConverter<int>();
            c9.Set(34567).Should().Be("34567");
            c9.Get("34567").Should().Be(34567);

            var c10 = new DefaultConverter<int?>();
            c10.Set(34567).Should().Be("34567");
            c10.Get("34567").Should().Be(34567);
            c10.Set(null).Should().Be(null);
            c10.Get(null).Should().Be(null);

            var c11 = new DefaultConverter<uint>();
            c11.Set(45678).Should().Be("45678");
            c11.Get("45678").Should().Be(45678);

            var c12 = new DefaultConverter<uint?>();
            c12.Set(45678).Should().Be("45678");
            c12.Get("45678").Should().Be(45678);
            c12.Set(null).Should().Be(null);
            c12.Get(null).Should().Be(null);

            var c13 = new DefaultConverter<long>();
            c13.Set(456789).Should().Be("456789");
            c13.Get("456789").Should().Be(456789);

            var c14 = new DefaultConverter<long?>();
            c14.Set(456789).Should().Be("456789");
            c14.Get("456789").Should().Be(456789);
            c14.Set(null).Should().Be(null);
            c14.Get(null).Should().Be(null);

            var c15 = new DefaultConverter<ulong>();
            c15.Set(4567890).Should().Be("4567890");
            c15.Get("4567890").Should().Be(4567890);

            var c16 = new DefaultConverter<ulong?>();
            c16.Set(4567890).Should().Be("4567890");
            c16.Get("4567890").Should().Be(4567890);
            c16.Set(null).Should().Be(null);
            c16.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Numeric_ErrorCases()
        {
            var invalidInput = "a-z";
            var numberErrorMessage = LanguageResource.Converter_InvalidNumber;

            // Test invalid format for various numeric types
            TestInvalidFormat<sbyte>(invalidInput, numberErrorMessage);
            TestInvalidFormat<sbyte?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<byte>(invalidInput, numberErrorMessage);
            TestInvalidFormat<byte?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<short>(invalidInput, numberErrorMessage);
            TestInvalidFormat<short?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<ushort>(invalidInput, numberErrorMessage);
            TestInvalidFormat<ushort?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<int>(invalidInput, numberErrorMessage);
            TestInvalidFormat<int?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<uint>(invalidInput, numberErrorMessage);
            TestInvalidFormat<uint?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<long>(invalidInput, numberErrorMessage);
            TestInvalidFormat<long?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<ulong>(invalidInput, numberErrorMessage);
            TestInvalidFormat<ulong?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<float>(invalidInput, numberErrorMessage);
            TestInvalidFormat<float?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<double>(invalidInput, numberErrorMessage);
            TestInvalidFormat<double?>(invalidInput, numberErrorMessage);
            TestInvalidFormat<decimal>(invalidInput, numberErrorMessage);
            TestInvalidFormat<decimal?>(invalidInput, numberErrorMessage);

            // invalid format for type supplied
            var c12 = new DefaultConverter<int?>();
            c12.Format = "dd/mm/yy";
            c12.Get(c12.Set(22)).Should().Be(null);
            c12.GetError.Should().BeTrue();
            c12.GetErrorMessage.HasValue.Should().BeTrue();
            c12.GetErrorMessage.Value.Item1.Should().Be(numberErrorMessage);
            c12.GetErrorMessage.Value.Item2.Should().BeEmpty();
        }

        [Test]
        public void DefaultConverter_Numeric_Culture_AffectsConversion()
        {
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
        }

        [Test]
        public void DefaultConverter_Numeric_Format_AffectsConversion()
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
        }

        [Test]
        public void DefaultConverter_Guid_ValidCases()
        {
            var c1 = new DefaultConverter<Guid>();
            var guid = Guid.NewGuid();
            c1.Set(guid).Should().Be(guid.ToString());
            c1.Get(guid.ToString()).Should().Be(guid);
            c1.Get("").Should().Be(Guid.Empty);
            c1.Get(null).Should().Be(Guid.Empty);

            var c2 = new DefaultConverter<Guid?>();
            Guid? guid2;
            guid2 = Guid.NewGuid();
            c2.Set(guid2).Should().Be(guid2.ToString());
            c2.Set(null).Should().Be(null);
            c2.Get(guid2.ToString()).Should().Be(guid2);
            c2.Get("").Should().Be(null);
            c2.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Guid_ErrorCases()
        {
            var invalidInput = "a-z";
            var guidErrorMessage = LanguageResource.Converter_InvalidGUID;

            // Test invalid format for Guid
            TestInvalidFormat<Guid>(invalidInput, guidErrorMessage);

            // Test invalid format for nullable Guid
            TestInvalidFormat<Guid?>(invalidInput, guidErrorMessage);
        }

        public enum YesNoMaybe { Maybe, Yes, No }

        [Test]
        public void DefaultConverter_Enum_ValidCases()
        {
            var c1 = new DefaultConverter<ButtonType>();
            c1.Set(ButtonType.Button).Should().Be("Button");
            c1.Get("Button").Should().Be(ButtonType.Button);

            var c2 = new DefaultConverter<YesNoMaybe>();
            c2.Set(YesNoMaybe.Yes).Should().Be("Yes");
            c2.Get("No").Should().Be(YesNoMaybe.No);
            c2.Get("").Should().Be(default(YesNoMaybe));
            c2.Get(null).Should().Be(default(YesNoMaybe));

            var c3 = new DefaultConverter<YesNoMaybe?>();
            c3.Set(YesNoMaybe.Maybe).Should().Be("Maybe");
            c3.Get("Maybe").Should().Be(YesNoMaybe.Maybe);
            c3.Get("").Should().Be(null);
            c3.Get(null).Should().Be(null);
            c3.Set(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_Enum_ErrorCases()
        {
            var invalidInput = "a-z";
            string enumErrorMessage = LanguageResource.Converter_NotValueOf;
            object[] enumErrorArgs = [typeof(YesNoMaybe).Name];

            // Test invalid format for Guid
            TestInvalidFormat<YesNoMaybe>(invalidInput, enumErrorMessage, enumErrorArgs);

            // Test invalid format for nullable Guid
            TestInvalidFormat<YesNoMaybe?>(invalidInput, enumErrorMessage, enumErrorArgs);
        }

        [Test]
        public void DefaultConverter_DateTime_ValidCases()
        {
            var c1 = new DefaultConverter<DateTime>();
            var date = DateTime.Today;
            c1.Get(c1.Set(date)).Should().Be(date);

            var c2 = new DefaultConverter<DateTime?>();
            var date2 = DateTime.Today;
            c2.Get(c2.Set(date2)).Should().Be(date2);
            c2.Set(null).Should().Be(null);
            c2.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_DateTime_Format_AffectsConversion()
        {
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
        public void DefaultConverter_DateTime_ErrorCases()
        {
            var invalidInput = "a-z";
            var dtErrorMessage = LanguageResource.Converter_InvalidDateTime;

            // Test invalid format for DateTime
            TestInvalidFormat<DateTime>(invalidInput, dtErrorMessage);

            // Test invalid format for nullable DateTime
            TestInvalidFormat<DateTime?>(invalidInput, dtErrorMessage);
        }

        [Test]
        public void DefaultConverter_TimeSpan_ValidCases()
        {
            var c1 = new DefaultConverter<TimeSpan>();
            var time = DateTime.Now.TimeOfDay;
            c1.Get(c1.Set(time)).Should().Be(time);

            var c2 = new DefaultConverter<TimeSpan?>();
            var time2 = DateTime.Now.TimeOfDay;
            c2.Get(c2.Set(time2)).Should().Be(time2);
            c2.Set(null).Should().Be(null);
            c2.Get(null).Should().Be(null);
        }

        [Test]
        public void DefaultConverter_TimeSpan_DefaultTimeSpanFormat_AffectsConversion()
        {
            var converter = new DefaultConverter<TimeSpan>();
            var time = new TimeSpan(1, 2, 3);

            // Test custom DefaultTimeSpanFormat
            converter.DefaultTimeSpanFormat = @"hh\:mm";
            converter.Set(time).Should().Be("01:02");
            converter.Get("01:02").Should().Be(new TimeSpan(1, 2, 0));

            // Test Format property override
            converter.Format = @"hh\:mm\:ss\.fff";
            var preciseTime = new TimeSpan(0, 1, 2, 3, 456);
            converter.Set(preciseTime).Should().Be("01:02:03.456");
            converter.Get("01:02:03.456").Should().Be(preciseTime);
        }

        [Test]
        public void DefaultConverter_TimeSpan_ErrorCases()
        {
            var invalidInput = "12:o1";
            var overflowInput = "25:00";
            var tmErrorMessage = LanguageResource.Converter_InvalidTimeSpan;

            // Test invalid format for TimeSpan
            TestInvalidFormat<TimeSpan>(invalidInput, tmErrorMessage);

            // Test invalid format for nullable TimeSpan
            TestInvalidFormat<TimeSpan?>(invalidInput, tmErrorMessage);

            // Test timespan overflow for TimeSpan
            TestInvalidFormat<TimeSpan>(overflowInput, tmErrorMessage);

            // Test timespan overflow for nullable TimeSpan
            TestInvalidFormat<TimeSpan?>(overflowInput, tmErrorMessage);
        }

        [Test]
        public void DefaultConverter_NotImplementedType_ValidCases()
        {
            var notImplementedType = new object();

            var c1 = new DefaultConverter<object>();
            c1.Set(notImplementedType).Should().Be(notImplementedType.ToString());
        }

        [Test]
        public void DefaultConverter_NotImplementedType_ErrorCases()
        {
            string errorMessage = LanguageResource.Converter_ConversionNotImplemented;
            object[] errorArgs = [typeof(object)];

            // Test invalid format
            TestInvalidFormat<object>("a-z", errorMessage, errorArgs);
        }

        private void TestInvalidFormat<T>(string input, string expectedErrorMessage, object[] expectedErrorArguments = null)
        {
            bool onErrorInvoked = false;

            var converter = new DefaultConverter<T>
            {
                OnError = (string msg, object[] arguments) => onErrorInvoked = true,
            };

            converter.Get(input).Should().Be(default(T));

            // Test GetError
            converter.GetError.Should().BeTrue();

            // Test OnError callback
            onErrorInvoked.Should().BeTrue();

            // Test GetErrorMessage
            converter.GetErrorMessage.HasValue.Should().BeTrue();
            converter.GetErrorMessage.Value.Item1.Should().Be(expectedErrorMessage);
            if (expectedErrorArguments == null)
            {
                converter.GetErrorMessage.Value.Item2.Should().BeEmpty();
            }
            else
            {
                converter.GetErrorMessage.Value.Item2.Should().BeEquivalentTo(expectedErrorArguments);
            }
        }

        [Test]
        public void DefaultConverter_GlobalCatch_ErrorCases()
        {
            bool toBeThrow = true;

            string exception = "test";
            var convertFromString = new DefaultConverter<object>()
            {
                // The goal is to trigger the catch block in DefaultConverter without propagating the exception to the Converter.Get method.
                OnError = (string msg, object[] arguments) =>
                {
                    if (toBeThrow)
                    {
                        toBeThrow = false;
                        throw new FormatException(exception);
                    }
                },
            };

            convertFromString.Get("a-z").Should().Be(null);
            convertFromString.GetError.Should().BeTrue();
            convertFromString.GetErrorMessage.HasValue.Should().BeTrue();
            convertFromString.GetErrorMessage.Value.Item1.Should().Be(LanguageResource.Converter_ConversionError);
            convertFromString.GetErrorMessage.Value.Item2.Should().BeEquivalentTo(new object[] { exception });

            toBeThrow = true;

            var convertToString = new DefaultConverter<object>();
            DefaultConverter<object>.GlobalGetFunc = x => throw new FormatException(exception);

            convertFromString.Set("a-z").Should().Be(null);
            convertFromString.SetError.Should().BeTrue();
            convertFromString.SetErrorMessage.HasValue.Should().BeTrue();
            convertFromString.SetErrorMessage.Value.Item1.Should().Be(LanguageResource.Converter_ConversionFailed);
            convertFromString.SetErrorMessage.Value.Item2.Should().BeEquivalentTo(new object[] { typeof(string).Name, typeof(object).Name, exception });
            DefaultConverter<object>.GlobalGetFunc = null;
        }

        [Test]
        public void DateConverter_Format_AffectsConversion()
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
        public void BoolConverter_ValidCases()
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
        public void CustomlConverter_ValidCases()
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
