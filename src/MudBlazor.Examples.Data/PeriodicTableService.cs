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
        public async Task<IEnumerable<Element>> GetElements()
        {
            return await GetElements(string.Empty);
        }

        public async Task<IEnumerable<Element>> GetElements(string search = "")
        {
            var elements = new List<Element>();
            var key = GetResourceKey(typeof(PeriodicTableService).Assembly, "Elements.json");
            using var stream = typeof(PeriodicTableService).Assembly.GetManifestResourceStream(key);
            var table = await JsonSerializer.DeserializeAsync<Table>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            foreach (var elementGroup in table.ElementGroups)
            {
                elements = elements.Concat(elementGroup.Elements).ToList();
            }

            if (search == string.Empty)
                return elements;
            else
                return elements.Where(elm => (elm.Sign + elm.Name).Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetResourceKey(Assembly assembly, string embeddedFile)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));
        }
    }
}
