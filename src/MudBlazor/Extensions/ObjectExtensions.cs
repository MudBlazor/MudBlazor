namespace MudBlazor.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="object"/> that perform a safe cast to a target type, returning the default value when the cast fails.
    /// </summary>
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
