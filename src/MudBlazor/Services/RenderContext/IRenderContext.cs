#nullable enable

namespace MudBlazor.Services;

public interface IRenderContext
{
    bool IsInteractiveWebAssembly();

    bool IsInteractiveServer();

    bool IsStatic();
}

