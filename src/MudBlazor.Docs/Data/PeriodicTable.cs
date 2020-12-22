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
        private static IList<Element> _elements = null;
        private static DateTime _loadTime;

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
            if (_elements == null || _loadTime.Add(TimeSpan.FromMinutes(5)) < DateTime.Now)
            {
                _elements = await LoadElements();
            }

            if (!string.IsNullOrEmpty(search))
            {
                return _elements
                    .Where(e => $"{e.Name.ToUpper()} ({e.Sign.ToUpper()})"
                    .Contains(search.ToUpper(), StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                return _elements;
            }
        }

        private static async Task<IList<Element>> LoadElements()
        {
            _loadTime=DateTime.Now;
            _elements =new List<Element>();
            var key = GetResourceKey(typeof(PeriodicTable).Assembly, "Elements.json");
            using (Stream stream = typeof(PeriodicTable).Assembly.GetManifestResourceStream(key))
            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                var periodicTable = await JObject.LoadAsync(reader);
                foreach (var row in periodicTable["table"].Values<JObject>())
                {
                    foreach(var el in row["elements"].Values<JObject>())
                        _elements.Add(el.ToObject<Element>());
                }
            }

            return _elements;
        }

        public static string GetResourceKey(Assembly assembly, string embeddedFile)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));
        }
    }
}
