namespace MudBlazor.Utilities.Exceptions;

internal static class GenericTypeMismatchGuard
{
    public static void ThrowIfGenericTypeMismatch(
        object? parent,
        Type expectedParentGenericTypeDefinition,
        Type childType,
        string parentName,
        string childName)
    {
        if (parent is null)
        {
            return;
        }

        var parentType = parent.GetType();
        if (!parentType.IsGenericType || parentType.GetGenericTypeDefinition() != expectedParentGenericTypeDefinition)
        {
            return;
        }

        var parentGenericType = parentType.GenericTypeArguments[0];
        if (parentGenericType != childType)
        {
            throw new GenericTypeMismatchException(parentName, childName, parentGenericType, childType);
        }
    }
}
