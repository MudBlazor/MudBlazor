﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a writer for generated API documentation.
/// </summary>
public sealed class ApiDocumentationWriter(string filePath) : StreamWriter(File.Create(filePath))
{
    /// <summary>
    /// Creates a new instance with types and the default output path.
    /// </summary>
    public ApiDocumentationWriter() : this(Paths.ApiDocumentationFilePath)
    {
    }

    /// <summary>
    /// Any types to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedTypes { get; private set; } =
    [
        "Enum"
    ];

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
        "MudBlazor.Interfaces.IMudStateHasChanged.StateHasChanged"
    ];

    /// <summary>
    /// The current indentation level.
    /// </summary>
    public int IndentLevel { get; set; }

    /// <summary>
    /// Writes the copyright boilerplate.
    /// </summary>
    public void WriteHeader()
    {
        WriteLine($"// Copyright (c) MudBlazor {DateTime.Now.Year}");
        WriteLine("// MudBlazor licenses this file to you under the MIT license.");
        WriteLine("// See the LICENSE file in the project root for more information.");
        WriteLine();
        WriteLine("//-----------------------------------------------------------------------");
        WriteLine("// Generated by MudBlazor.Docs.Compiler.ApiDocumentationWriter");
        WriteLine("// Any changes to this file will be overwritten on build");
        WriteLine("// <auto-generated />");
        WriteLine("//-----------------------------------------------------------------------");
        WriteLine();
        WriteLine("using System.Collections.Frozen;");
        WriteLine("using System.Collections.Generic;");
        WriteLine("using System.CodeDom.Compiler;");
        WriteLine();
        WriteLine("namespace MudBlazor.Docs.Models;");
        WriteLine();
    }

    /// <summary>
    /// Writes the start of the ApiDocumentation partial class.
    /// </summary>
    public void WriteClassStart()
    {
        WriteLine("/// <summary>");
        WriteLine("/// Represents all of the XML documentation for public-facing classes.");
        WriteLine("/// </summary>");
        WriteLine($"[GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"{typeof(ApiDocumentationWriter).Assembly.GetName().Version}\")]");
        WriteLine("public static partial class ApiDocumentation");
        WriteLine("{");
        Indent();
    }

    /// <summary>
    /// Writes the end of the ApiDocumentation partial class.
    /// </summary>
    public void WriteClassEnd()
    {
        Outdent();
        WriteLine("}");
    }

    /// <summary>
    /// Writes a series of tabs to indent the line.
    /// </summary>
    public void WriteIndent()
    {
        for (var index = 0; index < IndentLevel; index++)
        {
            Write("\t");
        }
    }

    /// <summary>
    /// Writes the start of the ApiDocumentation constructor.
    /// </summary>
    public void WriteConstructorStart(int typeCount)
    {
        WriteLineIndented("static ApiDocumentation()");
        WriteLineIndented("{");
        Indent();
        WriteLineIndented("// Build all of the documented types");
        WriteLineIndented($"var types = new Dictionary<string, DocumentedType>({typeCount});");
    }

    /// <summary>
    /// Writes text with the current indentation level.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteIndented(string text)
    {
        WriteIndent();
        Write(text);
    }

    /// <summary>
    /// Writes text with the current indentation level, and ends the line.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteLineIndented(string text)
    {
        WriteIndented(text);
        WriteLine();
    }

    /// <summary>
    /// Writes the end of the ApiDocumentation constructor.
    /// </summary>
    public void WriteConstructorEnd()
    {
        WriteLine();
        WriteLineIndented($"Types = types.ToFrozenDictionary();");
        Outdent();
        WriteLine("}");
    }

    /// <summary>
    /// Writes the end of the ApiDocumentation class.
    /// </summary>
    public void WriteApiDocumentationClassEnd()
    {
        Outdent();
        WriteLine("}");
    }

    /// <summary>
    /// Increases the indentation level.
    /// </summary>
    public void Indent()
    {
        IndentLevel++;
    }

    /// <summary>
    /// Decreases the indentation level.
    /// </summary>
    public void Outdent()
    {
        IndentLevel--;
    }

    /// <summary>
    /// Formats a string for use in C# code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string Escape(string code) => code?.Replace("\"", "\\\"");

    /// <summary>
    /// Serializes an XML summary for a category.
    /// </summary>
    /// <param name="category"></param>
    public void WriteCategory(string category)
    {
        if (!string.IsNullOrEmpty(category))
        {
            Write($"Category = \"{category}\", ");
        }
    }

    /// <summary>
    /// Serializes an XML summary for a member.
    /// </summary>
    /// <param name="summary"></param>
    public void WriteSummary(string summary)
    {
        if (!string.IsNullOrEmpty(summary))
        {
            Write($"Summary = \"{Escape(summary)}\", ");
        }
    }

    /// <summary>
    /// Serializes an XML summary for a member.
    /// </summary>
    /// <param name="remarks"></param>
    public void WriteSummaryIndented(string remarks)
    {
        if (!string.IsNullOrEmpty(remarks))
        {
            WriteLineIndented($"Summary = \"{Escape(remarks)}\", ");
        }
    }

    /// <summary>
    /// Serializes an XML remarks for a member.
    /// </summary>
    /// <param name="remarks"></param>
    public void WriteRemarks(string remarks)
    {
        if (!string.IsNullOrEmpty(remarks))
        {
            Write($"Remarks = \"{Escape(remarks)}\", ");
        }
    }

    /// <summary>
    /// Serializes an XML remarks for a member.
    /// </summary>
    /// <param name="remarks"></param>
    public void WriteRemarksIndented(string remarks)
    {
        if (!string.IsNullOrEmpty(remarks))
        {
            WriteIndented($"Remarks = \"{Escape(remarks)}\", ");
        }
    }

    /// <summary>
    /// Serializes all of the specified types.
    /// </summary>
    /// <param name="types"></param>
    public void WriteTypes(IDictionary<string, DocumentedType> types)
    {
        foreach (var type in types)
        {
            WriteType(type.Value);
        }
    }

    /// <summary>
    /// Serializes the specified type.
    /// </summary>
    /// <param name="type">The type to serialize.</param>
    public void WriteType(DocumentedType type)
    {
        WriteIndented($"types.Add(\"{type.Name}\", new()");
        WriteLine(" {");
        Indent();
        WriteLineIndented($"Name = \"{type.Name}\", ");
        WriteBaseTypeIndented(type.BaseType);
        WriteIsComponentIndented(type.Type.IsSubclassOf(typeof(MudComponentBase)));
        WriteSummaryIndented(type.Summary);
        WriteRemarksIndented(type.Remarks);
        WriteProperties(type);
        WriteFields(type);
        WriteMethods(type);
        Outdent();
        WriteLineIndented("});");
    }

    /// <summary>
    /// Serializes the specified properties.
    /// </summary>
    /// <param name="type">The type containing the properties.</param>
    public void WriteProperties(DocumentedType type)
    {
        /* Example:
         
            Properties = { 
				{ "JavaScriptListenerId", new() { Type = "Guid", Summary = "Gets the ID of the JavaScript listener.",  } },
				{ "BrowserWindowSize", new() { Type = "BrowserWindowSize", Summary = "Gets the browser window size.",  } },
				{ "Breakpoint", new() { Type = "Breakpoint", Summary = "Gets the breakpoint associated with the browser size.",  } },
				{ "IsImmediate", new() { Type = "Boolean",  } },
            },
          
         */

        // Anything to do?
        if (type.Properties.Count == 0)
        {
            return;
        }

        WriteLineIndented("Properties = { ");
        Indent();

        foreach (var property in type.Properties)
        {
            WriteProperty(type, property.Value);
        }

        Outdent();
        WriteLineIndented("},");
    }

    /// <summary>
    /// Serializes the specified property.
    /// </summary>
    /// <param name="type">The current type being serialized.</param>
    /// <param name="property">The property to serialize.</param>
    public void WriteProperty(DocumentedType type, DocumentedProperty property)
    {
        if (ExcludedTypes.Contains(property.DeclaringTypeName))
        {
            return;
        }

        /* Example:
         
        	{ "BrowserWindowSize", new() { Type = "BrowserWindowSize", Summary = "Gets the browser window size.",  } },
		
         */

        WriteIndented("{ ");
        Write($"\"{property.Name}\", new()");
        Write(" { ");
        Write($"Name = \"{property.Name}\", ");
        Write($"Type = \"{property.PropertyTypeName}\", ");
        WriteDeclaringType(type, property);
        WriteCategory(property.Category);
        WriteIsParameter(property.IsParameter);
        WriteSummary(property.Summary);
        WriteRemarks(property.Remarks);

        Write(" }");
        WriteLine(" },");
    }

    /// <summary>
    /// Serializes the specified methods.
    /// </summary>
    /// <param name="type">The type containing the methods.</param>
    /// <param name="properties">The methods to serialize.</param>
    public void WriteMethods(DocumentedType type)
    {
        /* Example:

           Methods = { 
               { "SetValue", new() { Type = "Guid", Summary = "Gets the ID of the JavaScript listener.",  } },
               { "BrowserWindowSize", new() { Type = "BrowserWindowSize", Summary = "Gets the browser window size.",  } },
               { "Breakpoint", new() { Type = "Breakpoint", Summary = "Gets the breakpoint associated with the browser size.",  } },
               { "IsImmediate", new() { Type = "Boolean",  } },
           },

        */

        // Get the non-excluded methods
        var methods = type.Methods.Where(method => !ExcludedMethods.Contains(method.Value.Name) && !ExcludedTypes.Contains(method.Value.DeclaringTypeName)).ToList();

        // Anything to do?
        if (methods.Count == 0)
        {
            return;
        }

        WriteLineIndented("Methods = { ");
        Indent();

        foreach (var method in methods)
        {
            WriteMethod(type, method.Value);
        }

        Outdent();
        WriteLineIndented("},");
    }

    /// <summary>
    /// Serializes the specified method.
    /// </summary>
    /// <param name="type">The current type being serialized.</param>
    /// <param name="method">The method to serialize.</param>
    public void WriteMethod(DocumentedType type, DocumentedMethod method)
    {
        if (ExcludedMethods.Contains(method.Name) || ExcludedTypes.Contains(method.DeclaringTypeName) || method.Name.StartsWith('<'))
        {
            return;
        }

        /* Example:
         
        	{ "BrowserWindowSize", new() { Type = "BrowserWindowSize", Summary = "Gets the browser window size.",  } },
		
         */

        WriteIndented("{ ");
        Write($"\"{method.Name}\", new()");
        Write(" { ");
        Write($"Name = \"{method.Name}\", ");
        WriteReturnType(type, method);
        WriteDeclaringType(type, method);
        WriteSummary(method.Summary);
        WriteRemarks(method.Remarks);

        Write(" }");
        WriteLine(" },");
    }

    /// <summary>
    /// Writes whether the type inherits from <see cref="MudComponentBase"/>.
    /// </summary>
    /// <param name="isComponent"></param>
    public void WriteIsComponentIndented(bool isComponent)
    {
        if (isComponent)
        {
            WriteIndent();
            WriteLine($"IsComponent = true, ");
        }
    }

    /// <summary>
    /// Writes the type in which the property was declared, if it's another type.
    /// </summary>
    /// <param name="type">The type containing the property.</param>
    /// <param name="method">The property being described.</param>
    public void WriteReturnType(DocumentedType type, DocumentedMethod method)
    {
        // Is this property declared in another type (like a base class)?
        if (!string.IsNullOrEmpty(method.DeclaringTypeName) && type.Name != method.DeclaringTypeName && method.ReturnTypeName != "Void")
        {
            Write($"ReturnType = \"{Escape(method.ReturnTypeName)}\", ");
        }
    }

    /// <summary>
    /// Writes whether a property is a parameter.
    /// </summary>
    /// <param name="isParameter"></param>
    public void WriteIsParameter(bool isParameter)
    {
        if (isParameter)
        {
            Write($"IsParameter = true, ");
        }
    }

    /// <summary>
    /// Writes the name of the given base type.
    /// </summary>
    /// <param name="baseType"></param>
    public void WriteBaseTypeIndented(Type baseType)
    {
        if (baseType != null)
        {
            WriteLineIndented($"BaseTypeName = \"{baseType.Name}\", ");
        }
    }

    /// <summary>
    /// Writes the type in which the property was declared, if it's another type.
    /// </summary>
    /// <param name="type">The type containing the property.</param>
    /// <param name="property">The property being described.</param>
    public void WriteDeclaringType(DocumentedType type, DocumentedProperty property)
    {
        // Is this property declared in another type (like a base class)?
        if (!string.IsNullOrEmpty(property.DeclaringTypeName) && type.Name != property.DeclaringTypeName)
        {
            Write($"DeclaringType = \"{Escape(property.DeclaringTypeName)}\", ");
        }
    }

    /// <summary>
    /// Writes the type in which the property was declared, if it's another type.
    /// </summary>
    /// <param name="type">The type containing the property.</param>
    /// <param name="method">The property being described.</param>
    public void WriteDeclaringType(DocumentedType type, DocumentedMethod method)
    {
        // Is this property declared in another type (like a base class)?
        if (!string.IsNullOrEmpty(method.DeclaringTypeName) && type.Name != method.DeclaringTypeName)
        {
            Write($"DeclaringType = \"{Escape(method.DeclaringTypeName)}\", ");
        }
    }

    /// <summary>
    /// Serializes all fields for the specified type.
    /// </summary>
    /// <param name="type">The type being serialized.</param>
    public void WriteFields(DocumentedType type)
    {
        if (type.Fields.Count == 0)
        {
            return;
        }

        WriteLineIndented("Fields = { ");
        Indent();

        foreach (var field in type.Fields)
        {
            WriteField(field);
        }

        Outdent();
        WriteLineIndented("},");
    }

    /// <summary>
    /// Serializes the specified field.
    /// </summary>
    /// <param name="field">The field to document.</param>
    public void WriteField(KeyValuePair<string, DocumentedField> field)
    {
        WriteIndented("{ ");
        Write($"\"{field.Value.Name}\", new()");
        Write(" { ");
        WriteSummary(field.Value.Summary);
        Write(" }");
        WriteLine(" },");
    }
}
