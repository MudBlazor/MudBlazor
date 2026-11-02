using MudBlazor.Docs.Services;

namespace MudBlazor.UnitTests.Mocks;

public class MockAdBlockDetectionService : IAdBlockDetectionService
{
    public ValueTask<bool> IsAdBlockedAsync(int waitMilliseconds = 2000) => ValueTask.FromResult(false);
}
