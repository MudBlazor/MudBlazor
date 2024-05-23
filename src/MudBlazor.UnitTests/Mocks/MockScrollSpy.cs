using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockScrollSpyFactory : IScrollSpyFactory
    {
        private readonly MockScrollSpy _spy;

        public MockScrollSpyFactory(MockScrollSpy spy)
        {
            _spy = spy;
        }

        public MockScrollSpyFactory()
        {

        }

        public IScrollSpy Create() => _spy ?? new MockScrollSpy();
    }

    /// <summary>
    /// Mock for scroll spy
    /// </summary>
    public class MockScrollSpy : IScrollSpy
    {
        public bool SpyingInitiated { get; private set; }
        public string SpyingClassSelector { get; private set; }

        private List<string> _scrollHistory = new();
        public IReadOnlyList<string> ScrollHistory => _scrollHistory.AsReadOnly();

        public string CenteredSection { get; set; } = "my-item";

        public event EventHandler<ScrollSectionCenteredEventArgs> ScrollSectionSectionCentered;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task ScrollToSection(string id)
        {
            _scrollHistory.Add(id);
            FireScrollSectionSectionCenteredEvent(id);
            return Task.FromResult(true);
        }

        public Task SetSectionAsActive(string id)
        {
            _scrollHistory.Add(id);
            return Task.FromResult(true);
        }

        public Task ScrollToSection(Uri uri) => Task.FromResult(false);
        public Task StartSpying(string containerSelector, string sectionClassSelector)
        {
            SpyingInitiated = true;
            SpyingClassSelector = sectionClassSelector;

            return Task.FromResult(false);
        }

        public void FireScrollSectionSectionCenteredEvent(string centeredElementId) => ScrollSectionSectionCentered?.Invoke(this, new(centeredElementId));
    }
}
