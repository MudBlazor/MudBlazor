using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor.Examples.Data.Models;

namespace MudBlazor.Examples.Data
{
    public class PeriodicTableService : IPeriodicTableService
    {
        private static readonly Table _table = null;
        private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

        static PeriodicTableService()
        {
            var key = GetResourceKey(typeof(PeriodicTableService).Assembly, "Elements.json");
            using var stream = typeof(PeriodicTableService).Assembly.GetManifestResourceStream(key);
            _table = JsonSerializer.Deserialize<Table>(stream, _serializerOptions);
        }

        public static string GetResourceKey(Assembly assembly, string embeddedFile)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));
        }

        public Task<IEnumerable<Element>> GetElements()
        {
            return GetElements(string.Empty);
        }

        public async Task<IEnumerable<Element>> GetElements(string search = "")
        {
            var elements = new List<Element>();
            foreach (var elementGroup in _table.ElementGroups)
            {
                elements = elements.Concat(elementGroup.Elements).ToList();
            }

            if (search == string.Empty)
                return await Task.FromResult(elements);
            else
                return elements.Where(elm => (elm.Sign + elm.Name).Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
