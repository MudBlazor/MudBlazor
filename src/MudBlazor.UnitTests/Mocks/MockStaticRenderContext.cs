#nullable enable

namespace MudBlazor.Services;

public class MockStaticRenderContext : IRenderContext
{
    public bool IsInteractiveWebAssembly() => false;

    public bool IsInteractiveServer() => false;

    public bool IsStaticServer() => true;
}

