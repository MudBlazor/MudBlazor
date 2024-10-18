﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text.RegularExpressions;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a generator of HTML documentation based on XML documentation files.
/// </summary>
/// <remarks>
/// <para>
/// This class documents the MudBlazor assembly, including all public types, properties, methods, events, and fields.  Inherited
/// members are supported, as well as "see cref" links.  Once all documentation has been loaded, several types are made available 
/// to the <c>MudBlazor.Docs</c> such as <see cref="DocumentedType"/>, <see cref="DocumentedMethod"/>, <see cref="DocumentedProperty"/>,
/// <see cref="DocumentedEvent"/>, and <see cref="DocumentedField"/>, in a strongly typed manner. 
/// </para>
/// </remarks>
public partial class ApiDocumentationBuilder()
{
    /// <summary>
    /// The assembly to document.
    /// </summary>
    public List<Assembly> Assemblies { get; private set; } = [typeof(_Imports).Assembly];

    /// <summary>
    /// The reader for XML documentation.
    /// </summary>
    private DocXmlReader _xmlDocs;

    /// <summary>
    /// The types in the assembly.
    /// </summary>
    public SortedDictionary<string, Type> PublicTypes { get; private set; } = [];

    /// <summary>
    /// The generated documentation for events.
    /// </summary>
    public SortedDictionary<string, DocumentedEvent> Events { get; private set; } = [];

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public SortedDictionary<string, DocumentedField> Fields { get; private set; } = [];

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public SortedDictionary<string, DocumentedType> Types { get; private set; } = [];

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public SortedDictionary<string, DocumentedProperty> Properties { get; private set; } = [];

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public SortedDictionary<string, DocumentedMethod> Methods { get; private set; } = [];

    /// <summary>
    /// Any types to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedTypes { get; private set; } =
    [
        "ActivatableCallback",
        "AbstractLocalizationInterceptor",
        "CloneableCloneStrategy`1",
        "CssBuilder",
        "MudBlazor._Imports",
        "MudBlazor.CategoryAttribute",
        "MudBlazor.CategoryTypes",
        "MudBlazor.CategoryTypes+",
        "MudBlazor.Colors",
        "MudBlazor.Colors+",
        "MudBlazor.Icons",
        "MudBlazor.Icons+",
        "MudBlazor.LabelAttribute",
        "MudBlazor.Resources.LanguageResource",
        "object",
        "string"
    ];

    /// <summary>
    /// Any methods to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedMembers { get; private set; } =
    [
        // Object methods
        "ToString",
        "Equals",
        "MemberwiseClone",
        "GetHashCode",
        "GetType",
        // Enum methods
        "CompareTo",
        "GetTypeCode",
        "GetValue",
        "HasFlag",
        // Operators
        "op_Equality",
        "op_Inequality",
        "op_Implicit",
        "op_Explicit",
        // Constructors
        "#ctor",
        // Blazor component methods
        "BuildRenderTree",
        "InvokeAsync",
        "OnAfterRender",
        "OnAfterRenderAsync",
        "OnInitialized",
        "OnInitializedAsync",
        "OnParametersSet",
        "OnParametersSetAsync",
        "StateHasChanged",
        "ShouldRender",
        // Dispose methods
        "Dispose",
        "DisposeAsync",
        "Finalize",
        // Internal MudBlazor methods
        "FieldId",
        "Logger",
        "IsJSRuntimeAvailable",
        "SetParametersAsync",
        "DispatchExceptionAsync",
        "CreateRegisterScope",
        "DetectIllegalRazorParametersV7",
        "MudBlazor.Interfaces.IMudStateHasChanged.StateHasChanged",
        "ParameterContainer",
        "InputIdState",
    ];

