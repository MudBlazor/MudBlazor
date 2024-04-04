// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MudBlazor;

#nullable enable
public interface IDateOperations<TDateType> where TDateType : struct
{
    TDateType Value { get; }
    TDateType Today { get; }

    IDateOperations<TDateType> WithDate(TDateType date);
    IDateOperations<TDateType> WithDate(TDateType? date, TDateType defaultDate);

    IDateOperations<TDateType> WithToday();

    IDateOperations<TDateType> EndOfMonth(int month);
    IDateOperations<TDateType> StartOfMonth();

    IDateOperations<TDateType> WithCulture(CultureInfo culture);

    IDateOperations<TDateType> GetMonthStart(int month);
    IDateOperations<TDateType> AddDays(int days);
    IDateOperations<TDateType> StartOfWeek(DayOfWeek dayOfWeek);
    IEnumerable<TDateType> GetWeekDays();
    IDateOperations<TDateType> SetYear(int year);
    IDateOperations<TDateType> AddMonths(int month);
    IDateOperations<TDateType> AddYears(int years);

    bool IsMinDate();
    int GetDayOfMonth();
    void SetCulture(CultureInfo value);
    string GetWeekNumber(int index);

    string ToString(string format, CultureInfo? culture = null);
    int GetYear();
    int GetMonth();
    IEnumerable<TDateType> GetAllMonths();
    TDateType MinSupportedDate();
    bool LesserThan(TDateType? date);
    bool GreaterThan(TDateType? date);
    bool Equal(TDateType? date);
    int DaysInMonth();
    IDateOperations<TDateType> SetDay(int day);
    int GetDay();
}
