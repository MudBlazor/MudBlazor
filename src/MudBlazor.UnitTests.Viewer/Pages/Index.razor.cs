using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public NavigationManager NavManager { get; set; } = null!;

    [Inject]
    public ISearchService SearchService { get; set; } = null!;

    private bool _rightToLeft;
    private Type? _selectedType;
    private bool _drawerOpen = true;
    private string _searchText = string.Empty;
    private TestEntry[] _entries = [];
    private TestEntry[] _filteredEntries = [];
    private Type[] _availableComponentTypes = [];
    private Dictionary<Type, TestEntry> _entryByType = [];
    private int _remainingResultsCount;
    private bool _showAllResults;
    private CancellationTokenSource? _searchCts;
    private bool _isDarkMode;
    private readonly MudTheme _customTheme = new()
    {
        LayoutProperties = new LayoutProperties
        {
            DrawerWidthLeft = "340px",
            DrawerWidthRight = "340px"
        }
    };

    private void ToggleTheme() => _isDarkMode = !_isDarkMode;

    private string ThemeIcon => _isDarkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;

    private string ThemeLabel => _isDarkMode ? "Light mode" : "Dark mode";

    private string SearchText
    {
        get => _searchText;
        set
        {
            value ??= string.Empty;

            if (string.Equals(_searchText, value, StringComparison.Ordinal))
            {
                return;
            }

            _searchText = value;
            _showAllResults = false;
            StartSearch();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _availableComponentTypes = IndexTestComponentCatalog.GetTestComponentTypes().ToArray();

        _entries = _availableComponentTypes
            .Select(IndexTestComponentCatalog.CreateEntry)
            .OrderBy(x => x.Name)
            .ToArray();

        _entryByType = _entries.ToDictionary(x => x.Type, x => x);
        StartSearch();

        ParseQueryString();
        NavManager.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        ParseQueryString();
        StateHasChanged();
    }

    public void Dispose()
    {
        NavManager.LocationChanged -= HandleLocationChanged;
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }

    private void ParseQueryString()
    {
        if (IndexQueryString.TryGetSelectedComponentType(NavManager.Uri, _availableComponentTypes, out var componentType))
        {
            _selectedType = componentType;
            StateHasChanged();
        }
    }

    private void Select(Type componentType)
    {
        if (componentType is null)
        {
            return;
        }

        _selectedType = componentType;
        UpdateQueryString(componentType);
    }

    private void UpdateQueryString(Type componentType)
    {
        if (componentType == null) return;

        NavManager.NavigateTo(IndexQueryString.CreateComponentUrl(NavManager.Uri, componentType), false);
    }

    private RenderFragment TestComponent() => builder =>
    {
        if (_selectedType is null)
        {
            return;
        }

        builder.OpenComponent(0, _selectedType);
        builder.CloseComponent();
    };

    private string GetDescriptionForDisplay(Type type)
    {
        if (_entryByType.TryGetValue(type, out var entry) && !string.IsNullOrWhiteSpace(entry.Description))
        {
            return entry.Description;
        }

        return "No description provided (add public static string __description__).";
    }

    private string GetFilePathForDisplay(Type type)
    {
        if (_entryByType.TryGetValue(type, out var entry))
        {
            return entry.FilePath;
        }

        return string.Empty;
    }

    private string GetTitleForDisplay(Type type)
    {
        if (_entryByType.TryGetValue(type, out var entry))
        {
            return entry.DisplayName;
        }

        return IndexTestComponentCatalog.GetDisplayName(type.Name);
    }

    private void StartSearch()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        _ = RecomputeFilteredEntriesAsync(_searchCts.Token);
    }

    private async Task RecomputeFilteredEntriesAsync(CancellationToken token)
    {
        try
        {
            _remainingResultsCount = 0;

            if (_entries.Length == 0)
            {
                _filteredEntries = [];
                await InvokeAsync(StateHasChanged);
                return;
            }

            token.ThrowIfCancellationRequested();

            var query = _searchText.Trim();

            if (query.Length == 0)
            {
                _filteredEntries = _entries;
                await InvokeAsync(StateHasChanged);
                return;
            }

            await Task.Yield();
            token.ThrowIfCancellationRequested();

            const int defaultLimit = 20;
            var results = SearchService.Search(_entries, e => new[] { e.Name, e.Category }, query);
            _filteredEntries = _showAllResults ? [.. results] : [.. results.Take(defaultLimit)];
            _remainingResultsCount = _showAllResults ? 0 : Math.Max(0, results.Count - _filteredEntries.Length);

            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException)
        {
            // Ignore; newer search already started.
        }
    }

    private void ShowAllResults(MouseEventArgs args)
    {
        _showAllResults = true;
        StartSearch();
    }

    private string GetEntryClass(TestEntry entry)
    {
        if (_selectedType == entry.Type)
        {
            return "test-viewer-entry test-viewer-entry-selected";
        }

        return "test-viewer-entry";
    }
}
