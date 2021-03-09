// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.Colors;

namespace MudBlazor.Utilities
{
    public static class RandomColorSelector
    {
        private static Random random = new Random();

        private static List<String> _colorValues = new List<string>();

        static RandomColorSelector()
        {
            var nestedTypes = typeof(Colors).GetNestedTypes();
            foreach (var item in nestedTypes)
            {
                if (item == typeof(Shades)) { continue; }

                var staticColorMembers = item.GetProperties(BindingFlags.Static | BindingFlags.Public);

                foreach (var color in staticColorMembers)
                {
                    String value = (String)color.GetValue(null);
                    _colorValues.Add(value);
                }
            }
        }

        public static String GetRandomColor()
        {
            Int32 index = random.Next(0, _colorValues.Count);
            return _colorValues[index];
        }
    }
}
