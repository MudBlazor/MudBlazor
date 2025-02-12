// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;

namespace MudBlazor.UnitTests.TestComponents.Table
{
    public partial class TableMultiSelectionServerDataTest
    {
        public static string __description__ = "The selected items should not be cleared when the page changes or filters are applied.";

        private readonly List<ComplexObject> _simulatedServerData = Enumerable
            .Range(1, 50)
            .Select(x => new ComplexObject
            {
                Id = x,
                DateTime = DateTime.UtcNow.AddDays(x),
                Name = $"Test {x}",
                NestedObject = new NestedObject
                {
                    X = x,
                    Y = -x
                }
            })
            .ToList();
        private HashSet<ComplexObject> _selectedItems = new();
        private readonly ElementComparer _comparer = new();

        protected Task<TableData<ComplexObject>> ServerData(TableState state, CancellationToken token)
        {
            try
            {
                TableData<ComplexObject> data = new();
                data.TotalItems = _simulatedServerData.Count;
                // Serialize & deserialize to test a more real scenario where the references to the objects changes
                var jsonData = JsonSerializer.Serialize(_simulatedServerData);
                var items = JsonSerializer.Deserialize<List<ComplexObject>>(jsonData) ?? [];
                data.Items = items.Skip(state.PageSize * state.Page).Take(state.PageSize);

                return Task.FromResult(data);
            }
            catch
            {
                return Task.FromResult(new TableData<ComplexObject>());
            }
        }

        private class ElementComparer : IEqualityComparer<ComplexObject>
        {
            public bool Equals(ComplexObject? a, ComplexObject? b) => a?.Id == b?.Id;
            public int GetHashCode(ComplexObject x) => HashCode.Combine(x?.Id);
        }

        public class ComplexObject
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public DateTime DateTime { get; set; }

            public NestedObject? NestedObject { get; set; }
        }

        public class NestedObject
        {
            public float X { get; set; }

            public float Y { get; set; }
        }
    }
}
