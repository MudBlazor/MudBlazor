using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using MudBlazor.ExampleData.Models;

namespace MudBlazor.ExampleData
{
    public static class PeriodicTable
    {
        private static IList<Element> s_elements = null;
        private static DateTime s_loadTime;

        public static IEnumerable<Element> GetElements()
        {
            return GetElements(string.Empty);
        }

        public static IEnumerable<Element> GetElements(string search)
        {
            var task = GetElementsAsync(search);
            return task.GetAwaiter().GetResult();
        }

        public static async Task<IEnumerable<Element>> GetElementsAsync()
        {
            return await GetElementsAsync(string.Empty);
        }

        public static async Task<IEnumerable<Element>> GetElementsAsync(string search)
        {
            if (s_elements == null || s_loadTime.Add(TimeSpan.FromMinutes(5)) < DateTime.Now)
            {
                s_elements = await LoadElements();
            }

            if (!string.IsNullOrEmpty(search))
            {
                return s_elements
                    .Where(e => $"{e.Name.ToUpper()} ({e.Sign.ToUpper()})"
                    .Contains(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                return s_elements;
            }
        }

        private static async Task<IList<Element>> LoadElements()
        {
            s_loadTime = DateTime.Now;
            s_elements = new List<Element>();
            var key = GetResourceKey(typeof(PeriodicTable).Assembly, "Elements.json");
            using var stream = typeof(PeriodicTable).Assembly.GetManifestResourceStream(key);
            var table = await JsonSerializer.DeserializeAsync<Table>(stream, new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true });
            foreach (var elementGroup in table.ElementGroups)
            {
                s_elements = s_elements.Concat(elementGroup.Elements).ToList();
            }

            return s_elements;
        }

        public static string GetResourceKey(Assembly assembly, string embeddedFile)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));
        }
    }
}
