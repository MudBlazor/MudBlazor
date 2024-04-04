// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudBlazor;

public class DateOnlyOperations : DateOperationsBase<DateOnly>, IDateOperations<DateOnly>
{
    public DateOnly Today => new(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

    public IDateOperations<DateOnly> WithDate(DateOnly date)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> WithDate(DateOnly? date, DateOnly defaultDate)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> WithToday()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> EndOfMonth(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> StartOfMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> WithCulture(CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> GetMonthStart(int month)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> AddDays(int days)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> StartOfWeek(DayOfWeek dayOfWeek)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateOnly> GetWeekDays()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> SetYear(int year)
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> AddMonths(int month)
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

    public IDateOperations<DateOnly> AddYears(int years)
    {
        throw new NotImplementedException();
    }

    public int GetMonth()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DateOnly> GetAllMonths()
    {
        throw new NotImplementedException();
    }

    public DateOnly MinSupportedDate()
    {
        throw new NotImplementedException();
    }

    public bool LesserThan(DateOnly? date)
    {
        throw new NotImplementedException();
    }

    public bool GreaterThan(DateOnly? date)
    {
        throw new NotImplementedException();
    }

    public bool Equal(DateOnly? date)
    {
        throw new NotImplementedException();
    }

    public int DaysInMonth()
    {
        throw new NotImplementedException();
    }

    public IDateOperations<DateOnly> SetDay(int day)
    {
        throw new NotImplementedException();
    }

    public int GetDay()
    {
        throw new NotImplementedException();
    }
}
