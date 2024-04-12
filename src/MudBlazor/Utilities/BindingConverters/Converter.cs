using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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

        public Action<string>? OnError { get; set; }

        [MemberNotNullWhen(true, nameof(SetErrorMessage))]
        public bool SetError { get; set; }

        [MemberNotNullWhen(true, nameof(GetErrorMessage))]
        public bool GetError { get; set; }

        public string? SetErrorMessage { get; set; }

        public string? GetErrorMessage { get; set; }

        public U? Set(T? value)
        {
            SetError = false;
            SetErrorMessage = null;
            if (SetFunc == null)
                return default(U);
            try
            {
                return SetFunc(value);
            }
            catch (Exception e)
            {
                SetError = true;
                SetErrorMessage = $"Conversion from {typeof(T).Name} to {typeof(U).Name} failed: {e.Message}";
            }
            return default(U);
        }

        public T? Get(U? value)
        {
            GetError = false;
            GetErrorMessage = null;
            if (GetFunc == null)
                return default(T);
            try
            {
                return GetFunc(value);
            }
            catch (Exception e)
            {
                GetError = true;
                GetErrorMessage = $"Conversion from {typeof(U).Name} to {typeof(T).Name} failed: {e.Message}";
            }
            return default(T);
        }

        protected void UpdateSetError(string msg)
        {
            SetError = true;
            SetErrorMessage = msg;
            OnError?.Invoke(msg);
        }

        protected void UpdateGetError(string msg)
        {
            GetError = true;
            GetErrorMessage = msg;
            OnError?.Invoke(msg);
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
