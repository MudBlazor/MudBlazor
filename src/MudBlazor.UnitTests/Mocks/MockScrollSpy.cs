using System;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    /// <summary>
    /// Mock for scroll spy
    /// </summary>
    public class MockScrollSpy : IScrollSpy
    {
        public string CenteredSection => "my-item";

        public event EventHandler<ScrollSectionCenteredEventArgs> ScrollSectionSectionCentered;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task ScrollToSection(string id) => Task.FromResult(true);
        public Task ScrollToSection(Uri uri) => Task.FromResult(false);
        public Task StartSpying(string elementsSelector) => Task.FromResult(false);

        public void FireScrollSectionSectionCenteredEvent(string centeredElementId) => ScrollSectionSectionCentered?.Invoke(this, new(centeredElementId));
    }
}
