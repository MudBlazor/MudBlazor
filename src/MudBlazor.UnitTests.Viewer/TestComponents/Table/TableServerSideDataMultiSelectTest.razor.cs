// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.TestComponents
{
#pragma warning disable CS1998 // async without await

    public partial class TableServerSideDataMultiSelectTest
    {
        #region COMPLEX TYPE TEST

        public static string __description__ = "The selected items should not be cleared when the page changes or filters are applied.";
        private List<ComplexObject> _simulatedServerData = Enumerable
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
        private ElementComparer Comparer = new();

        protected async Task<TableData<ComplexObject>> ServerData(TableState state)
        {
            try
            {
                TableData<ComplexObject> data = new();
                data.TotalItems = _simulatedServerData.Count;
                // Serialize & deserialize to test a more real scenario where the references to the objects changes
                string jsonData = JsonSerializer.Serialize(_simulatedServerData);
                data.Items = JsonSerializer.Deserialize<List<ComplexObject>>(jsonData).Skip(state.PageSize * state.Page).Take(state.PageSize);
                return data;
            }
            catch
            {
                return new();
            }
        }

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

        #endregion

        #region BASIC TYPE DATA TEST

        private List<string> _simulatedServerDataBasicType = Enumerable.Range(1, 50).Select(x => $"Test {x}").ToList();
        private HashSet<string> _selectedItemsBasicType = new();

        protected async Task<TableData<string>> ServerDataBasicType(TableState state)
        {
            try
            {
                TableData<string> data = new();
                data.TotalItems = _simulatedServerDataBasicType.Count;
                // Serialize & deserialize to test a more real scenario where the references to the objects is not the same
                string jsonData = JsonSerializer.Serialize(_simulatedServerDataBasicType);
                data.Items = JsonSerializer.Deserialize<List<string>>(jsonData).Skip(state.PageSize * state.Page).Take(state.PageSize);
                return data;
            }
            catch
            {
                return new();
            }
        }

        #endregion
    }
}
