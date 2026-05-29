using System.Collections;
using System.Resources;

namespace MudBlazor.UnitTests;

#nullable enable
public static class ResourceSetExtensions
{
    public static IEnumerable<DictionaryEntry> ToEnumerable(this ResourceSet? resourceSet)
    {
        return resourceSet is null ? new List<DictionaryEntry>() : resourceSet.Cast<DictionaryEntry>();
    }
}
