// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace MudBlazor.Interfaces
{
    /// <summary>
    /// Connects an input to a <see cref="MudForm"/> so the form can validate and reset it.
    /// </summary>
    public interface IFormComponent
    {
        /// <summary>
        /// Requires an input value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, an error with the text in the implementors RequiredError will be shown during validation if no input was given.
        /// </remarks>
        bool Required { get; set; }

        /// <summary>
        /// Displays an error.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the text in the implementors ErrorText field is displayed.
        /// </remarks>
        bool Error { get; set; }

        /// <summary>
        /// Indicates any error, conversion error, or validation error with this input.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the <see cref="Error"/> property is <c>true</c>, or the implementors ConversionError is <c>true</c>, or one or more ValidationErrors exists.
        /// </remarks>
        bool HasErrors { get; }

        /// <summary>
        /// Indicates whether the user has interacted with this input or the focus has been released.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user has performed input, or focus has moved away from this input.  This property is typically used to show the implementors RequiredError text only after the user has interacted with this input.
        /// </remarks>
        bool Touched { get; }

        /// <summary>
        /// Indicates that this input has a non-null, non-empty value.
        /// </summary>
        /// <remarks>Default implementation which indicates the input has been <see cref="Touched"/></remarks>
        /// <returns>True if some value exists, or false if null or empty</returns>
        bool HasValue() => Touched; // Default implementation for backwards compatibility; built-in inputs override this to report actual value presence.

        /// <summary>
        /// The function used to detect problems with the input.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, this property can be any of several kinds of functions:
        /// <para>
        /// 1. A <c>Func&lt;T,bool&gt;</c> or <c>Func&lt;T,Task&lt;bool&gt;&gt;</c> function.  Returns <c>true</c> if valid.  When <c>false</c>, a standard <c>"Invalid"</c> message is shown.
        /// </para>
        /// <para>
        /// 2. A <c>Func&lt;T,string&gt;</c> or <c>Func&lt;T,Task&lt;string&gt;&gt;</c> function.  Returns <c>null</c> if valid, or a string explaining the error.
        /// </para>
        /// <para>
        /// 3. A <c>Func&lt;T,IEnumerable&lt;string&gt;&gt;</c> or <c>Func&lt;T,Task&lt;IEnumerable&lt;string&gt;&gt;&gt;</c> function.  Returns an empty list if valid, or a list of validation errors.
        /// </para>
        /// <para>
        /// 3. A <c>Func&lt;object,string,IEnumerable&lt;string&gt;&gt;</c> or <c>Func&lt;object,string,Task&lt;IEnumerable&lt;string&gt;&gt;&gt;</c> function.  Given the form model and path to the member, returns an empty list if valid, or a list of validation errors.
        /// </para>
        /// <para>
        /// 4. A <see cref="ValidationAttribute"/> object.
        /// </para>
        /// </remarks>
        object? Validation { get; set; }

        /// <summary>
        /// Indicates that the <see cref="MudFormComponent{T,U}.For"/> field identifier expression is null
        /// </summary>
        /// <remarks>The <see cref="MudFormComponent{T,U}.For"/> expression is used to uniquely identify a form field to perform validation. </remarks>
        bool IsForNull { get; }

        /// <summary>
        /// The list of problems with the current input value.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, this property is updated when validation has been performed.  Use the <see cref="Validation"/> property to control what validations are performed.
        /// </remarks>
        List<string> ValidationErrors { get; set; }

        /// <summary>
        /// Causes validation to be performed for this input.
        /// </summary>
        /// <remarks>
        /// When using a <see cref="MudForm"/>, the input is validated via the function set in the <see cref="Validation"/> property.
        /// </remarks>
        Task ValidateAsync();

        /// <summary>
        /// Clears the input and any validation errors.
        /// </summary>
        /// <remarks>
        /// When called, the <c>Value</c>, <see cref="Error"/>, <see cref="MudFormComponent{T,U}.ErrorText"/>, and <see cref="ValidationErrors"/> properties are all reset.
        /// </remarks>
        Task ResetAsync();

        /// <summary>
        /// Clears any validation errors.
        /// </summary>
        /// <remarks>
        /// When called, the <see cref="Error"/>, <see cref="MudFormComponent{T,U}.ErrorText"/>, and <see cref="ValidationErrors"/> properties are all reset.
        /// </remarks>
        Task ResetValidationAsync();
    }
}