    /// <summary>
    /// Gets whether a type is excluded from documentation.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>When <c>true</c>, the type is excluded from documentation.</returns>
    public static bool IsExcluded(Type type)
    {
        if (ExcludedTypes.Contains(type.Name))
        {
            return true;
        }
        if (type.FullName != null && ExcludedTypes.Contains(type.FullName))
        {
            return true;
        }
        if (type.FullName != null && ExcludedTypes.Any(excludedType => type.FullName.StartsWith(excludedType)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets whether a type is excluded from documentation.
    /// </summary>
    /// <param name="member">The type to check.</param>
    /// <returns>When <c>true</c>, the type is excluded from documentation.</returns>
    public static bool IsExcluded(MemberInfo member)
    {
        if (ExcludedMembers.Contains(member.Name))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Generates documentation for all types.
    /// </summary>
    public bool Execute()
    {
        _xmlDocs = new(Assemblies);
        AddTypesToDocument();
        FindDeclaringTypes();
        AddGlobalsToDocument();
        ExportApiDocumentation();
        CalculateDocumentationCoverage();
        return true;
    }

    /// <summary>
    /// Adds an empty documented type for each public type.
    /// </summary>
    public void AddTypesToDocument()
    {
        foreach (var assembly in Assemblies)
        {
            // Document all public types
            var typesToDocument = assembly.GetTypes()
                .Where(type =>
                    // Include public types
                    type.IsPublic
                    // ... which aren't excluded
                    && !IsExcluded(type)
                    // ... which aren't interfaces
                    && !type.IsInterface
                    // ... which aren't source generators
                    && !type.Name.Contains("SourceGenerator")
                    // ... which aren't extension classes
                    && !type.Name.Contains("Extensions"))
                .ToList();
            foreach (var type in typesToDocument)
            {
                PublicTypes.Add(type.Name, type);
            }
        }
        // Now build all public members
        foreach (var pair in PublicTypes)
        {
            AddTypeToDocument(pair.Value);
        }
    }

    /// <summary>
    /// Adds the specified type and any related public types.
    /// </summary>
    /// <param name="type">The type to add.</param>
    public DocumentedType AddTypeToDocument(Type type)
    {
        // Is the type already documented?
        if (!Types.TryGetValue(type.FullName, out var documentedType))
        {
            // Look up the XML documentation
            var typeXmlDocs = _xmlDocs.GetTypeComments(type);

            // No.  Add it
            documentedType = new DocumentedType()
            {
                BaseType = type.BaseType,
                IsPublic = type.IsPublic,
                IsAbstract = type.IsNestedFamORAssem,
                Key = type.FullName,
                Name = type.Name,
                Remarks = typeXmlDocs.Remarks?.Replace("\r\n", "").Trim(),
                Summary = typeXmlDocs.Summary?.Replace("\r\n", "").Trim(),
                Type = type,
            };

            // Add the root-level type
            Types.Add(type.FullName, documentedType);

            // Record properties, methods, fields, and events            
            AddPropertiesToDocument(type, documentedType);
            AddMethodsToDocument(type, documentedType);
            AddFieldsToDocument(type, documentedType);
            AddEventsToDocument(type, documentedType);

            // Look for binable properties
            FindBindableProperties(documentedType);

            // Also add nested types            
            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public))
            {
                AddTypeToDocument(nestedType);
            }
        }

        return documentedType;
    }

    /// <summary>
    /// Adds public properties for the specified type.
    /// </summary>
    /// <param name="type"></param>
    public void AddPropertiesToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var properties = type.GetProperties().ToList();
        // Add protected methods
        properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove private and backing fields
        properties.RemoveAll(property => property.GetMethod.IsPrivate || IsExcluded(property));
        // Remove duplicates
        properties = properties.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var property in properties)
        {
            var category = property.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = property.GetCustomAttribute<ParameterAttribute>();
            var key = GetPropertyFullName(property);

            // Is this an event?
            if (property.PropertyType.Name.StartsWith("EventCallback"))
            {
                // Has this event been documented before?
                if (!Events.TryGetValue(key, out var documentedEvent))
                {
                    // No.  Get the XML documentation
                    var xmlDocs = _xmlDocs.GetMemberComments(property);

                    // Record this event
                    documentedEvent = new DocumentedEvent()
                    {
                        Category = category?.Name,
                        DeclaringType = property.DeclaringType,
                        IsProtected = property.GetMethod.IsFamily,
                        IsParameter = blazorParameter != null,
                        Key = key,
                        Name = property.Name,
                        Order = category?.Order,
                        Remarks = xmlDocs.Remarks?.Replace("\r\n", "").Trim(),
                        Summary = xmlDocs.Summary?.Replace("\r\n", "").Trim(),
                        Type = property.PropertyType,
                    };
                    Events.Add(key, documentedEvent);
                }
                // Link the event to the type
                documentedType.Events.Add(documentedEvent.Key, documentedEvent);
            }
            else
            {
                // Has this property been documented before?
                if (!Properties.TryGetValue(key, out var documentedProperty))
                {
                    // No.  Get the XML documentation
                    var xmlDocs = _xmlDocs.GetMemberComments(property);

                    // Record this property                
                    documentedProperty = new DocumentedProperty()
                    {
                        Category = category?.Name,
                        DeclaringType = property.DeclaringType,
                        IsProtected = property.GetMethod.IsFamily,
                        IsParameter = blazorParameter != null,
                        Key = key,
                        Name = property.Name,
                        Order = category?.Order,
                        Remarks = xmlDocs.Remarks?.Replace("\r\n", "").Trim(),
                        Summary = xmlDocs.Summary?.Replace("\r\n", "").Trim(),
                        Type = property.PropertyType,
                    };
                    Properties.Add(key, documentedProperty);
                }
                // Link the property to the type
                documentedType.Properties.Add(documentedProperty.Key, documentedProperty);
            }
        }
    }

