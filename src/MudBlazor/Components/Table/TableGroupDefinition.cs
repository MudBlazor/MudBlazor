// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class TableGroupDefinition<T>
    {
        public TableGroupDefinition(Func<T, object> selector, TableGroupDefinition<T> innerGroup = null)
        {
            Selector = selector;
            InnerGroup = innerGroup;
        }

        public Func<T, object> Selector { get; private set; }
        public TableGroupDefinition<T> InnerGroup { get; private set; }

    }
}
