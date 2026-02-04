using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A form input for boolean values or selecting multiple items in a list. Use checkboxes (instead of switches or radio buttons) if multiple options can be selected from a list.
    /// </summary>
    /// <typeparam name="T">The type of item managed by this checkbox.</typeparam>
    /// <seealso cref="MudRadio{T}"/>
    /// <seealso cref="MudSwitch{T}"/>
    public partial class MudCheckBox<T> : MudBooleanInput<T>
    {
        private readonly string _elementId = Identifier.Create("checkbox");
        private readonly string _ariaId = Identifier.Create("cbox-aria-");

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        protected override string Classname => new CssBuilder("mud-input-control-boolean-input")
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass("mud-input-with-content", ChildContent is not null)
            .AddClass(Class)
            .Build();

        protected override string LabelClassname => new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-input-content-placement-{ConvertPlacement(LabelPlacement).ToStringFast(true)}")
            .Build();

        protected override string IconClassname => new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-{Color.ToStringFast(true)}-text hover:mud-{Color.ToStringFast(true)}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor == null || (UncheckedColor != null && BoolValue == true))
            .AddClass($"mud-{UncheckedColor?.ToStringFast(true)}-text hover:mud-{UncheckedColor?.ToStringFast(true)}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor != null && BoolValue == false)
            .AddClass($"mud-checkbox-dense", Dense)
            .AddClass($"mud-ripple mud-ripple-checkbox", Ripple && !GetReadOnlyState() && !GetDisabledState())
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-checkbox-true", BoolValue == true)
            .AddClass($"mud-checkbox-false", BoolValue == false)
            .AddClass($"mud-checkbox-null", BoolValue is null)
            .Build();

        /// <summary>
        /// The color of the checkbox when its <c>Value</c> is <c>false</c> or <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? UncheckedColor { get; set; } = null;

        /// <summary>
        /// The Aria Label to be assigned to the checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Used to improve accessibility for screen readers. Adds an aria-labelledby to <c>UserAttributes</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Allows this checkbox to be controlled via the keyboard.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  The <c>Space</c> key cycles through true and false values (or true/false/null states if <see cref="TriState"/> is <c>true</c>). <c>Delete</c> will clear the checkbox. <c>Enter</c> (or <c>NumPadEnter</c>) will set the checkbox.  <c>Backspace</c> will set an indeterminate value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyboardEnabled { get; set; } = true;

        /// <summary>
        /// Uses compact padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The icon to display for a checked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon to display for an unchecked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// The icon to display for an indeterminate state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// Allows the checkbox to have an indeterminate state.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the checkbox can support an indeterminate state such as a <c>null</c> value, in addition to <c>true</c> and <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool TriState { get; set; }

        private string GetIcon()
        {
            return BoolValue switch
            {
                true => CheckedIcon,
                false => UncheckedIcon,
                _ => IndeterminateIcon
            };
        }

        protected override Task OnChange(ChangeEventArgs args)
        {
            // Apply only when TriState parameter is set to true and T is bool?
            if (TriState && typeof(T) == typeof(bool?))
            {
                // The cycle is forced with the following steps: true, false, indeterminate, true, false, indeterminate...
                var boolValue = (bool?)(object?)ReadValue;
                if (!boolValue.HasValue)
                {
                    return SetBoolValueAsync(true, true);
                }

                return boolValue.Value
                    ? SetBoolValueAsync(false, true)
                    : SetBoolValueAsync(default, true);
            }

            return SetBoolValueAsync((bool?)args.Value, true);
        }

        protected Task HandleKeyDownAsync(KeyboardEventArgs obj) => KeyInterceptorService.DispatchAsync(_elementId, KeyEventKind.Down, obj);

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // if Aria Label has a value add aria-labeled by
            if (!string.IsNullOrWhiteSpace(AriaLabel))
            {
                // we use try add in case the user has already set an aria-labelledby attribute
                UserAttributes.TryAdd("aria-labelledby", _ariaId);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-button-root",
                    [
                        // prevent scrolling page
                        new(" ", preventDown: "key+none", preventUp: "key+none"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        new("Backspace", preventDown: "key+none")
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keys => keys
                    .When(CanHandleKeys, builder => builder
                        .OnKeyDown("Delete", () => SetBoolValueAsync(false, true))
                        .OnKeyDownAny(["Enter", "NumpadEnter"], () => SetBoolValueAsync(true, true))
                        .OnKeyDown("Backspace", HandleBackspaceAsync)
                        .OnKeyDown(" ", HandleSpaceAsync)));
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Determines whether the checkbox is considered to have a value.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a value is considered present; otherwise, <c>false</c>.<br/>
        /// When <see cref="TriState"/> is <c>true</c>, this returns <c>true</c> if <paramref name="value"/> is not <c>null</c>.
        /// Otherwise, it defers to the base implementation.
        /// </returns>
        protected override bool HasValue(T? value)
        {
            if (TriState)
            {
                return BoolValue is not null;
            }

            return base.HasValue(value);
        }

        private bool CanHandleKeys() => KeyboardEnabled && !GetDisabledState() && !GetReadOnlyState();

        private Task HandleSpaceAsync()
        {
            bool? nextValue = BoolValue switch
            {
                null => true,
                true => false,
                false when TriState => null,
                false => true
            };

            return SetBoolValueAsync(nextValue, true);
        }

        private Task HandleBackspaceAsync() => TriState
                ? SetBoolValueAsync(null, true)
                : Task.CompletedTask;

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }
    }
}
