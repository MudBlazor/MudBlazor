using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MudBlazor.Docs.Data
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
            using (var stream = typeof(PeriodicTable).Assembly.GetManifestResourceStream(key))
            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                var periodicTable = await JObject.LoadAsync(reader);
                foreach (var row in periodicTable["table"].Values<JObject>())
                {
                    foreach (var el in row["elements"].Values<JObject>())
                        s_elements.Add(el.ToObject<Element>());
                }
            }

            return s_elements;
        }

        public static string GetResourceKey(Assembly assembly, string embeddedFile)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));
        }
    }
}
