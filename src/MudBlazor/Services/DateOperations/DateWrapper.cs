// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using MudBlazor.Extensions;

namespace MudBlazor.Services;
#nullable enable

internal interface IDateWrapper<T> where T : struct
{
    CultureInfo Culture { get; }

    /// <summary>
    /// returns the current date
    /// </summary>
    T Today { get; }

    void SetCulture(CultureInfo culture);
    T EndOfMonth(T? date, int month = 0);
    T StartOfMonth(T? date, int month = 0);
    IEnumerable<T> GetWeek(T? date, int month, int index, DayOfWeek? dayOfWeek = null);
    int? GetWeekNumber(T? date, int month, int index, DayOfWeek? dayOfWeek = null);
    bool IsDateDisabled(T date, T? minDate, T? maxDate, Func<T, bool> isDateDisabledFunc);
    bool IsMonthDisabled(T date, int? fixDay, T? minDate, T? maxDate, Func<T, bool> isDateDisabledFunc);
    bool SameMonth(T date1, T date2);
    bool SameDay(T? date1, T? date2);
    bool AreEqual(T? date1, T? date2);
    bool GreaterThan(T date1, T date2);
    bool LesserThan(T date1, T date2);

    /// <summary>
    /// Just curious why AddYears differs from AddMonths
    /// </summary>
    /// <param name="date"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    T AddYear(T? date, int year);

    T? AddMonths(T? date, int month);
    bool IsMinYearMonth(T? date);
    T GetPreviousMonth(T? date);
    T GetNextMonth(T? pickerMonth);
    string? ToString(T? date, string format);
    int GetYear(T date);

    /// <summary>
    /// Returns all months of the year of date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    IEnumerable<T> GetAllMonths(T? date);

    int GetCalendarMonth(T month);
    int GetCalendarDayOfMonth(T date);
    T AddDays(T date, int days);
    T GetFromFixedValues(T selectedDate, int? fixYear, int? fixMonth, int? fixDay);
    int GetCalendarYear(T? date, T yearDate);
    bool DateEquals(T? date, T day);
    T SetYear(T? current, int year);
    T SetDateKind(T date, T value);
    T GetDate(T date);
    T SetYearMonth(T month, T? selectedDate);
}

internal class DateWrapper<T> : IDateWrapper<T> where T : struct
{
    private readonly IDateConverter<T> _converter;

    public CultureInfo Culture { get; private set; }

    /// <summary>
    /// returns the current date
    /// </summary>
    public T Today => _converter.ConvertFrom(DateTimeOffset.UtcNow.Date);

    public DateWrapper(IDateConverter<T> converter)
    {
        _converter = converter;
        Culture = CultureInfo.CurrentCulture;
    }

    public void SetCulture(CultureInfo culture)
    {
        Culture = culture;
    }

    public T EndOfMonth(T? date, int month = 0)
    {
        var monthStartDate = date ?? StartOfMonth(Today);

        var dateTime = _converter.ConvertTo(monthStartDate)
            .AddMonths(month, Culture)
            .EndOfMonth(Culture);

        return _converter.ConvertFrom(dateTime);
    }

    public T StartOfMonth(T? date, int month = 0)
    {
        var monthStartDate = date ?? StartOfMonth(Today);

        var dateTime = _converter.ConvertTo(monthStartDate)
            .AddMonths(month, Culture)
            .StartOfMonth(Culture);

        return _converter.ConvertFrom(dateTime);
    }

