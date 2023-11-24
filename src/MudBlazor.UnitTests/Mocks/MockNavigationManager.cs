using Microsoft.AspNetCore.Components;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockNavigationManager
        : NavigationManager
    {
        public MockNavigationManager() : base() =>
            this.Initialize("http://localhost:2112/", "http://localhost:2112/test");

        public MockNavigationManager(string baseUri, string uri) : base() =>
            this.Initialize(baseUri, uri);

        protected override void NavigateToCore(string uri, bool forceLoad) =>
            this.WasNavigateInvoked = true;

        public bool WasNavigateInvoked { get; private set; }
    }
}
