using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MudBlazor.UnitTests.Shared.Mocks;

public class MockNavigationManager
    : NavigationManager
{
    public MockNavigationManager() =>
        Initialize("http://localhost:2112/", "http://localhost:2112/test");

    public MockNavigationManager(string baseUri, string uri) =>
        Initialize(baseUri, uri);

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        WasNavigateInvoked = true;
        Uri = ToAbsoluteUri(uri).ToString();
        NotifyLocationChanged(false);
    }

    public bool WasNavigateInvoked { get; private set; }

    protected override void EnsureInitialized()
    {
        Initialize("http://localhost:2112/", "http://localhost:2112/test");
    }
}
