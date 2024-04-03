using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace MudBlazor.Docs.Extensions;

#nullable enable
public static class ResourceSetExtensions
{
    public static IEnumerable<DictionaryEntry> ToEnumerable(this ResourceSet? resourceSet)
    {
        return resourceSet is null ? new List<DictionaryEntry>() : resourceSet.Cast<DictionaryEntry>();
    }
}
