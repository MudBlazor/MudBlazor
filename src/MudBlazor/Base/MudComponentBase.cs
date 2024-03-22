using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using MudBlazor.State;

namespace MudBlazor
{
#nullable enable
    public abstract partial class MudComponentBase : ComponentBase, IMudStateHasChanged
    {
        private readonly ParameterSet _parameters = new();

        [Inject]
        private ILoggerFactory LoggerFactory { get; set; } = null!;
        private ILogger? _logger;
        protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());

        /// <summary>
        /// User class names, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string? Class { get; set; }

        /// <summary>
        /// User styles, applied on top of the component's own classes and styles.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public string? Style { get; set; }

        /// <summary>
        /// Use Tag to attach any user data object to the component for your convenience.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ComponentBase.Common)]
        public object? Tag { get; set; }

        /// <summary>
        /// UserAttributes carries all attributes you add to the component that don't match any of its parameters.
        /// They will be splatted onto the underlying HTML tag.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        [Category(CategoryTypes.ComponentBase.Common)]
        public Dictionary<string, object?> UserAttributes { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="JSRuntime" /> is available.
        /// </summary>
        protected bool IsJSRuntimeAvailable { get; set; }

        /// <summary>
        /// If the UserAttributes contain an ID make it accessible for WCAG labelling of input fields
        /// </summary>
        public string FieldId => (UserAttributes?.ContainsKey("id") == true ? UserAttributes["id"]?.ToString() ?? $"mudinput-{Guid.NewGuid()}" : $"mudinput-{Guid.NewGuid()}");

        /// <inheritdoc />
        protected override void OnAfterRender(bool firstRender)
        {
            IsJSRuntimeAvailable = true;
            base.OnAfterRender(firstRender);
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            _parameters.OnInitialized();
        }

        /// <inheritdoc />
        public override Task SetParametersAsync(ParameterView parameters)
        {
            return _parameters.SetParametersAsync(base.SetParametersAsync, parameters);
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _parameters.OnParametersSet();
        }

        /// <inheritdoc />
        void IMudStateHasChanged.StateHasChanged() => StateHasChanged();
    }
}
