// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.TestComponents
{
    public partial class TableMultiSelectionItemsTest1
    {
        [Parameter]
        public DateTime? StartDate { get => _dateRange.Start; set => _dateRange.Start = value; }
        [Parameter]
        public DateTime? EndDate { get => _dateRange.End; set => _dateRange.End = value; }

        public static string __description__ = "The selected items should not be cleared when the page changes or filters are applied.";
        private List<ComplexObject> _simulatedServerData = Enumerable
            .Range(1, 25)
            .Select(x => new ComplexObject
            {
                Id = x,
                DateTime = DateTime.Parse("2024-03-30 00:00:00").AddDays(x),
                Name = $"Test {x}",
                NestedObject = new NestedObject
                {
                    X = x,
                    Y = -x
                }
            })
            .ToList();
        private HashSet<ComplexObject> _selectedItems = new();
        private ElementComparer Comparer = new();
        private DateRange _dateRange { get; set; } = new();

        protected bool Filter(ComplexObject item)
            => (StartDate is null || item.DateTime > StartDate) && (EndDate is null || item.DateTime < EndDate);

        class ElementComparer : IEqualityComparer<ComplexObject>
        {
            public bool Equals(ComplexObject a, ComplexObject b) => a?.Id == b?.Id;
            public int GetHashCode(ComplexObject x) => HashCode.Combine(x?.Id);
        }

        public class ComplexObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime DateTime { get; set; }
            public NestedObject NestedObject { get; set; }
        }

        public class NestedObject
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
    }
}
