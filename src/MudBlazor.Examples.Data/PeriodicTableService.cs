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
        private static readonly Table s_table = null;

        static PeriodicTableService()
        {
            var key = GetResourceKey(typeof(PeriodicTableService).Assembly, "Elements.json");
            using var stream = typeof(PeriodicTableService).Assembly.GetManifestResourceStream(key);
            s_table = JsonSerializer.Deserialize<Table>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
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
            foreach (var elementGroup in s_table.ElementGroups)
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
