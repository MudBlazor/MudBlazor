namespace MudBlazor.Extensions
{
#nullable enable
    public static class EnumExtensions
    {
        /// <summary>
        /// Universal method that retrieves an array of the values of the constant in specified enumeration, works with nullable and non-nullable enums.
        /// Original <see cref="Enum.GetValues"/> works only with non-nullable enums and will throw exception.
        /// </summary>
        /// <returns>An array that contains the values of constant in type</returns>
        internal static IEnumerable<Enum> GetSafeEnumValues(Type? type)
        {
            if (type is null)
            {
                return [];
            }

            if (type.IsEnum)
            {
                return Enum.GetValues(type).Cast<Enum>();
            }

            if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                var actualType = type.GetGenericArguments()[0];
                return Enum.GetValues(actualType).Cast<Enum>();
            }

            return [];
        }

        /// <summary>
        /// Converts an <see cref="Adornment"/> to its corresponding <see cref="Edge"/> value.
        /// </summary>
        /// <param name="adornment">The adornment value to convert.</param>
        /// <returns>The corresponding <see cref="Edge"/> value.</returns>
        internal static Edge ToEdge(this Adornment adornment)
        {
            return adornment switch
            {
                Adornment.Start => Edge.Start,
                Adornment.End => Edge.End,
                _ => Edge.False
            };
        }
    }
}
