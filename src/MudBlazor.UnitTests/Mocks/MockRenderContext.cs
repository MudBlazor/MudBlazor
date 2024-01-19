#nullable enable

namespace MudBlazor.Services;

public class MockRenderContext : IRenderContext
{
    public bool IsInteractiveWebAssembly() => false;

    public bool IsInteractiveServer() => true;

    public bool IsStaticServer() => false;
}

