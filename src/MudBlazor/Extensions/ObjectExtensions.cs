namespace MudBlazor.Extensions
{
    public static class ObjectExtensions
    {

        public static T As<T>(this object self)
        {
            if (self == null || !(self is T))
                return default;
            return (T)self;
        }
    }
}
