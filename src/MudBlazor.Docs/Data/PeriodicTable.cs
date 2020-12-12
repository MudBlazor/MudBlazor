using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MudBlazor.Docs.Data
{
    public static class PeriodicTable
    {
        private static List<Element> _elements = null;
        private static DateTime _loadTime;

        public static IEnumerable<Element> GetElements()
        {
            if (_elements == null || _loadTime.Add(TimeSpan.FromMinutes(5)) < DateTime.Now)
                LoadElements();
            return _elements;
        }

        private static void LoadElements()
        {
            _loadTime=DateTime.Now;
            _elements =new List<Element>();
            var key = GetResourceKey(typeof(PeriodicTable).Assembly, "Elements.json");
            using (Stream stream = typeof(PeriodicTable).Assembly.GetManifestResourceStream(key))
            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                var periodicTable = JObject.Load(reader);
                foreach (var row in periodicTable["table"].Values<JObject>())
                {
                    foreach(var el in row["elements"].Values<JObject>())
                        _elements.Add(el.ToObject<Element>());
                }
            }
        }


        public static string GetResourceKey(Assembly assembly, string embedded_file)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embedded_file));
        }
    }
}
