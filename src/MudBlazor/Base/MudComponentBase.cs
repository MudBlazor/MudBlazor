// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing MudBlazor components.
    /// </summary>
    public abstract class MudComponentBase : ComponentBaseWithState, IMudStateHasChanged
    {
        [Inject]
        private ILoggerFactory LoggerFactory { get; set; } = null!;
        private ILogger? _logger;
        protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());

        /// <summary>
        /// The CSS classes applied to this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.  Use the <see cref="Style"/> property to apply custom CSS styles.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string? Class { get; set; }

        /// <summary>
        /// The CSS styles applied to this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use the <see cref="Class"/> property to apply CSS classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string? Style { get; set; }

        /// <summary>
        /// The arbitrary object to link to this component.
        /// </summary>
        /// <remarks>
        /// This property is typically used to associate additional information with this component, such as a model containing data for this component.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public object? Tag { get; set; }

        /// <summary>
        /// The additional HTML attributes to apply to this component.
        /// </summary>
        /// <remarks>
        /// This property is typically used to provide additional HTML attributes during rendering such as ARIA accessibility tags or a custom ID.
        /// </remarks>
        [Parameter(CaptureUnmatchedValues = true)]
        [Category(CategoryTypes.ComponentBase.Common)]
        public Dictionary<string, object?> UserAttributes { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Whether the <see cref="JSRuntime" /> is available.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, JavaScript interop calls can be made.
        /// </remarks>
        protected bool IsJSRuntimeAvailable { get; set; }

        private readonly string _id = Identifier.Create("mudinput");
        /// <summary>
        /// If the UserAttributes contain an ID make it accessible for WCAG labelling of input fields
        /// </summary>
        public string FieldId => UserAttributes.TryGetValue("id", out var id) && id is not null
            ? id.ToString() ?? _id
            : _id;

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            IsJSRuntimeAvailable = true;
            base.OnAfterRender(firstRender);
        }

        /// <inheritdoc />
        void IMudStateHasChanged.StateHasChanged() => StateHasChanged();

    }
}
