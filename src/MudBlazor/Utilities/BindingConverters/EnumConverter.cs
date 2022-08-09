using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public class EnumConverter<T> : Converter<T, string> where T : Enum
    {
        public EnumConverter()
        {
            SetFunc = OnSet;
            GetFunc = OnGet;
        }

        private T OnGet(string value)
        {
            try
            {
                foreach (var fieldInfo in typeof(T).GetFields())
                {
                    if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute descAttr)
                    {
                        if (descAttr.Description == value || fieldInfo.Name == value)
                        {
                            return (T)fieldInfo.GetValue(null);
                        }
                    }
                    else
                    {
                        if (fieldInfo.Name == value)
                        {
                            return (T)fieldInfo.GetValue(null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UpdateGetError("Conversion error: " + e.Message);
                return default;
            }

            return default;
        }

        private string OnSet(T value)
        {
            try
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute descAttr && !string.IsNullOrEmpty(descAttr.Description))
                {
                    return descAttr.Description;
                }
                else
                {
                    return value.ToString();
                }
            }
            catch (FormatException ex)
            {
                UpdateSetError("Conversion error: " + ex.Message);
                return default;
            }
        }
    }
}
