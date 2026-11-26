using System.Linq;
using System.Reflection;

namespace MudBlazor.Docs.Models
{
    // this is needed for the copy-to-clipboard feature
    public static partial class Snippets
    {
        public static string GetCode(string component)
        {
            var field = typeof(Snippets).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .FirstOrDefault(f => f.Name == component);
            if (field == null)
                return null;
            return (string)field.GetValue(null);
        }
    }
}
