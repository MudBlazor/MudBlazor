// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
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
public class ApiDocumentationBuilder
{
    /// <summary>
    /// The reader for XML documentation.
    /// </summary>
    private readonly Lazy<DocXmlReader> _xmlDocs;

    /// <summary>
    /// The assembly to document.
    /// </summary>
    public List<Assembly> Assemblies { get; } = [typeof(_Imports).Assembly];

    /// <summary>
    /// The types in the assembly.
    /// </summary>
    public SortedDictionary<string, Type> PublicTypes { get; } = [];

    /// <summary>
    /// The generated documentation for events.
    /// </summary>
    public SortedDictionary<string, DocumentedEvent> Events { get; } = [];

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public SortedDictionary<string, DocumentedField> Fields { get; } = [];

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public SortedDictionary<string, DocumentedType> Types { get; } = [];

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public SortedDictionary<string, DocumentedProperty> Properties { get; } = [];

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public SortedDictionary<string, DocumentedMethod> Methods { get; } = [];

    /// <summary>
    /// Any types to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedTypes { get; } =
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
    public static List<string> ExcludedMembers { get; } =
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
        "DispatchExceptionAsync",
        "SetParametersAsync",
        "CreateRegisterScope",
        // Dispose methods
        "Dispose",
        "DisposeAsync",
        "Finalize",
    ];

    public ApiDocumentationBuilder()
    {
        _xmlDocs = new Lazy<DocXmlReader>(() => new DocXmlReader(Assemblies));
    }

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
        if (type.FullName != null && ExcludedTypes.Any(type.FullName.StartsWith))
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
        AddTypesToDocument();
        ResolveSeeAlsoLinks();
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
                    // ... which aren't internal

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
    public void AddTypeToDocument(Type type)
    {
        // Is the type already documented?
        if (type.FullName is not null && !Types.TryGetValue(type.FullName, out var documentedType))
        {
            // Look up the XML documentation
            var typeXmlDocs = _xmlDocs.Value.GetTypeComments(type);

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
                Links = typeXmlDocs.SeeAlso.Select(seealso => new DocumentedLink()
                {
                    Cref = seealso.Cref,
                    Href = seealso.Href,
                    Text = seealso.Text,
                }).ToList()
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
    }

    /// <summary>
    /// Adds public properties for the specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="documentedType">The document type.</param>
    public void AddPropertiesToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var properties = type.GetProperties().ToList();
        // Add protected methods
        properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove properties we don't want on the site
        properties.RemoveAll(property => property.GetMethod is not null
                                         && (property.GetMethod.IsPrivate // Remove private properties
                                             || property.GetMethod.IsAssembly // Remove internal properties
                                             || property.GetMethod.IsFamilyOrAssembly // Remove overridden internal properties
                                             || IsExcluded(property)));                   // Remove properties from the manually maintained list
        // Remove duplicates
        properties = properties.DistinctBy(property => property.Name).ToList();

        // Go through each property
        foreach (var property in properties)
        {
            var category = property.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = property.GetCustomAttribute<ParameterAttribute>();
            var key = GetPropertyFullName(property);

            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            // Is this an event?
            if (property.PropertyType.Name.StartsWith("EventCallback"))
            {
                // Has this event been documented before?
                if (!Events.TryGetValue(key, out var documentedEvent))
                {
                    // No.  Get the XML documentation
                    var xmlDocs = _xmlDocs.Value.GetMemberComments(property);

                    // Record this event
                    documentedEvent = new DocumentedEvent
                    {
                        Category = category?.Name,
                        DeclaringType = property.DeclaringType,
                        IsProtected = property.GetMethod?.IsFamily ?? false,
                        IsParameter = blazorParameter != null,
                        Key = key,
                        Name = property.Name,
                        Order = category?.Order ?? int.MaxValue,
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
                    var xmlDocs = _xmlDocs.Value.GetMemberComments(property);

                    // Record this property                
                    documentedProperty = new DocumentedProperty()
                    {
                        Category = category?.Name,
                        DeclaringType = property.DeclaringType,
                        IsProtected = property.GetMethod?.IsFamily ?? false,
                        IsParameter = blazorParameter is not null,
                        Key = key,
                        Name = property.Name,
                        Order = category?.Order ?? int.MaxValue,
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
    /// <param name="documentedType">The document type.</param>
    public void AddFieldsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var fields = type.GetFields().ToList();
        // Add protected methods
        fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove fields we don't want documented
        fields.RemoveAll(field =>
            field.Name.Contains("k__BackingField")    // Remove backing fields
            || field.Name == "value__"
            || field.Name.StartsWith('_')
            || field.IsPrivate                        // Remove private fields            
            || field.IsAssembly                       // Remove internal fields
            || field.IsFamilyOrAssembly               // Remove overridden internal fields
            || IsExcluded(field));                    // Remove fields the team doesn't want shown
        // Remove duplicates
        fields = fields.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var field in fields)
        {
            var category = field.GetCustomAttribute<CategoryAttribute>();
            var key = GetFieldFullName(field);

            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            // Has this property been documented before?
            if (!Fields.TryGetValue(key, out var documentedField))
            {
                // No.  Get the XML documentation
                var xmlDocs = _xmlDocs.Value.GetMemberComments(field);

                // Record this property
                documentedField = new DocumentedField
                {
                    Category = category?.Name,
                    DeclaringType = field.DeclaringType,
                    IsProtected = field.IsFamily,
                    Key = key,
                    Name = field.Name,
                    Order = category?.Order ?? int.MaxValue,
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
    /// <param name="documentedType">The document type.</param>
    public void AddEventsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var events = type.GetEvents().ToList();
        // Add protected methods
        events.AddRange(type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove unwanted events
        events.RemoveAll(eventItem =>
            eventItem.AddMethod is not null
            && (eventItem.AddMethod.IsPrivate // Remove private events
                || eventItem.AddMethod.IsAssembly // Remove internal events
                || eventItem.AddMethod.IsFamilyOrAssembly)); // Remove overridden internal fields
        // Remove duplicates
        events = events.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var eventItem in events)
        {
            var category = eventItem.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = eventItem.GetCustomAttribute<ParameterAttribute>();
            var key = $"{eventItem.DeclaringType?.FullName}.{eventItem.Name}";

            // Has this property been documented before?
            if (!Events.TryGetValue(key, out var documentedEvent))
            {
                // No.
                documentedEvent = new DocumentedEvent
                {
                    Category = category?.Name,
                    DeclaringType = eventItem.DeclaringType,
                    Key = key,
                    Name = eventItem.Name,
                    Order = category?.Order ?? int.MaxValue,
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
        AssignDeclaringDocumentedType(Properties);
        AssignDeclaringDocumentedType(Fields);
        AssignDeclaringDocumentedType(Methods);
        AssignDeclaringDocumentedType(Events);
        return;

        void AssignDeclaringDocumentedType<T>(IEnumerable<KeyValuePair<string, T>> items) where T : DocumentedMember
        {
            foreach (var item in items)
            {
                var fullName = GetTypeFullName(item.Value.DeclaringType);
                if (fullName is not null && Types.TryGetValue(fullName, out var documentedType))
                {
                    item.Value.DeclaringDocumentedType = documentedType;
                }
            }
        }
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string? GetTypeFullName(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        // Is a full name already given?
        if (type.FullName is not null)
        {
            return $"{type.FullName}";
        }
        // Is there a type by name?

        if (PublicTypes.TryGetValue(type.Name, out var publicType))
        {
            return $"{publicType.FullName}";
        }

        return null;
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="property"></param>
    public string? GetPropertyFullName(PropertyInfo? property)
    {
        if (property?.DeclaringType is null)
        {
            return null;
        }

        // Is a full name already given?
        if (property.DeclaringType.FullName is not null)
        {
            return $"{property.DeclaringType.FullName}.{property.Name}";
        }

        // Is there a type by name?
        if (PublicTypes.TryGetValue(property.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{property.Name}";
        }

        return null;
    }

    /// <summary>
    /// Gets the full name of the field's declaring type.
    /// </summary>
    /// <param name="field"></param>
    public string? GetFieldFullName(FieldInfo? field)
    {
        if (field?.DeclaringType is null)
        {
            return null;
        }

        // Is a full name already given?
        if (field.DeclaringType.FullName is not null)
        {
            return $"{field.DeclaringType.FullName}.{field.Name}";
        }

        // Is there a type by name?
        if (PublicTypes.TryGetValue(field.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{field.Name}";
        }

        return null;
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="method"></param>
    public string? GetMethodFullName(MethodInfo? method)
    {
        if (method?.DeclaringType is null)
        {
            return null;
        }

        // Is a full name already given?
        if (method.DeclaringType.FullName != null)
        {
            return $"{method.DeclaringType.FullName}.{method.Name}";
        }

        // Is there a type by name?
        if (PublicTypes.TryGetValue(method.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{method.Name}";
        }

        return null;
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
        // Remove methods we don't want on the site
        methods.RemoveAll(method => method.IsPrivate // Remove private methods
            || method.IsAssembly                     // Remove internal methods
            || method.IsFamilyOrAssembly             // Remove overridden internal methods
            || IsExcluded(method)                    // Remove some internal methods
            || method.Name.StartsWith("add_")        // Remove event subscribers
            || method.Name.StartsWith("remove_")     // Remove event unsubscribers 
            || method.Name.StartsWith("get_")        // Remove property getters
            || method.Name.StartsWith("set_")        // Remove property setters
            || method.Name.StartsWith("Microsoft")   // Remove object methods
            || method.Name.StartsWith("System"));    // Remove built-in methods
        // Remove duplicates
        methods = methods.DistinctBy(method => method.Name).ToList();
        // Look for methods and add related types
        foreach (var method in methods)
        {
            // Get the key for this method
            var key = GetMethodFullName(method);
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }
            // Has this been documented before?
            if (!Methods.TryGetValue(key, out var documentedMethod))
            {
                // No.  Get the XML documentation
                var xmlDocs = _xmlDocs.Value.GetMethodComments(method);

                // Record this property          
                documentedMethod = new DocumentedMethod
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
                    var (_, text) = xmlDocs.Parameters.SingleOrDefault(docParameter => docParameter.Name == parameter.Name);
                    var documentedParameter = new DocumentedParameter
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
    /// Serializes all documentation to the MudBlazor.Docs "Generated" folder.
    /// </summary>
    public void ExportApiDocumentation()
    {
        // Sort everything by category
        using var writer = new ApiDocumentationWriter();
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
        writer.WriteSeeAlsoLinks(Types);
        writer.WriteConstructorEnd();
        writer.WriteClassEnd();
        var currentCode = string.Empty;
        if (File.Exists(Paths.ApiDocumentationFilePath))
        {
            currentCode = File.ReadAllText(Paths.ApiDocumentationFilePath);
        }
        if (currentCode != writer.ToString())
        {
            File.WriteAllText(Paths.ApiDocumentationFilePath, writer.ToString());
        }
    }

    /// <summary>
    /// Calculates how thoroughly types are documented.
    /// </summary>
    public void CalculateDocumentationCoverage()
    {
        // Calculate how many items have good documentation
        var wellDocumentedTypes = Types.Count(type => !string.IsNullOrEmpty(type.Value.Summary));
        var wellDocumentedProperties = Properties.Count(property => !string.IsNullOrEmpty(property.Value.Summary));
        var wellDocumentedMethods = Methods.Count(method => !string.IsNullOrEmpty(method.Value.Summary));
        var wellDocumentedFields = Fields.Count(field => !string.IsNullOrEmpty(field.Value.Summary));
        var wellDocumentedEvents = Events.Count(eventItem => !string.IsNullOrEmpty(eventItem.Value.Summary));
        // Calculate the coverage metrics for documentation
        var typeCoverage = wellDocumentedTypes / (double)Types.Count;
        var propertyCoverage = wellDocumentedProperties / (double)Properties.Count;
        var methodCoverage = wellDocumentedMethods / (double)Methods.Count;
        var fieldCoverage = wellDocumentedFields / (double)Fields.Count;
        var eventCoverage = wellDocumentedEvents / (double)Events.Count;

        Console.WriteLine(@"XML Documentation Coverage for MudBlazor:");
        Console.WriteLine();
        Console.WriteLine(@$"Types:      {wellDocumentedTypes} of {Types.Count} ({typeCoverage:P0}) types");
        Console.WriteLine(@$"Properties: {wellDocumentedProperties} of {Properties.Count} ({propertyCoverage:P0}) properties");
        Console.WriteLine(@$"Methods:    {wellDocumentedMethods} of {Methods.Count} ({methodCoverage:P0}) methods");
        Console.WriteLine(@$"Fields:     {wellDocumentedFields} of {Fields.Count} ({fieldCoverage:P0}) fields");
        Console.WriteLine(@$"Events:     {wellDocumentedEvents} of {Events.Count} ({eventCoverage:P0}) events/EventCallback");
        Console.WriteLine();
    }

    /// <summary>
    /// Finds <see cref="MudGlobal"/> settings related to all types.
    /// </summary>
    public void AddGlobalsToDocument()
    {
        // Find all the "MudGlobal" properties
        var globalProperties = Properties.Where(property => property.Key.StartsWith("MudBlazor.MudGlobal+")).ToList();
        foreach (var globalProperty in globalProperties)
        {
            /* MudGlobal properties thankfully mention the component they are for, by way of a
             * <see cref=""> tag in the summary.  Let's use this to tie a global property with its 
             * component.  Also, let's link this global to any of the component's descendants.
             */

            if (globalProperty.Value.Summary is null)
            {
                continue;
            }

            // Does the summary mention the type?
            var start = globalProperty.Value.Summary.IndexOf("<see cref=\"T:", StringComparison.OrdinalIgnoreCase);
            if (start != -1)
            {
                // Yes.   Move up to the type  (i.e. "MudBlazor.___")
                start += 13;
                var end = start == -1 ? -1 : globalProperty.Value.Summary.IndexOf('\"', start);
                var typeName = globalProperty.Value.Summary.Substring(start, end - start);

                // Does the mentioned type exist?
                if (Types.TryGetValue(typeName, out var documentedType))
                {
                    // Yes.  Link it to this global if it is not already linked
                    if (documentedType.GlobalSettings.All(pair => pair.Value.Name != globalProperty.Value.Name))
                    {
                        documentedType.GlobalSettings.Add(globalProperty.Key, globalProperty.Value);
                    }
                    // Also link descendants of this type
                    foreach (var descendant in Types.Where(type => type.Value.BaseType is not null && type.Value.BaseType.Name == documentedType.Type.Name))
                    {
                        // Link it to this global as well if it is not already linked
                        if (descendant.Value.GlobalSettings.All(pair => pair.Value.Name != globalProperty.Value.Name))
                        {
                            descendant.Value.GlobalSettings.Add(globalProperty.Key, globalProperty.Value);
                        }
                    }
                }
                else
                {
                    // A global is missing the docs necessary to link it to a type.                      
                }
            }
        }
    }

    /// <summary>
    /// Resolves XML references like "T:MudBlazor.MudAlert" into a <see cref="DocumentedType"/> or <see cref="DocumentedMember"/>.
    /// </summary>
    public void ResolveSeeAlsoLinks()
    {
        // Find the types which have see-also links
        foreach (var type in Types.Where(type => type.Value.Links.Count > 0))
        {
            // Go through each link
            foreach (var link in type.Value.Links)
            {
                // Is this a CREF link?  (e.g. "T:MudBlazor.MudAlert")
                if (!string.IsNullOrEmpty(link.Cref))
                {
                    // Split the type and link
                    var values = link.Cref.Split(":");
                    var linkType = values[0];
                    var cref = values[1];
                    switch (linkType)
                    {
                        case "T":
                            if (Types.TryGetValue(cref, out var existingType))
                            {
                                link.Type = existingType;
                            }
                            break;
                        case "P":
                            if (Properties.TryGetValue(cref, out var existingProperty))
                            {
                                link.Property = existingProperty;
                            }
                            break;
                        case "F":
                            if (Fields.TryGetValue(cref, out var existingField))
                            {
                                link.Field = existingField;
                            }
                            break;
                        case "M":
                            if (Methods.TryGetValue(cref, out var existingMethod))
                            {
                                link.Method = existingMethod;
                            }
                            break;
                        case "E":
                            if (Events.TryGetValue(cref, out var existingEvent))
                            {
                                link.Event = existingEvent;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
