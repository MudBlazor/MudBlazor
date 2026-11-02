namespace MudBlazor.Extensions
{
    public static class ObjectExtensions
    {
        public static T? As<T>(this object? self)
        {
            if (self is T selfT)
            {
                return selfT;
            }

            return default;
        }
    }
}
