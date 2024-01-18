#nullable enable

namespace MudBlazor.Services;

public class MockStaticRenderContext : IRenderContext
{
    public bool IsInteractiveWebAssembly() => false;

    public bool IsInteractiveServer() => true;

    public bool IsStatic() => true;
}

