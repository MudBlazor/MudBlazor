// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MudBlazor;

public class DateTimeMask : PatternMask
{
    private char _y;
    private char _M;
    private char _d;
    private char _h;
    private char _m;
    private char _s;

    private int _year = 0;
    private int _month = 0;
    private int _day = 0;
    private int _hour = 0;
    private int _minute = 0;
    private int _second = 0;

    public DateTimeMask(string mask, char year = 'y', char month = 'M', char day = 'd', char hour = 'h', char minute = 'm', char second = 's') : base(mask)
    {
        _y = year;
        _M = month;
        _d = day;
        _h = hour;
        _m = minute;
        _s = second;
        _maskChars = _maskChars.Concat(new[] { MaskChar.Digit(year), MaskChar.Digit(month), MaskChar.Digit(day), MaskChar.Digit(hour), MaskChar.Digit(minute), MaskChar.Digit(second), })
            .ToArray();
    }

    protected override void ModifyPartiallyAlignedMask(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        if (alignedText.IsEmpty())
            return;
        var y = ExtractYear(mask, alignedText, maskOffset);
        if (y >= 0)
            _year = y;
        MonthLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        DayLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        HourLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        MinuteLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        SecondLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
    }

    private int ExtractYear(string mask, string alignedText, int maskOffset)
    {
        var yyyy = new string(_y, 4);
        var yy = new string(_y, 2);
        if (mask.Contains(yyyy))
        {
            var (yearString, _) = Extract(yyyy, mask, maskOffset, alignedText);
            if (yearString == null || yearString.Length < 4)
                return -1;
            if (int.TryParse(yearString, out var year))
                return year;
        }
        else if (mask.Contains(yy))
        {
            var (yearString, _) = Extract(yy, mask, maskOffset, alignedText);
            if (yearString == null || yearString.Length < 2)
                return -1;
            if (int.TryParse(yearString, out var year))
                return year + 2000;
        }
        return -1;
    }

    private void MonthLogic(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        var MM = new string(_M, 2);
        var (monthString, index) = Extract(MM, mask, maskOffset, alignedText);
        if (monthString == null)
            return;
        if (!int.TryParse(monthString, out var month))
            return;
        if (monthString.Length == 1)
        {
            // we are at the first digit of MM, only 0 and 1 are allowed
            if (month > 1)
            {
                alignedText = alignedText.Insert(index, "0");
                maskIndex++;
            }
        }
        else if (monthString.Length == 2)
        {
            var fixedMonth = FixMonth(month);
            _month = fixedMonth;
            if (fixedMonth != month)
                alignedText = alignedText.Remove(index, 2).Insert(index, $"{fixedMonth:D2}");
        }
    }

    private void DayLogic(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        var dd = new string(_d, 2);
        var (dayString, index) = Extract(dd, mask, maskOffset, alignedText);
        if (dayString == null)
            return;
        if (!int.TryParse(dayString, out var day))
            return;
        if (dayString.Length == 1)
        {
            // we are at the first digit of dd, only 0..3 are allowed except if month is February. 
            if (day > 3 || day == 3 && _month == 2)
            {
                // by inserting a 0 we make 09 out of 9
                alignedText = alignedText.Insert(index, "0");
                maskIndex++;
                _day = day;
            }
        }
        else if (dayString.Length == 2)
        {
            var fixedDay = FixDay(_year, _month, day);
            if (fixedDay != day)
                alignedText = alignedText.Remove(index, 2).Insert(index, $"{fixedDay:D2}");
            _day = fixedDay;
        }
    }

    private int FixDay(int year, int month, int day)
    {
        if (day == 0)
            return 1;
        if (day > 28)
        {
            var daysInMonth = GetDaysInMonth(year, month);
            if (day > daysInMonth)
                return daysInMonth;
        }
        return day;
    }

    private int FixMonth(int month)
    {
        if (month == 0)
            return 1;
        if (month > 12)
            return 12;
        return month;
    }

    private int GetDaysInMonth(int year, int month)
    {
        if (month <= 0 || month > 12) // we don't know yet which month the user means, so assume 31
            return 31;
        if (year == 0) // DateTime.DaysInMonth does not support year 0 but we just use 4 instead because it was a leap year too
            year = 4;
        return DateTime.DaysInMonth(year, Math.Min(12, Math.Max(1, month)));
    }

    private void HourLogic(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        var hh = new string(_h, 2);
        var (hourString, _) = Extract(hh, mask, maskOffset, alignedText);
        if (hourString == null)
            return;
        if (!int.TryParse(hourString, out var hour))
            return;
        if (hourString.Length == 1)
        {
            if (hour > 1)
            {
                alignedText = alignedText.Insert(maskIndex, "0");
                maskIndex++;
            }
        }
        else if (hourString.Length == 2)
        {
            var fixedHour = FixHour(hour);
            _hour = fixedHour;
            if (fixedHour != hour)
                alignedText = alignedText.Remove(maskIndex, 2).Insert(maskIndex, $"{fixedHour:D2}");
        }
    }

    private void MinuteLogic(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        var mm = new string(_m, 2);
        var (minuteString, _) = Extract(mm, mask, maskOffset, alignedText);
        if (minuteString == null)
            return;
        if (!int.TryParse(minuteString, out var minute))
            return;
        if (minuteString.Length == 1)
        {
            if (minute > 1)
            {
                alignedText = alignedText.Insert(maskIndex, "0");
                maskIndex++;            }
        }
        else if (minuteString.Length == 2)
        {
            var fixedMinute = Fixminute(minute);
            _minute = fixedMinute;
            if (fixedMinute != minute)
                alignedText = alignedText.Remove(maskIndex, 2).Insert(maskIndex, $"{fixedMinute:D2}");
        }
    }

    private void SecondLogic(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        var ss = new string(_s, 2);
        var (secondString, _) = Extract(ss, mask, maskOffset, alignedText);
        if (secondString == null)
            return;
        if (!int.TryParse(secondString, out var second))
            return;
        if (secondString.Length == 1)
        {
            if (second > 1)
            {
                alignedText = alignedText.Insert(maskIndex, "0");
                maskIndex++;
            }
        }
        else if (secondString.Length == 2)
        {
            var fixedSecond = Fixminute(second);
            _second = fixedSecond;
            if (fixedSecond != second)
                alignedText = alignedText.Remove(maskIndex, 2).Insert(maskIndex, $"{fixedSecond:D2}");
        }
    }

    // Fixes hour within the 24 hour range
    private int FixHour(int hour)
    {
        return hour % 24;
    }

    private int Fixminute(int minute)
    {
        return minute % 60;
    }

    private (string, int) Extract(string maskPart, string mask, int maskOffset, string alignedText)
    {
        var maskIndex = mask.IndexOf(maskPart);
        var index = maskIndex - maskOffset;
        if (index < 0 || index >= alignedText.Length)
            return (null, -1);
        var subString = alignedText.Substring(index, Math.Min(maskPart.Length, alignedText.Length - index));
        if (!Regex.IsMatch(subString, @"^\d+$"))
            return (null, -1);
        return (subString, index);
    }
}