    /// <summary>
    /// Adds fields for the specified type.
    /// </summary>
    /// <param name="type">The type to examine.</param>
    public void AddFieldsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var fields = type.GetFields().ToList();
        // Add protected methods
        fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove private and backing fields
        fields.RemoveAll(field => field.Name.Contains("k__BackingField") || field.Name == "value__" || field.Name.StartsWith('_') || field.IsPrivate || IsExcluded(field));
        // Remove duplicates
        fields = fields.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var field in fields)
        {
            var category = field.GetCustomAttribute<CategoryAttribute>();
            var key = GetFieldFullName(field);

            // Has this property been documented before?
            if (!Fields.TryGetValue(key, out var documentedField))
            {
                // No.  Get the XML documentation
                var xmlDocs = _xmlDocs.GetMemberComments(field);

                // Record this property
                documentedField = new DocumentedField()
                {
                    Category = category?.Name,
                    DeclaringType = field.DeclaringType,
                    IsProtected = field.IsFamily,
                    Key = key,
                    Name = field.Name,
                    Order = category?.Order,
                    Remarks = xmlDocs.Remarks?.Replace("\r\n", "").Trim(),
                    Summary = xmlDocs.Summary?.Replace("\r\n", "").Trim(),
                    Type = field.FieldType,
                };
                Fields.Add(key, documentedField);
            }
            // Link the property to the type
            documentedType.Fields.Add(documentedField.Key, documentedField);
        }
    }

    /// <summary>
    /// Adds events for the specified type.
    /// </summary>
    /// <param name="type">The type to examine.</param>
    public void AddEventsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var events = type.GetEvents().ToList();
        // Add protected methods
        events.AddRange(type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove duplicates
        events = events.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var eventItem in events)
        {
            var category = eventItem.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = eventItem.GetCustomAttribute<ParameterAttribute>();
            var key = $"{eventItem.DeclaringType.FullName}.{eventItem.Name}";

            // Has this property been documented before?
            if (!Events.TryGetValue(key, out var documentedEvent))
            {
                // No.
                documentedEvent = new DocumentedEvent()
                {
                    Category = category?.Name,
                    DeclaringType = eventItem.DeclaringType,
                    Key = key,
                    Name = eventItem.Name,
                    Order = category?.Order,
                    Type = eventItem.EventHandlerType,
                };
                Events.Add(key, documentedEvent);
            }
            // Link the property to the type
            documentedType.Events.Add(documentedEvent.Name, documentedEvent);
        }
    }

    /// <summary>
    /// Looks for properties with an associated "____Changed" event.
    /// </summary>
    /// <param name="type">The documented type to search.</param>
    public static void FindBindableProperties(DocumentedType type)
    {
        // Look for "[Property]Changed" event callbacks
        var changedEvents = type.Events.Where(eventItem => eventItem.Value.Name.EndsWith("Changed", StringComparison.OrdinalIgnoreCase));
        foreach (var eventItem in changedEvents)
        {
            // Look for a property for this event callback
            var property = type.Properties.SingleOrDefault(property => property.Value.Name.Equals(eventItem.Value.Name.Replace("Changed", "", StringComparison.OrdinalIgnoreCase)));
            if (property.Value != null)
            {
                property.Value.ChangeEvent = eventItem.Value;
                eventItem.Value.Property = property.Value;
            }
        }
    }

    /// <summary>
    /// Calculates the types in which all members are declared.
    /// </summary>
    public void FindDeclaringTypes()
    {
        foreach (var property in Properties)
        {
            if (Types.TryGetValue(GetTypeFullName(property.Value.DeclaringType), out var documentedType))
            {
                property.Value.DeclaringDocumentedType = documentedType;
            }
        }
        foreach (var field in Fields)
        {
            if (Types.TryGetValue(GetTypeFullName(field.Value.DeclaringType), out var documentedType))
            {
                field.Value.DeclaringDocumentedType = documentedType;
            }
        }
        foreach (var method in Methods)
        {
            if (Types.TryGetValue(GetTypeFullName(method.Value.DeclaringType), out var documentedType))
            {
                method.Value.DeclaringDocumentedType = documentedType;
            }
        }
        foreach (var eventItem in Events)
        {
            if (Types.TryGetValue(GetTypeFullName(eventItem.Value.DeclaringType), out var documentedType))
            {
                eventItem.Value.DeclaringDocumentedType = documentedType;
            }
        }
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetTypeFullName(Type type)
    {
        // Is a full name already given?
        if (type.FullName != null)
        {
            return $"{type.FullName}";
        }
        // Is there a type by name?
        else if (PublicTypes.TryGetValue(type.Name, out var publicType))
        {
            return $"{publicType.FullName}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public string GetPropertyFullName(PropertyInfo property)
    {
        // Is a full name already given?
        if (property.DeclaringType.FullName != null)
        {
            return $"{property.DeclaringType.FullName}.{property.Name}";
        }
        // Is there a type by name?
        else if (PublicTypes.TryGetValue(property.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{property.Name}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the full name of the field's declaring type.
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public string GetFieldFullName(FieldInfo field)
    {
        // Is a full name already given?
        if (field.DeclaringType.FullName != null)
        {
            return $"{field.DeclaringType.FullName}.{field.Name}";
        }
        // Is there a type by name?
        else if (PublicTypes.TryGetValue(field.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{field.Name}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public string GetMethodFullName(MethodInfo method)
    {
        // Is a full name already given?
        if (method.DeclaringType.FullName != null)
        {
            return $"{method.DeclaringType.FullName}.{method.Name}";
        }
        // Is there a type by name?
        else if (PublicTypes.TryGetValue(method.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{method.Name}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Adds methods the specified documented type.
    /// </summary>
    /// <param name="type">The type to find methods for.</param>
    /// <param name="documentedType">The documentation for the type.</param>
    public void AddMethodsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public methods
        var methods = type.GetMethods().ToList();
        // Add protected methods
        methods.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove internal methods
        methods.RemoveAll(method => method.IsPrivate // Remove private methods
            || IsExcluded(method)                    // Remove some internal methods
            || method.Name.StartsWith("add_")        // Remove event subscribers
            || method.Name.StartsWith("remove_")     // Remove event unsubscribers 
            || method.Name.StartsWith("get_")        // Remove property getters
            || method.Name.StartsWith("set_")        // Remove property setters
            || method.Name.StartsWith("Microsoft")   // Remove object methods
            || method.Name.StartsWith("System"));
        // Remove duplicates
        methods = methods.DistinctBy(method => method.Name).ToList();
        // Look for methods and add related types
        foreach (var method in methods)
        {
            // Get the key for this method
            var key = GetMethodFullName(method);

            // Has this been documented before?
            if (!Methods.TryGetValue(key, out var documentedMethod))
            {
                // No.  Get the XML documentation
                var xmlDocs = _xmlDocs.GetMethodComments(method);

                // Record this property          
                documentedMethod = new DocumentedMethod()
                {
                    DeclaringType = method.DeclaringType,
                    IsProtected = method.IsFamily,
                    Key = key,
                    Name = method.Name,
                    Returns = xmlDocs.Returns?.Replace("\r\n", "").Trim(),
                    Remarks = xmlDocs.Remarks?.Replace("\r\n", "").Trim(),
                    Summary = xmlDocs.Summary?.Replace("\r\n", "").Trim(),
                    Type = method.ReturnType,
                };
                // Reach out and document types mentioned in these methods
                foreach (var parameter in method.GetParameters())
                {
                    var (name, text) = xmlDocs.Parameters.SingleOrDefault(docParameter => docParameter.Name == parameter.Name);
                    var documentedParameter = new DocumentedParameter()
                    {
                        Name = parameter.Name,
                        Type = parameter.ParameterType,
                        Summary = text,
                    };
                    documentedMethod.Parameters.Add(documentedParameter);
                }
                // Add to the list
                Methods.Add(key, documentedMethod);
            }
            // Add the method to the type
            documentedType.Methods.Add(documentedMethod.Key, documentedMethod);
        }
    }

    /// <summary>
    /// Gets the name of a method without its parameters.
    /// </summary>
    /// <param name="xmlMethodName"></param>
    /// <returns></returns>
    public static string GetMethodFullName(string xmlMethodName)
    {
        // Are there parenthesis?
        var parenthesis = xmlMethodName.IndexOf('(');
        if (parenthesis == -1)
        {
            return xmlMethodName;
        }
        else
        {
            return xmlMethodName.Substring(0, parenthesis);
        }
    }

    /// <summary>
    /// Gets the name of a method from the full name.
    /// </summary>
    /// <param name="xmlMethodName"></param>
    /// <returns></returns>
    public static string GetMethodName(string xmlMethodName)
    {
        return xmlMethodName.Substring(xmlMethodName.LastIndexOf('.') + 1);
    }

    /// <summary>
    /// Serializes all documentation to the MudBlazor.Docs "Generated" folder.
    /// </summary>
    public void ExportApiDocumentation()
    {
        // Sort everything by category
        using var writer = new ApiDocumentationWriter(Paths.ApiDocumentationFilePath);
        writer.WriteHeader();
        writer.WriteClassStart();
        writer.WriteConstructorStart();
        writer.WriteProperties(Properties);
        writer.WriteMethods(Methods);
        writer.WriteFields(Fields);
        writer.WriteEvents(Events);
        writer.WriteTypes(Types);
        writer.LinkDocumentedTypes(Properties);
        writer.LinkDocumentedTypes(Methods);
        writer.LinkDocumentedTypes(Fields);
        writer.LinkDocumentedTypes(Events);
        writer.WriteConstructorEnd();
        writer.WriteClassEnd();
    }

    /// <summary>
    /// Calculates how thoroughly types are documented.
    /// </summary>
    public void CalculateDocumentationCoverage()
    {
        // Calculate how many items have good documentation
        var summarizedTypes = Types.Count(type => !string.IsNullOrEmpty(type.Value.Summary));
        var summarizedProperties = Properties.Count(property => !string.IsNullOrEmpty(property.Value.Summary));
        var summarizedMethods = Methods.Count(method => !string.IsNullOrEmpty(method.Value.Summary));
        var summarizedFields = Fields.Count(field => !string.IsNullOrEmpty(field.Value.Summary));
        var summarizedEvents = Events.Count(eventItem => !string.IsNullOrEmpty(eventItem.Value.Summary));
        // Calculate the coverage metrics for documentation
        var typeCoverage = summarizedTypes / (double)Types.Count;
        var propertyCoverage = summarizedProperties / (double)Properties.Count;
        var methodCoverage = summarizedMethods / (double)Methods.Count;
        var fieldCoverage = summarizedFields / (double)Fields.Count;
        var eventCoverage = summarizedEvents / (double)Events.Count;

        Console.WriteLine("XML Documentation Coverage for MudBlazor:");
        Console.WriteLine();
        Console.WriteLine($"Types:      {summarizedTypes} of {Types.Count} ({typeCoverage:P0}) public types");
        Console.WriteLine($"Properties: {summarizedProperties} of {Properties.Count} ({propertyCoverage:P0}) properties");
        Console.WriteLine($"Methods:    {summarizedMethods} of {Methods.Count} ({methodCoverage:P0}) methods");
        Console.WriteLine($"Fields:     {summarizedFields} of {Fields.Count} ({fieldCoverage:P0}) fields/enums");
        Console.WriteLine($"Events:     {summarizedEvents} of {Events.Count} ({eventCoverage:P0}) events/EventCallback");
        Console.WriteLine();
    }

    /// <summary>
    /// Finds <see cref="MudGlobal"/> settings related to all types.
    /// </summary>
    public void AddGlobalsToDocument()
    {
        // Find all of the "MudGlobal" properties
        var globalProperties = Properties.Where(property => property.Value.Key.StartsWith("MudBlazor.MudGlobal")).ToList();
        foreach (var globalProperty in globalProperties)
        {
            // TODO: Make a more explicit way of doing this without string parsing, like an attribute
            // or making each global a static property in the component it affects.

            // Calculate the class this property links to
            var relatedTypeName = "MudBlazor.Mud" + GlobalComponentNameRegEx().Match(globalProperty.Key).Groups[1].Value;

            // Look up the related type
            if (Types.TryGetValue(relatedTypeName, out var type))
            {
                type.GlobalSettings.Add(globalProperty.Key, globalProperty.Value);
            }
        }
    }

    /// <summary>
    /// The regular expression used to extract XML documentation summaries.
    /// </summary>
    [GeneratedRegex(@"MudBlazor\.MudGlobal\+([ \S]*)Defaults\.")]
    private static partial Regex GlobalComponentNameRegEx();
}
