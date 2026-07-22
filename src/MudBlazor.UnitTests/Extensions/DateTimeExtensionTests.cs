// Copyright (c) 2021 - MudBlazor

using System;
using System.Globalization;
using FluentAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;



namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        [Test]
        public void StartOfMonth_EndOfMonth_Test()
        {
            var date = new DateTime(2021, 02, 14);
            var gregorian_start = date.StartOfMonth(CultureInfo.InvariantCulture);
            gregorian_start.ToIsoDateString().Should().Be("2021-02-01");
            var gregorian_end = date.EndOfMonth(CultureInfo.InvariantCulture);
            gregorian_end.ToIsoDateString().Should().Be("2021-02-28");
            var persian = CultureInfo.GetCultureInfo("fa-IR");
            var persian_start = date.StartOfMonth(persian);
            persian_start.ToIsoDateString().Should().Be("2021-01-20"); // 1399-11-01
            var persian_end = date.EndOfMonth(persian);
            persian_end.ToIsoDateString().Should().Be("2021-02-18");  // 1399-11-30
        }
    }
}
