// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a form input component which stores a boolean value.
    /// </summary>
    /// <typeparam name="T">The type of item managed by this component.</typeparam>
    public class MudBooleanInput<T> : MudFormComponent<T?, bool?>
    {
        public MudBooleanInput() : base(new BoolConverter<T?>()) { }

        protected virtual string? Classname { get; set; }
        protected virtual string? LabelClassname { get; set; }
        protected virtual string? IconClassname { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// Prevents the user from changing the input.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the user can copy the input but cannot change it.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// The currently selected value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T? Value
        {
            get => _value;
            set
            {
                _value = value;

            }
        }

        /// <summary>
        /// Prevents the parent component from receiving click events.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, the click will not bubble up to parent components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool StopClickPropagation { get; set; } = true;

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The location of the label relative to the input icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Placement LabelPlacement { get; set; } = Placement.End;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Default;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Value"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        protected bool? BoolValue => Converter.Set(Value);

        protected virtual Task OnChange(ChangeEventArgs args)
        {
            return SetBoolValueAsync((bool?)args.Value, true);
        }

        protected Task SetBoolValueAsync(bool? value, bool? markAsTouched = null)
        {
            if (markAsTouched is true)
            {
                Touched = true;
            }
            return SetCheckedAsync(Converter.Get(value));
        }

        protected async Task SetCheckedAsync(T? value)
        {
            if (GetDisabledState())
            {
                return;
            }

            if (!EqualityComparer<T>.Default.Equals(Value, value))
            {
                Value = value;
                await ValueChanged.InvokeAsync(value);
                await BeginValidateAsync();
                FieldChanged(Value);
            }
        }

        protected override bool SetConverter(Converter<T?, bool?> value)
        {
            var changed = base.SetConverter(value);
            if (changed)
            {
                SetBoolValueAsync(Converter.Set(Value)).CatchAndLog();
            }

            return changed;
        }

        /// <summary>
        /// A value is required, so if not checked we return ERROR.
        /// </summary>
        protected override bool HasValue(T? value)
        {
            return BoolValue == true;
        }

        protected Placement ConvertPlacement(Placement placement)
        {
            return placement switch
            {
                Placement.Left => Placement.Start,
                Placement.Right => Placement.End,
                _ => placement
            };
        }
    }
}
