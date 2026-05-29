using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor.Examples.Data.Models;

namespace MudBlazor.Examples.Data;

public class PeriodicTableService : IPeriodicTableService
{
    private static readonly Table? _table;
    private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    static PeriodicTableService()
    {
        var key = GetResourceKey(typeof(PeriodicTableService).Assembly, "Elements.json");
        if (key is not null)
        {
            using var stream = typeof(PeriodicTableService).Assembly.GetManifestResourceStream(key);
            if (stream is not null)
            {
                _table = JsonSerializer.Deserialize<Table>(stream, _serializerOptions);
            }
        }
    }

    public static string? GetResourceKey(Assembly assembly, string embeddedFile) => assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(embeddedFile));

    public Task<IEnumerable<Element>> GetElements() => GetElements(string.Empty);

    public async Task<IEnumerable<Element>> GetElements(string search)
    {
        var elements = new List<Element>();
        foreach (var elementGroup in _table?.ElementGroups ?? ReadOnlyCollection<ElementGroup>.Empty)
        {
            elements = [.. elements, .. elementGroup.Elements ?? ReadOnlyCollection<Element>.Empty];
        }

        if (string.IsNullOrEmpty(search))
        {
            return await Task.FromResult(elements);
        }

        return elements.Where(elm => (elm.Sign + elm.Name).Contains(search, StringComparison.InvariantCultureIgnoreCase));
    }
}
