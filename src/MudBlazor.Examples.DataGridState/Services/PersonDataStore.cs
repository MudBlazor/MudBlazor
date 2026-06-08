using MudBlazor.Examples.DataGridState.Data;

namespace MudBlazor.Examples.DataGridState.Services;

/// <summary>
/// Keeps the generated people data in memory for the current app session
/// and persists it to localStorage so refreshes reuse the same records.
/// </summary>
public sealed class PersonDataStore(LocalStorageService localStorage)
{
    public const string DataStorageKey = "mudblazor.datagrid-state.demo.people";

    private IReadOnlyList<Person> _people = [];
    private bool _initialized;

    public IReadOnlyList<Person> People => _people;

    public bool IsInitialized => _initialized;

    public async Task<IReadOnlyList<Person>> EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return _people;
        }

        var people = await localStorage.GetItemAsync<List<Person>>(DataStorageKey);
        if (people is not { Count: > 0 })
        {
            people = [.. PersonDataGenerator.CreatePeople()];
            await localStorage.SetItemAsync(DataStorageKey, people);
        }

        _people = people;
        _initialized = true;
        return _people;
    }

    public async Task ResetAsync()
    {
        var people = PersonDataGenerator.CreatePeople().ToList();
        await localStorage.SetItemAsync(DataStorageKey, people);
        _people = people;
        _initialized = true;
    }
}
