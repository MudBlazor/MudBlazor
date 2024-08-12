// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
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
    public Assembly Assembly { get; private set; } = typeof(_Imports).Assembly;

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
    /// The types which have documentation but could not be linked to a reflected type.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected type.
    /// </remarks>
    public List<string> UnresolvedTypes { get; private set; } = [];

    /// <summary>
    /// The properties which have documentation but could not be linked to a reflected property.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected property.
    /// </remarks>
    public List<string> UnresolvedProperties { get; private set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected field.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected field.
    /// </remarks>
    public List<string> UnresolvedFields { get; private set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected method.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected method.
    /// </remarks>
    public List<string> UnresolvedMethods { get; private set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected event.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected event.
    /// </remarks>
    public List<string> UnresolvedEvents { get; private set; } = [];

    /// <summary>
    /// Any types to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedTypes { get; private set; } =
    [
        "MudBlazor._Imports",
        "MudBlazor.CategoryTypes",
        "MudBlazor.CategoryTypes+",
        "MudBlazor.Colors",
        "MudBlazor.Colors+",
        "MudBlazor.Resources.LanguageResource",
        "MudBlazor.Icons",
        "MudBlazor.Icons+",
        "string"
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
    /// Any methods to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedMethods { get; private set; } =
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
        "SetParametersAsync",
        "DispatchExceptionAsync",
        "CreateRegisterScope",
        "DetectIllegalRazorParametersV7",
        "MudBlazor.Interfaces.IMudStateHasChanged.StateHasChanged",
    ];

    /// <summary>
    /// Generates documentation for all types.
    /// </summary>
    public bool Execute()
    {
        AddTypesToDocument();
        AddGlobalsToDocument();
        MergeXmlDocumentation();
        ExportApiDocumentation();
        CalculateDocumentationCoverage();
        return true;
    }

    /// <summary>
    /// Adds an empty documented type for each public type.
    /// </summary>
    public void AddTypesToDocument()
    {
        // Get all MudBlazor public types
        PublicTypes = new(Assembly.GetTypes().Where(type => type.IsPublic).ToDictionary(r => r.Name, v => v));
        foreach (var type in PublicTypes)
        {
            AddTypeToDocument(type.Value);
        }
    }

    /// <summary>
    /// Adds the specified type and any related public types.
    /// </summary>
    /// <param name="type">The type to add.</param>
    public DocumentedType AddTypeToDocument(Type type)
    {
        // Is this type excluded?
        if (IsExcluded(type))
        {
            return null;
        }

        // Is the type already documented?
        if (!Types.TryGetValue(type.FullName, out var documentedType))
        {
            // No.
            documentedType = new DocumentedType()
            {
                BaseType = type.BaseType,
                IsPublic = type.IsPublic,
                IsAbstract = type.IsNestedFamORAssem,
                Key = type.FullName,
                XmlKey = GetXmlKey(type.FullName),
                Name = type.Name,
                Type = type,
            };

            // Add the root-level type
            Types.Add(type.FullName, documentedType);

            // Record properties, methods, fields, and events            
            AddPropertiesToDocument(type, documentedType);
            AddMethodsToDocument(type, documentedType);
            AddFieldsToDocument(type, documentedType);
            AddEventsToDocument(type, documentedType);

            // Also add nested types            
            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public))
            {
                AddTypeToDocument(nestedType);
            }
        }

        return documentedType;
    }

    /// <summary>
    /// Gets the XML member key for the specified type and member.
    /// </summary>
    /// <param name="typeFullName">The <see cref="Type.FullName"/> of the type containing the member.</param>
    /// <param name="memberName">The fully qualified name of the member.</param>
    /// <returns>The member key for looking up documentation.</returns>
    public static string GetXmlKey(string typeFullName, string memberName = null)
    {
        // See: https://learn.microsoft.com/archive/msdn-magazine/2019/october/csharp-accessing-xml-documentation-via-reflection

        // Get the key for the type
        var key = TypeFullNameRegEx().Replace(typeFullName, string.Empty).Replace('+', '.');
        return (memberName != null) ? key + "." + memberName : key;
    }

    /// <summary>
    /// Gets the XML member key for the specified type and method.
    /// </summary>
    /// <param name="typeFullName">The <see cref="Type.FullName"/> of the type containing the member.</param>
    /// <param name="memberName">The fully qualified name of the member.</param>
    /// <returns>The member key for looking up documentation.</returns>
    public static string GetXmlKey(string typeFullNameString, MethodInfo methodInfo)
    {
        if (methodInfo.Name == "GetOrAdd")
        {
            Debugger.Break();
        }

        var typeGenericMap = new Dictionary<string, int>();
        var tempTypeGeneric = 0;
        Array.ForEach(methodInfo.DeclaringType.GetGenericArguments(), x => typeGenericMap[x.Name] = tempTypeGeneric++);
        var methodGenericMap = new Dictionary<string, int>();
        var tempMethodGeneric = 0;
        Array.ForEach(methodInfo.GetGenericArguments(), x => methodGenericMap.Add(x.Name, tempMethodGeneric++));
        var parameterInfos = methodInfo.GetParameters().ToList();

        var key = typeFullNameString + "." + methodInfo.Name;

        if (parameterInfos.Count > 0)
        {
            key += "(";
            for (var index = 0; index < parameterInfos.Count; index++)
            {
                var parameterInfo = parameterInfos[index];
                if (index > 0)
                {
                    key += ",";
                }
                key += parameterInfo.ParameterType.FullName;

                if (parameterInfo.ParameterType.HasElementType)
                {
                    //Debugger.Break();
                    // The type is either an array, pointer, or reference
                    if (parameterInfo.ParameterType.IsArray)
                    {
                        // Append the "[]" array brackets onto the element type
                        key += "[]";
                    }
                    else if (parameterInfo.ParameterType.IsPointer)
                    {
                        // Append the "*" pointer symbol to the element type
                    }
                    else if (parameterInfo.ParameterType.IsByRef)
                    {
                        // Append the "@" symbol to the element type
                    }
                }
                else if (parameterInfo.ParameterType.IsGenericParameter)
                {
                    // Look up the index of the generic from the
                    // dictionaries in Figure 5, appending "`" if
                    // the parameter is from a type or "``" if the
                    // parameter is from a method
                    //Debugger.Break();
                }
                else
                {
                    // Nothing fancy, just convert the type to a string
                }
            }
            key += ")";
        }
        return key;
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
        // Remove duplicates
        properties = properties.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var property in properties)
        {
            var category = property.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = property.GetCustomAttribute<ParameterAttribute>();
            var key = GetPropertyFullName(property);

            // Has this property been documented before?
            if (!Properties.TryGetValue(key, out var documentedProperty))
            {
                // No.
                documentedProperty = new DocumentedProperty()
                {
                    Category = category?.Name,
                    DeclaringType = property.DeclaringType,
                    DeclaringTypeFullName = GetTypeFullName(property.DeclaringType),
                    IsProtected = property.GetMethod.IsFamily,
                    IsParameter = blazorParameter != null,
                    Key = key,
                    Name = property.Name,
                    Order = category?.Order,
                    Type = property.PropertyType,
                    XmlKey = GetXmlKey(GetTypeFullName(property.DeclaringType), property.Name),
                };
                Properties.Add(key, documentedProperty);
            }
            // Link the property to the type
            documentedType.Properties.Add(documentedProperty.Key, documentedProperty);
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
        fields.RemoveAll(field => field.Name.Contains("k__BackingField") || field.Name == "value__" || field.Name.StartsWith('_'));
        // Remove duplicates
        fields = fields.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var field in fields)
        {
            var category = field.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = field.GetCustomAttribute<ParameterAttribute>();
            var key = GetFieldFullName(field);

            // Has this property been documented before?
            if (!Fields.TryGetValue(key, out var documentedField))
            {
                // No.
                documentedField = new DocumentedField()
                {
                    Category = category?.Name,
                    DeclaringType = field.DeclaringType,
                    IsProtected = field.IsFamily,
                    Key = key,
                    Name = field.Name,
                    Order = category?.Order,
                    Type = field.FieldType,
                    XmlKey = GetXmlKey(GetTypeFullName(field.DeclaringType), field.Name),
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
                    XmlKey = GetXmlKey(GetTypeFullName(eventItem.DeclaringType), eventItem.Name),
                };
                Events.Add(key, documentedEvent);
            }
            // Link the property to the type
            documentedType.Events.Add(documentedEvent.Name, documentedEvent);
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
        methods = methods
            // Remove duplicates
            .DistinctBy(method => method.Name)
            .Where(method =>
                // Exclude getter and setter methods
                !method.Name.StartsWith("get_")
                && !method.Name.StartsWith("set_")
                // Exclude inherited .NET methods
                && !method.Name.StartsWith("Microsoft")
                && !method.Name.StartsWith("System")
            )
            .OrderBy(method => method.Name)
            .ToList();
        // Look for methods and add related types
        foreach (var method in methods)
        {
            //if (method.Name.Contains("ToDescriptionString"))
            //{
            //    Debugger.Break();
            //}

            // Get the key for this method
            var key = GetMethodFullName(method);

            // Has this been documented before?
            if (!Methods.TryGetValue(key, out var documentedMethod))
            {
                // No.
                documentedMethod = new DocumentedMethod()
                {
                    DeclaringType = method.DeclaringType,
                    IsProtected = method.IsFamily,
                    Key = key,
                    Name = method.Name,
                    Type = method.ReturnType,
                    XmlKey = GetXmlKey(GetTypeFullName(method.DeclaringType), method)
                };
                // Reach out and document types mentioned in these methods
                foreach (var parameter in method.GetParameters())
                {
                    var documentedParameter = new DocumentedParameter()
                    {
                        Name = parameter.Name,
                        Type = parameter.ParameterType,
                        TypeFullName = parameter.ParameterType.FullName,
                        TypeName = parameter.ParameterType.Name
                    };
                    documentedMethod.Parameters.Add(parameter.Name, documentedParameter);
                }
                // Add to the list
                Methods.Add(key, documentedMethod);
            }
            // Add the method to the type
            documentedType.Methods.Add(documentedMethod.Key, documentedMethod);
        }
    }

    /// <summary>
    /// Merges XML documentation with existing documentation types.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    public void MergeXmlDocumentation()
    {
        // Open the XML documentation file
        var path = Assembly.Location.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase);
        using var reader = new XmlTextReader(path);
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.DtdProcessing = DtdProcessing.Ignore;
        // Move to the first member
        reader.ReadToFollowing("member");
        // Read each "<member name=...>" element
        while (!reader.EOF)
        {
            var memberTypeAndName = reader.GetAttribute("name").Split(":");
            var content = reader.ReadInnerXml();
            switch (memberTypeAndName[0])
            {
                case "T": // Type
                    DocumentType(memberTypeAndName[1], content);
                    break;
                case "P": // Property
                    DocumentProperty(memberTypeAndName[1], content);
                    break;
                case "M": // Method
                    DocumentMethod(memberTypeAndName[1], content);
                    break;
                case "F": // Field (or Enum)
                    DocumentField(memberTypeAndName[1], content);
                    break;
                case "E": // Event
                    DocumentEvent(memberTypeAndName[1], content);
                    break;
            }
            // Are we at the end of the document?
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "members")
            {
                break;
            }
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified type.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentType(string memberFullName, string xmlContent)
    {
        var type = Types.FirstOrDefault(type => type.Value.XmlKey == memberFullName);
        if (type.Value != null)
        {
            type.Value.Remarks = GetRemarks(xmlContent);
            type.Value.Summary = GetSummary(xmlContent);
        }
        else
        {
            UnresolvedTypes.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified property.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentProperty(string memberFullName, string xmlContent)
    {
        var property = Properties.FirstOrDefault(type => type.Value.XmlKey == memberFullName);
        if (property.Value != null)
        {
            property.Value.Summary = GetSummary(xmlContent);
            property.Value.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            UnresolvedProperties.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentField(string memberFullName, string xmlContent)
    {
        var field = Fields.FirstOrDefault(type => type.Value.XmlKey == memberFullName);
        if (field.Value != null)
        {
            field.Value.Summary = GetSummary(xmlContent);
            field.Value.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            UnresolvedFields.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentMethod(string memberFullName, string xmlContent)
    {
        var method = Methods.FirstOrDefault(method => method.Value.XmlKey == memberFullName);
        if (method.Value != null)
        {
            method.Value.Summary = GetSummary(xmlContent);
            method.Value.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            // No.  It should be documented
            UnresolvedMethods.Add(memberFullName);
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
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentEvent(string memberFullName, string xmlContent)
    {
        if (Events.TryGetValue(memberFullName, out var documentedType))
        {
            documentedType.Summary = GetSummary(xmlContent);
            documentedType.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            UnresolvedEvents.Add(memberFullName);
        }
    }

    /// <summary>
    /// Gets the content of the "summary" element as HTML.
    /// </summary>
    /// <param name="xml">The member XML to search.</param>
    /// <returns>The HTML content of the member.</returns>
    public static string GetSummary(string xml)
    {
        var summary = SummaryRegEx().Match(xml).Groups.GetValueOrDefault("1");
        return summary?.Value;
    }

    /// <summary>
    /// Gets the content of the "remarks" element as HTML.
    /// </summary>
    /// <param name="xml">The member XML to search.</param>
    /// <returns>The HTML content of the member.</returns>
    public static string GetRemarks(string xml)
    {
        var remarks = RemarksRegEx().Match(xml).Groups.GetValueOrDefault("1");
        return remarks?.Value;
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
        Console.WriteLine($"Types:      {summarizedTypes} of {Types.Count} ({typeCoverage:P0}) other types");
        Console.WriteLine($"Properties: {summarizedProperties} of {Properties.Count} ({propertyCoverage:P0}) properties");
        Console.WriteLine($"Methods:    {summarizedMethods} of {Methods.Count} ({methodCoverage:P0}) methods");
        Console.WriteLine($"Fields:     {summarizedFields} of {Fields.Count} ({fieldCoverage:P0}) fields/enums");
        Console.WriteLine($"Events:     {summarizedEvents} of {Events.Count} ({eventCoverage:P0}) events");
        Console.WriteLine();

        if (UnresolvedTypes.Count > 0)
        {
            Console.WriteLine($"API Builder: WARNING: {UnresolvedTypes.Count} types have XML documentation which couldn't be matched to a type.");
        }
        if (UnresolvedProperties.Count > 0)
        {
            Console.WriteLine($"API Builder: WARNING: {UnresolvedProperties.Count} properties have XML documentation which couldn't be matched to a property.");
        }
        if (UnresolvedMethods.Count > 0)
        {
            Console.WriteLine($"API Builder: WARNING: {UnresolvedMethods.Count} methods have XML documentation which couldn't be matched to a method.");
        }
        if (UnresolvedEvents.Count > 0)
        {
            Console.WriteLine($"API Builder: WARNING: {UnresolvedEvents.Count} events have XML documentation which couldn't be matched to an event.");
        }
        if (UnresolvedFields.Count > 0)
        {
            Console.WriteLine($"API Builder: WARNING: {UnresolvedFields.Count} fields have XML documentation which couldn't be matched to a field.");
        }
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

    /// <summary>
    /// The regular expression used to extract XML documentation summaries.
    /// </summary>
    [GeneratedRegex(@"<summary>\s*([ \S]*)\s*<\/summary>")]
    private static partial Regex SummaryRegEx();

    /// <summary>
    /// The regular expression used to extract XML documentation remarks.
    /// </summary>
    [GeneratedRegex(@"<remarks>\s*([ \S]*)\s*<\/remarks>")]
    private static partial Regex RemarksRegEx();

    /// <summary>
    /// The regular expression used to extract XML documentation return values.
    /// </summary>
    [GeneratedRegex(@"<returns>\s*([ \S]*)\s*<\/returns>")]
    private static partial Regex ReturnsRegEx();

    /// <summary>
    /// The regular expression used to calculate the XML member key.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\[.*\]")]
    private static partial Regex TypeFullNameRegEx();
}
