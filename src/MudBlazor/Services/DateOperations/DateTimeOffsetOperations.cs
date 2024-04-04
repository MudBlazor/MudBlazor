// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudBlazor;

public class DateTimeOffsetOperations : DateOperationsBase<DateTimeOffset>, IDateOperations<DateTimeOffset>
{
    public DateTimeOffset Today { get; }

    public IDateOperations<DateTimeOffset> WithDate(DateTimeOffset date)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> WithDate(DateTimeOffset? date, DateTimeOffset defaultDate)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> WithToday()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> EndOfMonth(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> StartOfMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> WithCulture(CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> GetMonthStart(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> AddDays(int days)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> StartOfWeek(DayOfWeek dayOfWeek)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateTimeOffset> GetWeekDays()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> SetYear(int year)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> AddMonths(int month)
    {
        throw new NotImplementedException();
    }

    public bool IsMinDate()
    {
        throw new NotImplementedException();
    }

    public int GetDayOfMonth()
    {
        throw new NotImplementedException();
    }

    public void SetCulture(CultureInfo value)
    {
        throw new NotImplementedException();
    }

    public string GetWeekNumber(int index)
    {
        throw new NotImplementedException();
    }

    public string ToString(string format, CultureInfo culture = null)
    {
        throw new NotImplementedException();
    }

    public int GetYear()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> AddYears(int years)
    {
        throw new NotImplementedException();
    }

    public int GetMonth()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateTimeOffset> GetAllMonths()
    {
        throw new NotImplementedException();
    }

    public DateTimeOffset MinSupportedDate()
    {
        throw new NotImplementedException();
    }

    public bool LesserThan(DateTimeOffset? date)
    {
        throw new NotImplementedException();
    }

    public bool GreaterThan(DateTimeOffset? date)
    {
        throw new NotImplementedException();
    }

    public bool Equal(DateTimeOffset? date)
    {
        throw new NotImplementedException();
    }

    public int DaysInMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTimeOffset> SetDay(int day)
    {
        throw new NotImplementedException();
    }

    public int GetDay()
    {
        throw new NotImplementedException();
    }
}
