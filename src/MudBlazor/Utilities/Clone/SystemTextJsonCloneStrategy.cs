// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudBlazor.Utilities.Clone;

#nullable enable
/// <summary>
/// Provides a deep copy implementation using System.Text.Json.
/// </summary>
/// <remarks>
/// This implementation is <b>not</b> trim safe.
/// Use different strategy or use System Text Json with <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-7-0">source generator</see> and pass <see cref="JsonSerializerContext"/> of your object.
/// </remarks>
/// <typeparam name="T">The type of the object to be deep-copied.</typeparam>
public sealed class SystemTextJsonDeepCloneStrategy<T> : ICloneStrategy<T>
{
    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026: Using member 'System.Text.Json.JsonSerializer.Deserialize<T>(string, System.Text.Json.JsonSerializerOptions?)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.", Justification = "Suppressing because T is a type supplied by the user and it is unlikely that it is not referenced by their code.")]
    public T? CloneObject(T item) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(item));

    /// <summary>
    /// Represents a static field providing an instance of <see cref="SystemTextJsonDeepCloneStrategy{T}"/>.
    /// </summary>
    public static readonly ICloneStrategy<T> Instance = new SystemTextJsonDeepCloneStrategy<T>();
}