    public IEnumerable<T> GetWeek(T? date, int month, int index, DayOfWeek? dayOfWeek = null)
    {
        if (index is < 0 or > 5)
            throw new ArgumentException("Index must be between 0 and 5");

        var monthFirst = _converter.ConvertTo(StartOfMonth(date, month));
        var weekFirst = monthFirst.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek(dayOfWeek));
        for (var i = 0; i < 7; i++)
        {
            yield return _converter.ConvertFrom(weekFirst.AddDays(i));
        }
    }

    public int? GetWeekNumber(T? date, int month, int index, DayOfWeek? dayOfWeek = null)
    {
        if (index is < 0 or > 5)
            throw new ArgumentException("Index must be between 0 and 5");

        var monthFirst = _converter.ConvertTo(StartOfMonth(date, month));
        var weekFirst = monthFirst.AddDays(index * 7).StartOfWeek(GetFirstDayOfWeek(dayOfWeek));

        //january 1st
        if (monthFirst.Month == 1 && index == 0)
        {
            weekFirst = monthFirst;
        }

        if (weekFirst.Month != monthFirst.Month && weekFirst.AddDays(6).Month != monthFirst.Month)
            return null;

        return Culture.Calendar.GetWeekOfYear(weekFirst.DateTime,
            Culture.DateTimeFormat.CalendarWeekRule, dayOfWeek ?? Culture.DateTimeFormat.FirstDayOfWeek);
    }

    public bool IsDateDisabled(T date, T? minDate, T? maxDate, Func<T, bool> isDateDisabledFunc)
    {
        var dateTimeOffset = _converter.ConvertTo(date);
        var minDateTimeOffset = _converter.ConvertTo(minDate);
        var maxDateTimeOffset = _converter.ConvertTo(maxDate);

        return dateTimeOffset < minDateTimeOffset || dateTimeOffset > maxDateTimeOffset || isDateDisabledFunc(_converter.ConvertFrom(dateTimeOffset));
    }

    public bool IsMonthDisabled(T date, int? fixDay, T? minDate, T? maxDate, Func<T, bool> isDateDisabledFunc)
    {
        var month = _converter.ConvertTo(date);
        var minDateTimeOffset = _converter.ConvertTo(minDate);
        var maxDateTimeOffset = _converter.ConvertTo(maxDate);

        if (!fixDay.HasValue)
        {
            return month.EndOfMonth(Culture) < minDateTimeOffset || month > maxDateTimeOffset;
        }
        if (DateTime.DaysInMonth(month.Year, month.Month) < fixDay.Value)
        {
            return true;
        }
        var day = new DateTimeOffset(month.Year, month.Month, fixDay.Value, 0, 0, 0, month.Offset);
        return day < minDateTimeOffset || day > maxDateTimeOffset || isDateDisabledFunc(_converter.ConvertFrom(day));
    }

    public bool SameMonth(T date1, T date2)
    {
        var date1DateTimeOffset = _converter.ConvertTo(date1);
        var date2DateTimeOffset = _converter.ConvertTo(date2);

        return Culture.Calendar.GetMonth(date1DateTimeOffset.Date) == Culture.Calendar.GetMonth(date2DateTimeOffset.Date);
    }

    public bool SameDay(T? date1, T? date2)
    {
        if (date1 == null ^ date2 == null)
        {
            return false;
        }
        if (!date1.HasValue && !date2.HasValue)
        {
            return true;
        }

        var date1DateTimeOffset = _converter.ConvertTo(date1!.Value);
        var date2DateTimeOffset = _converter.ConvertTo(date2!.Value);

        return date1DateTimeOffset.Date.Day == date2DateTimeOffset.Date.Day;
    }

    public bool AreEqual(T? date1, T? date2)
    {
        if (date1 == null ^ date2 == null)
        {
            return false;
        }
        if (!date1.HasValue && !date2.HasValue)
        {
            return true;
        }
        return _converter.ConvertTo(date1!.Value) == _converter.ConvertTo(date2!.Value);
    }

    public bool GreaterThan(T date1, T date2)
    {
        return _converter.ConvertTo(date1) > _converter.ConvertTo(date2);
    }

    public bool LesserThan(T date1, T date2)
    {
        return _converter.ConvertTo(date1) < _converter.ConvertTo(date2);
    }

    protected DayOfWeek GetFirstDayOfWeek(DayOfWeek? dayOfWeek = null)
    {
        return dayOfWeek ?? Culture.DateTimeFormat.FirstDayOfWeek;
    }

    /// <summary>
    /// Just curious why AddYears differs from AddMonths
    /// </summary>
    /// <param name="date"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    public T AddYear(T? date, int year)
    {
        var current = _converter.ConvertTo(StartOfMonth(date));
        var newDate = new DateTimeOffset(year, current.Month, 1, 0, 0, 0, 0, Culture.Calendar, current.Offset);

        return _converter.ConvertFrom(newDate);
    }

    public T? AddMonths(T? date, int month)
    {
        var newDate = _converter.ConvertTo(date)?.AddMonths(month);

        return _converter.ConvertFrom(newDate);
    }

    public bool IsMinYearMonth(T? date)
    {
        return date.HasValue && _converter.ConvertTo(date.Value) is { Year: 1, Month: 1 };
    }

    public T GetPreviousMonth(T? date)
    {
        var previousMonth = _converter.ConvertTo(StartOfMonth(date)).AddDays(-1).StartOfMonth(Culture);
        return _converter.ConvertFrom(previousMonth);
    }

    public T GetNextMonth(T? pickerMonth)
    {
        var nextMonth = _converter.ConvertTo(EndOfMonth(pickerMonth)).AddDays(1);
        return _converter.ConvertFrom(nextMonth);
    }

    public string? ToString(T? date, string format)
    {
        return _converter.ConvertTo(date)?.ToString(format, Culture);
    }

    public int GetYear(T date)
    {
        return Culture.Calendar.GetYear(_converter.ConvertTo(date).Date);
    }

    /// <summary>
    /// Returns all months of the year of date
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public IEnumerable<T> GetAllMonths(T? date)
    {
        var calendarYear = GetYear(StartOfMonth(date));
        var firstOfCalendarYear = Culture.Calendar.ToDateTime(calendarYear, 1, 1, 0, 0, 0, 0);
        for (var i = 0; i < Culture.Calendar.GetMonthsInYear(calendarYear); i++)
            yield return _converter.ConvertFrom(Culture.Calendar.AddMonths(firstOfCalendarYear, i));
    }

    public int GetCalendarMonth(T month)
    {
        return Culture.Calendar.GetMonth(_converter.ConvertTo(month).Date);
    }

    public int GetCalendarDayOfMonth(T date)
    {
        return Culture.Calendar.GetDayOfMonth(_converter.ConvertTo(date).Date);
    }

    public T AddDays(T date, int days)
    {
        return _converter.ConvertFrom(_converter.ConvertTo(date).AddDays(days));
    }

    public T GetFromFixedValues(T selectedDate, int? fixYear, int? fixMonth, int? fixDay)
    {
        if (!fixYear.HasValue && !fixMonth.HasValue && !fixDay.HasValue)
        {
            return selectedDate;
        }

        var selectedDateTimeOffset = _converter.ConvertTo(selectedDate);

        var fixedSelectedDateTimeOffset = new DateTimeOffset(
            fixYear ?? selectedDateTimeOffset.Year,
            fixMonth ?? selectedDateTimeOffset.Month,
            fixDay ?? selectedDateTimeOffset.Day,
            selectedDateTimeOffset.Hour,
            selectedDateTimeOffset.Minute,
            selectedDateTimeOffset.Second,
            selectedDateTimeOffset.Millisecond, selectedDateTimeOffset.Offset);

        return _converter.ConvertFrom(fixedSelectedDateTimeOffset);
    }

    public int GetCalendarYear(T? date, T yearDate)
    {
        var dateInternal = date ?? Today;
        var dateTimeOffset = _converter.ConvertTo(dateInternal);
        var yearDateTimeOffset = _converter.ConvertTo(yearDate);

        var diff = Culture.Calendar.GetYear(dateTimeOffset.Date) - Culture.Calendar.GetYear(yearDateTimeOffset.Date);
        var calenderYear = Culture.Calendar.GetYear(dateTimeOffset.Date);

        return calenderYear - diff;
    }

    public bool DateEquals(T? date, T day)
    {
        return _converter.ConvertTo(date)?.Date == _converter.ConvertTo(day).Date;
    }

    public T SetYear(T? current, int year)
    {
        DateTimeOffset newDateTimeOffset;
        if (current.HasValue)
        {
            var currentDateTimeOffset = _converter.ConvertTo(current.Value);
            newDateTimeOffset = new DateTimeOffset(year, currentDateTimeOffset.Month, currentDateTimeOffset.Day, currentDateTimeOffset.Hour, currentDateTimeOffset.Minute, currentDateTimeOffset.Second, currentDateTimeOffset.Millisecond, currentDateTimeOffset.Offset);
        }
        else
        {
            newDateTimeOffset = new DateTimeOffset(year, 1, 1, 0, 0, 0, 0, Culture.Calendar, TimeSpan.Zero);
        }

        var newDate = _converter.ConvertFrom(newDateTimeOffset);
        if (current.HasValue)
        {
            newDate = SetDateKind(newDate, current.Value);
        }

        return newDate;
    }

    /// <summary>
    /// </summary>
    /// <param name="date">The date which kind will be set</param>
    /// <param name="value">The date containing the date kind to set</param>
    /// <returns></returns>
    public T SetDateKind(T date, T value)
    {
        // only set the kind when the date is a DateTime
        if (date is DateTime { Kind: DateTimeKind.Unspecified } dateTime && value is DateTime dateKind)
        {
            return (T)(object)DateTime.SpecifyKind(dateTime, dateKind.Kind);
        }

        return date;
    }

    public T GetDate(T date)
    {
        return _converter.ConvertFrom(_converter.ConvertTo(date).Date);
    }

    public T SetYearMonth(T month, T? selectedDate)
    {
        var selectedDateTimeOffset = _converter.ConvertTo(selectedDate);
        var monthDateTimeOffset = _converter.ConvertTo(month);

        selectedDateTimeOffset = selectedDateTimeOffset.HasValue ?
            //everything has to be set because a value could already be defined -> fix values can be ignored as they are set in submit anyway
            new DateTime(
                monthDateTimeOffset.Year,
                monthDateTimeOffset.Month,
                selectedDateTimeOffset.Value.Day,
                selectedDateTimeOffset.Value.Hour,
                selectedDateTimeOffset.Value.Minute,
                selectedDateTimeOffset.Value.Second,
                selectedDateTimeOffset.Value.Millisecond) //We can assume day here, as it was not set yet. If a fix value is set, it will be overriden in Submit
            : new DateTime(monthDateTimeOffset.Year, monthDateTimeOffset.Month, 1);

        var newDate = _converter.ConvertFrom(selectedDateTimeOffset.Value);

        if (selectedDate.HasValue)
        {
            newDate = SetDateKind(newDate, selectedDate.Value);
        }

        return newDate;
    }
}
