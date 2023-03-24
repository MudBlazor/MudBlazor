// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public static class ExpressionExtensions
    {
        private static Tout GetAttribute<Tin, Tout>(Expression<Func<Tin>> expression)
        {
            var memberExpression = (MemberExpression)expression?.Body;
            var propertyInfo = memberExpression?.Expression?.Type.GetProperty(memberExpression.Member.Name);

            var attributeList = propertyInfo?.GetCustomAttributes(typeof(Tout), true)?.Cast<Tout>();
            if (attributeList?.Any() ?? false)
            {
                return attributeList.First() ?? default;
            }
            return default;
        }

        public static string GetFullPathOfMember<T>(this Expression<Func<T>> property)
        {
            var resultingString = string.Empty;
            var p = property.Body as MemberExpression;

            while (p != null)
            {
                if (p.Expression is MemberExpression)
                {
                    resultingString = p.Member.Name + (resultingString != string.Empty ? "." : string.Empty) + resultingString;
                }
                p = p.Expression as MemberExpression;
            }
            return resultingString;
        }

        /// <summary>
        /// Returns the display name attribute of the provided field property as a string. If this attribute is missing, the member name will be returned.
        /// </summary>
        public static string GetLabelString<T>(this Expression<Func<T>> expression)
        {
            var attribute = GetAttribute<T, LabelAttribute>(expression);
            return attribute?.Name ?? string.Empty;
        }

        /// <summary>
        /// Returns the data type attribute of the provided field property as an InputType. If this attribute is missing, the InputType.Text will be returned.
        /// </summary>
        public static InputType GetInputTypeFromDataType<T>(this Expression<Func<T>> expression)
        {
            var attribute = GetAttribute<T, DataTypeAttribute>(expression);
            var dataType = attribute?.DataType ?? DataType.Text;

            switch (dataType)
            {
                case DataType.DateTime:
                case DataType.Date:
                    return InputType.Date;

                case DataType.Time:
                case DataType.Duration:
                    return InputType.Time;
                    
                case DataType.PhoneNumber:
                    return InputType.Telephone;

                case DataType.Currency:
                    return InputType.Number;
                
                case DataType.EmailAddress:
                    return InputType.Email;

                case DataType.Password:
                    return InputType.Password;

                case DataType.Url:
                case DataType.ImageUrl:
                    return InputType.Url;

                case DataType.Custom:
                case DataType.Text:
                case DataType.Html:
                case DataType.MultilineText:
                case DataType.CreditCard:
                case DataType.PostalCode:
                case DataType.Upload:
                default:
                    return InputType.Text;
            }
        }

        /// <summary>
        /// Returns the data type attribute of the provided field property as an InputMode. If this attribute is missing, the InputMode.text will be returned.
        /// </summary>
        public static InputMode GetInputModeFromDataType<T>(this Expression<Func<T>> expression)
        {
            var attribute = GetAttribute<T, DataTypeAttribute>(expression);
            var dataType = attribute?.DataType ?? DataType.Text;

            switch (dataType)
            {
                case DataType.PhoneNumber:
                    return InputMode.tel;

                case DataType.Currency:
                    return InputMode.@decimal;

                case DataType.Url:
                case DataType.ImageUrl:
                    return InputMode.url;

                case DataType.CreditCard:
                    return InputMode.numeric;

                case DataType.Text:
                case DataType.Html:
                case DataType.MultilineText:
                case DataType.EmailAddress:
                case DataType.Password:
                case DataType.PostalCode:
                case DataType.Custom:
                case DataType.DateTime:
                case DataType.Date:
                case DataType.Time:
                case DataType.Duration:
                case DataType.Upload:
                default:
                    return InputMode.text;
            }
        }
    }
}
