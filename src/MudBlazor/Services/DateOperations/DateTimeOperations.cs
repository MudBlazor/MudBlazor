// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudBlazor;

public class DateTimeOperations : DateOperationsBase<DateTime>, IDateOperations<DateTime>
{
    public DateTime Today { get; }

    public IDateOperations<DateTime> WithDate(DateTime date)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> WithDate(DateTime? date, DateTime defaultDate)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> WithToday()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> EndOfMonth(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> StartOfMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> WithCulture(CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> GetMonthStart(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> AddDays(int days)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> StartOfWeek(DayOfWeek dayOfWeek)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateTime> GetWeekDays()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> SetYear(int year)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> AddMonths(int month)
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

    public IDateOperations<DateTime> AddYears(int years)
    {
        throw new NotImplementedException();
    }

    public int GetMonth()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateTime> GetAllMonths()
    {
        throw new NotImplementedException();
    }

    public DateTime MinSupportedDate()
    {
        throw new NotImplementedException();
    }

    public bool LesserThan(DateTime? date)
    {
        throw new NotImplementedException();
    }

    public bool GreaterThan(DateTime? date)
    {
        throw new NotImplementedException();
    }

    public bool Equal(DateTime? date)
    {
        throw new NotImplementedException();
    }

    public int DaysInMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateTime> SetDay(int day)
    {
        throw new NotImplementedException();
    }

    public int GetDay()
    {
        throw new NotImplementedException();
    }
}
