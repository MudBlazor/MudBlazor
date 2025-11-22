using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MudBlazor.Resources;

namespace MudBlazor
{
#nullable enable
    public class Converter<T, U>
    {
        public Func<T?, U?>? SetFunc { get; set; }

        public Func<U?, T?>? GetFunc { get; set; }

        /// <summary>
        /// The culture info being used for decimal points, date and time format, etc.
        /// </summary>
        public CultureInfo Culture { get; set; } = Converters.DefaultCulture;

        public Action<string, object[]>? OnError { get; set; }

        [MemberNotNullWhen(true, nameof(SetErrorMessage))]
        public bool SetError { get; set; }

        [MemberNotNullWhen(true, nameof(GetErrorMessage))]
        public bool GetError { get; set; }

        public (string, object[])? SetErrorMessage { get; set; }

        public (string, object[])? GetErrorMessage { get; set; }

        public U? Set(T? value)
        {
            SetError = false;
            SetErrorMessage = default;
            if (SetFunc == null)
                return default(U);
            try
            {
                return SetFunc(value);
            }
            catch (Exception e)
            {
                SetError = true;
                SetErrorMessage = (LanguageResource.Converter_ConversionFailed, [typeof(T).Name, typeof(U).Name, e.Message]);
            }
            return default(U);
        }

        public T? Get(U? value)
        {
            GetError = false;
            GetErrorMessage = default;
            if (GetFunc == null)
                return default(T);
            try
            {
                return GetFunc(value);
            }
            catch (Exception e)
            {
                GetError = true;
                GetErrorMessage = (LanguageResource.Converter_ConversionFailed, [typeof(U).Name, typeof(T).Name, e.Message]);
            }
            return default(T);
        }

        protected void UpdateSetError(string msg)
        {
            UpdateSetError(msg, []);
        }

        protected void UpdateSetError(string msg, object[] arguments)
        {
            SetError = true;
            SetErrorMessage = (msg, arguments);
            OnError?.Invoke(msg, arguments);
        }

        protected void UpdateGetError(string msg)
        {
            UpdateGetError(msg, []);
        }

        protected void UpdateGetError(string msg, object[] arguments)
        {
            GetError = true;
            GetErrorMessage = (msg, arguments);
            OnError?.Invoke(msg, arguments);
        }
    }

    /// <summary>
    /// <para>Converter from T to string</para>
    /// <para>
    /// Set converts to string
    /// Get converts from string
    /// </para>
    /// </summary>
    public class Converter<T> : Converter<T, string>
    {
        /// <summary>
        /// Custom Format to be applied on bidirectional way.
        /// </summary>
        public string? Format { get; set; } = null;
    }
}
