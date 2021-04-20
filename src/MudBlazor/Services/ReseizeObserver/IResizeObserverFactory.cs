namespace MudBlazor.Services
{
    public interface IResizeObserverFactory
    {
        IResizeObserver Create(ResizeObserverOptions options);
    }
}
