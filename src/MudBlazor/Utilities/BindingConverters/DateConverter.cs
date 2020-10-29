using System;
using System.Globalization;

namespace MudBlazor
{
    
    /// <summary>
    /// A ready made DateTime? to string binding converter with configurable date format and culture
    /// </summary>
    public class DateConverter : Converter<DateTime?>
    {
        public string DateFormat { get; set; }="yyyy-MM-dd";
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
        
        public DateConverter(string format)
        {
            DateFormat = format;
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private DateTime? OnGet(string arg)
        {
            try
            {
                return DateTime.ParseExact(arg, DateFormat, Culture);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private string OnSet(DateTime? arg)
        {
            if (arg == null)
                return null;
            try
            {
                return arg.Value.ToString( DateFormat, Culture);
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}