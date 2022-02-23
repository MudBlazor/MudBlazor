using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudBooleanInput<T> : MudFormComponent<T, bool?>
    {
        public MudBooleanInput() : base(new BoolConverter<T>()) { }

        /// <summary>
        /// If true, the input will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the input will be read-only.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// The state of the component
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public T Checked
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// If true will prevent the click from bubbling up the event tree.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool StopClickPropagation { get; set; } = true;

        /// <summary>
        /// Fired when Checked changes.
        /// </summary>
        [Parameter] public EventCallback<T> CheckedChanged { get; set; }

        protected bool? BoolValue => Converter.Set(Checked);

        protected virtual Task OnChange(ChangeEventArgs args)
        {
            Touched = true;
            return SetBoolValueAsync((bool?)args.Value);
        }

        protected Task SetBoolValueAsync(bool? value)
        {
            return SetCheckedAsync(Converter.Get(value));
        }

        protected async Task SetCheckedAsync(T value)
        {
            if (Disabled)
                return;
            if (!EqualityComparer<T>.Default.Equals(Checked, value))
            {
                Checked = value;
                await CheckedChanged.InvokeAsync(value);
                BeginValidate();
            }
        }

        protected override bool SetConverter(Converter<T, bool?> value)
        {
            var changed = base.SetConverter(value);
            if (changed)
                SetBoolValueAsync(Converter.Set(Checked)).AndForget();

            return changed;
        }

        /// <summary>
        /// A value is required, so if not checked we return ERROR.
        /// </summary>
        protected override bool HasValue(T value)
        {
            return (BoolValue == true);
        }
    }
}
