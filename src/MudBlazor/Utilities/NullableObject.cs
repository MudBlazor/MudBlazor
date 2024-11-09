using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.Utilities;

#nullable enable
/// <summary>
/// Represents a wrapper for an object that can be null.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
public readonly struct NullableObject<T> : IEquatable<NullableObject<T>>, IEquatable<T>
{
    /// <summary>
    /// Gets the wrapped item.
    /// </summary>
    public T? Item { get; }

    /// <summary>
    /// Gets a value indicating whether the wrapped item is null.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Item))]
    public bool IsNull => Item is null;

    /// <summary>
    /// Initializes a new instance of the <see cref="NullableObject{T}"/> struct.
    /// </summary>
    /// <param name="item">The item to wrap.</param>
    public NullableObject(T? item)
    {
        Item = item;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string? ToString()
    {
        return Item is not null
            ? Item.ToString()
            : "NULL";
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
    public bool Equals(NullableObject<T> other)
    {
        if (other.IsNull)
        {
            return IsNull;
        }

        if (IsNull)
        {
            // other is not null and this is null, therefore they are not equal
            return false;
        }

        return EqualityComparer<T?>.Default.Equals(Item, other.Item);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
    public bool Equals(T? other)
    {
        if (other is null)
        {
            return IsNull;
        }

        if (IsNull)
        {
            // other is not null and this is null, therefore they are not equal
            return false;
        }

        return EqualityComparer<T?>.Default.Equals(Item, other);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <remarks>
    /// If you compare two <see cref="NullableObject{T}"/> instances with different generic types and both are null, 
    /// the <see cref="Equals(object?)"/> method will return false because they are considered two different null values.
    /// There is a type match check.
    /// </remarks>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            NullableObject<T> nullObject => Equals(nullObject),
            T item => Equals(item),
            _ => false
        };
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return IsNull
            ? 0
            : EqualityComparer<T>.Default.GetHashCode(Item);
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="NullableObject{T}"/> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if the left and right parameters are equal; otherwise, false.</returns>
    public static bool operator ==(NullableObject<T> left, NullableObject<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two specified instances of <see cref="NullableObject{T}"/> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if the left and right parameters are not equal; otherwise, false.</returns>
    public static bool operator !=(NullableObject<T> left, NullableObject<T> right) => !(left == right);

    /// <summary>
    /// Performs an implicit conversion from <see cref="NullableObject{T}"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="nullObject">The <see cref="NullableObject{T}"/> to convert.</param>
    public static implicit operator T?(NullableObject<T> nullObject) => nullObject.Item;

    /// <summary>
    /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="NullableObject{T}"/>.
    /// </summary>
    /// <param name="item">The item to convert.</param>
    public static implicit operator NullableObject<T>(T? item) => new(item);

    /// <summary>
    /// Gets a <see cref="NullableObject{T}"/> that represents a null value.
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="T"/> is a struct that is not wrapped in <see cref="Nullable{T}"/>, 
    /// this property will return a <see cref="NullableObject{T}"/> with a non-null default value 
    /// because structs cannot be null unless wrapped in <see cref="Nullable{T}"/>.
    /// </remarks>
    public static NullableObject<T> Null { get; } = new(default);
}
