using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudCheckBox<T> : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
        .Build();
        protected string CheckBoxClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-checkbox-{Color.ToDescriptionString()}")
            .AddClass($"mud-ripple mud-ripple-checkbox", !DisableRipple)
            .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
        .Build();

        //[CascadingParameter] internal MudForm Form { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the checkbox will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// A callback when Checked changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> CheckedChanged { get; set; }

        private T _checked;
        private bool? _value
        {
            get
            {
                try
                {
                    if (_checked is bool)
                        return (bool) (object) _checked;
                    else if (_checked is bool?)
                        return (bool?) (object) _checked;
                    else if (_checked is string)
                    {
                        var s = (string) (object) _checked;
                        if (string.IsNullOrWhiteSpace(s))
                            return null;
                        if (bool.TryParse(s, out var b))
                            return b;
                        if (s.ToLowerInvariant() == "on")
                            return true;
                        if (s.ToLowerInvariant() == "off")
                            return false;
                        return null;
                    }
                    return null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            set
            {
                if (typeof(T) == typeof(bool))
                    Checked = (T) (object)(value == true);
                else if (typeof(T) == typeof(bool?))
                    Checked = (T)(object)value;
                else if (typeof(T) == typeof(string))
                    Checked = (T) (object) (value == true ? "on" : (value == false ? "off" : null));
            }
        }

        /// <summary>
        /// The state of the checkbox
        /// </summary>
        [Parameter] public T Checked
        {
            get => _checked;
            set
            {
                if (object.Equals(value, _checked))
                    return;
                _checked = value;
                CheckedChanged.InvokeAsync(value);
            }   
        }

        //protected override Task OnInitializedAsync()
        //{
        //    Form?.Add(this);
        //    if (_converter != null)
        //        _converter.OnError = OnConversionError;
        //    return base.OnInitializedAsync();
        //}
    }
}
