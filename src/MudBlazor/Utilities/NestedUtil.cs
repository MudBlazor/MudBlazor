// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MudBlazor.Utilities
{
    [RequiresUnreferencedCode(CodeMessage.SerializationUnreferencedCodeMessage)]
    public static class NestedUtil
    {
        public static object GetPropertyValue(object item, string propName)
        {
            if (item == null) throw new ArgumentException("Value cannot be null.", "item");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains('.'))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                var tempItem = GetPropertyValue(item, temp[0]);
                return (tempItem != null) ? GetPropertyValue(tempItem, temp[1]) : null;
            }
            else
            {
                var prop = item.GetType().GetProperties().SingleOrDefault(x => x.Name == propName);
                return prop.GetValue(item);
            }
        }

        public static void SetPropertyValue(object item, string propName, object propValue)
        {
            if (item == null) throw new ArgumentException("Value cannot be null.", "item");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains('.'))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                var tempItem = GetPropertyValue(item, temp[0]);
                SetPropertyValue(tempItem, temp[1], propValue);
            }
            else
            {
                var prop = item.GetType().GetProperties().SingleOrDefault(x => x.Name == propName);
                prop.SetValue(item, propValue);
            }
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propName)
        {
            if (type == null) throw new ArgumentException("Value cannot be null.", "type");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains('.'))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                var tempInfo = GetPropertyInfo(type, temp[0]);
                return (tempInfo != null) ? GetPropertyInfo(tempInfo.PropertyType, temp[1]) : null;
            }
            else
                return type.GetProperties().SingleOrDefault(x => x.Name == propName);
        }

        public static MemberExpression GetProperty(ParameterExpression parameter, string propName)
        {
            if (parameter == null) throw new ArgumentException("Value cannot be null.", "parameter");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            Expression result = parameter;
            foreach (var member in propName.Split('.'))
                result = Expression.Property(result, member);

            return (MemberExpression)result;
        }
    }
}
