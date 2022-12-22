// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <typeparam name="T">The type of data represented by each row in the data grid.</typeparam>
    [RequiresUnreferencedCode(CodeMessage.SerializationUnreferencedCodeMessage)]
    public partial class TemplateColumn<T> : Column<T>
    {
        protected internal override object? CellContent(T item)
            => null;

        protected internal override object? PropertyFunc(T item)
            => null;

        //protected internal override MemberExpression? PropertyExpression()
        //    => null;

        protected internal override Type PropertyType
            => typeof(object);

        public override string? PropertyName
            => null;

        protected internal override string? FullPropertyName
            => null;

        protected internal override void SetProperty(object item, object value)
        {

        }
    }
}
