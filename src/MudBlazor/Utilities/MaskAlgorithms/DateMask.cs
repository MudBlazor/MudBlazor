// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MudBlazor;

public partial class DateMask : PatternMask
{

    public DateMask(string mask, char year = 'y', char month = 'M', char day = 'd') : base(mask)
    {
        _y = year;
        _M = month;
        _d = day;
        _maskChars = _maskChars.Concat(new[] { MaskChar.Digit(year), MaskChar.Digit(month), MaskChar.Digit(day), })
            .ToArray();
    }

    private char _y;
    private char _M;
    private char _d;

    private int _year = 0;
    private int _month = 0;
    private int _day = 0;

    protected override void ModifyPartiallyAlignedMask(string mask, string text, int maskOffset, ref int textIndex, ref int maskIndex, ref string alignedText)
    {
        if (alignedText.IsEmpty())
            return;
        var y = ExtractYear(mask, alignedText, maskOffset);
        if (y >= 0)
            _year = y;
        MonthLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
        DayLogic(mask, text, maskOffset, ref textIndex, ref maskIndex, ref alignedText);
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
            if (int.TryParse(yearString, out var y))
                return (DateTime.Today.Year / 100 * 100) + y; // this code will still work in 2100 until 2900. I guess in a thousand years we'll have to update this line ;)
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

    /// <summary>
    /// Note: this is required for special cases where the date becomes invalid after the last character has been input
    /// For instance: MM/dd/yyyy from 02/29/000| to 02/29/0001|
    /// The year 0001 is not a leap year so the day needs to be corrected to 28
    /// Or this 0[3]/31/2000 input "4" => 04/31/2000
    /// But we do this only for a complete date!
    /// </summary>
    protected override string ModifyFinalText(string text)
    {
        try
        {
            var yyyy = new string(_y, 4);
            var yy = new string(_y, 2);
            var dd = new string(_d, 2);
            var MM = new string(_M, 2);
            var maskHasDay = Mask.Contains(dd);
            var maskHasMonth = Mask.Contains(MM);
            var maskHasYear = Mask.Contains(yy) || Mask.Contains(yyyy);
            var (dayString, dayIndex) = Extract(dd, Mask, 0, text);
            var (monthString, monthIndex) = Extract(MM, Mask, 0, text);
            var dayFound = dayIndex >= 0;
            var dayComplete = dayString?.Length == 2;
            var monthFound = monthIndex >= 0;
            var monthComplete = monthString?.Length == 2;
            var y = ExtractYear(Mask, text, 0);
            //if (maskHasYear && y < 0 || maskHasMonth && (!monthFound || !monthComplete) || maskHasDay && (!dayFound || !dayComplete))
            //    return text; // we have incomplete input, no final check necessary/possible
            int.TryParse(dayString ?? "", out var d);
            int.TryParse(monthString ?? "", out var m);
            if (!maskHasYear)
                y = 0;
            if (y < 0)
                y = _year;
            if (maskHasMonth && (monthFound || monthComplete))
            {
                var m1 = FixMonth(m);
                if (m1 != m)
                    text = text.Remove(monthIndex, 2).Insert(monthIndex, $"{m1:D2}");
            }

            if (maskHasDay && (dayFound || dayComplete))
            {
                var d1 = FixDay(y, m, d);
                text = text.Remove(dayIndex, 2).Insert(dayIndex, $"{d1:D2}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in ModifyFinalText: " + e.Message);
            return text;
        }
        return text;
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

    private (string, int) Extract(string maskPart, string mask, int maskOffset, string alignedText)
    {
        var maskIndex = mask.IndexOf(maskPart);
        var index = maskIndex - maskOffset;
        if (index < 0 || index >= alignedText.Length)
            return (null, -1);
        var subString = alignedText.Substring(index, Math.Min(maskPart.Length, alignedText.Length - index));
        if (!ValidDigitRegularExpression().IsMatch(subString))
            return (null, -1);
        return (subString, index);
    }

    public override void UpdateFrom(IMask other)
    {
        base.UpdateFrom(other);
        if (other is not DateMask o)
            return;
        _y = o._y;
        _M = o._M;
        _d = o._d;
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex ValidDigitRegularExpression();
}

