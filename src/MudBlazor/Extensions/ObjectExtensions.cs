namespace MudBlazor.Extensions
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object self)
            => self is not T selfT
                ? default
                : selfT;
    }
}
