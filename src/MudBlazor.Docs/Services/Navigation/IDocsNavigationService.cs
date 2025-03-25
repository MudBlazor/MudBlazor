using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services;

public interface IDocsNavigationService
{
    NavigationFooterLink Next { get; }

    NavigationFooterLink Previous { get; }

    NavigationSection Section { get; }
}
