// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Linq;

namespace MudBlazor.Docs.WasmHost.Prerender;

public class LimitedConcurrentDictionary<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _dict = new();

    private int MaxCapacity { get; }

    public LimitedConcurrentDictionary(int maxCapacity)
    {
        MaxCapacity = maxCapacity;
    }

    public TValue this[TKey key] => _dict[key];

    public bool ContainsKey(TKey value) => _dict.ContainsKey(value);

    public void TryAdd(TKey key, TValue value)
    {
        while (_dict.Count > MaxCapacity)
        {
            _dict.TryRemove(_dict.First());
        }

        _dict.TryAdd(key, value);
    }
}
