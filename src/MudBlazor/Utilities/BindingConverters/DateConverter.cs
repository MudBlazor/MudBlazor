using System;

namespace MudBlazor
{

    /// <summary>
    /// A ready made DateTime? to string binding converter with configurable date format and culture
    /// </summary>
    public class NullableDateConverter : Converter<DateTime?>
    {
        public string DateFormat { get; set; }

        public NullableDateConverter(string format = "yyyy-MM-dd")
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
            catch (FormatException e)
            {
                UpdateGetError(e.Message);
                return null;
            }
        }

        private string OnSet(DateTime? arg)
        {
            if (arg == null)
                return null;
            try
            {
                return arg.Value.ToString(DateFormat, Culture);
            }
            catch (FormatException e)
            {
                UpdateSetError(e.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// A ready made DateTime to string binding converter with configurable date format and culture
    /// </summary>
    public class DateConverter : Converter<DateTime>
    {
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        public DateConverter(string format)
        {
            DateFormat = format;
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private DateTime OnGet(string arg)
        {
            try
            {
                return DateTime.ParseExact(arg, DateFormat, Culture);
            }
            catch (FormatException e)
            {
                UpdateGetError(e.Message);
                return default;
            }
        }

        private string OnSet(DateTime arg)
        {
            try
            {
                return arg.ToString(DateFormat, Culture);
            }
            catch (FormatException e)
            {
                UpdateSetError(e.Message);
                return null;
            }
        }
    }

}
