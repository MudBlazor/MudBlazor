﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudRadio<T> : MudBooleanInput<T>, IDisposable
    {
        private IMudRadioGroup? _parent;
        private string _elementId = Identifier.Create("radio");

        protected override string Classname => new CssBuilder("mud-input-control-boolean-input")
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass("mud-readonly", GetReadOnlyState())
            .AddClass("mud-input-with-content", ChildContent is not null)
            .AddClass(Class)
            .Build();

        protected override string LabelClassname => new CssBuilder("mud-radio")
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-input-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
            .Build();

        protected override string IconClassname => new CssBuilder("mud-button-root mud-icon-button")
            .AddClass("mud-ripple mud-ripple-radio", Ripple && !GetDisabledState() && !GetReadOnlyState())
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && (UncheckedColor == null || (UncheckedColor != null && Checked)))
            .AddClass($"mud-{UncheckedColor?.ToDescriptionString()}-text hover:mud-{UncheckedColor?.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor != null && Checked == false)
            .AddClass("mud-radio-dense", Dense)
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass("mud-readonly", GetReadOnlyState())
            .AddClass("mud-checked", Checked)
            .AddClass("mud-error-text", MudRadioGroup?.HasErrors)
            .Build();

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        /// <summary>
        /// The parent Radio Group
        /// </summary>
        [CascadingParameter]
        internal IMudRadioGroup? IMudRadioGroup
        {
            get => _parent;
            set
            {
                _parent = value;
                _parent?.CheckGenericTypeMatch(this);
            }
        }

        /// <summary>
        /// The base color of the component in its none active/unchecked state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? UncheckedColor { get; set; } = null;

        /// <summary>
        /// If true, compact padding will be applied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The icon to display for a checked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.RadioButtonChecked"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonChecked;

        /// <summary>
        /// The icon to display for an unchecked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.RadioButtonUnchecked"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.RadioButtonUnchecked;

        /// <summary>
        /// The icon to display for an indeterminate state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        private string GetIcon()
        {
            return Checked switch
            {
                true => CheckedIcon,
                false => UncheckedIcon
            };
        }

        internal bool Checked { get; private set; }

        internal MudRadioGroup<T>? MudRadioGroup => (MudRadioGroup<T>?)IMudRadioGroup;

        internal void SetChecked(bool value)
        {
            if (Checked != value)
            {
                Checked = value;
                StateHasChanged();
            }
        }

        public Task SelectAsync()
        {
            if (MudRadioGroup is not null)
            {
                return MudRadioGroup.SetSelectedRadioAsync(this);
            }

            return Task.CompletedTask;
        }

        internal Task OnClickAsync()
        {
            if (GetDisabledState() || (MudRadioGroup?.GetReadOnlyState() ?? false))
            {
                return Task.CompletedTask;
            }

            if (MudRadioGroup != null)
            {
                return MudRadioGroup.SetSelectedRadioAsync(this);
            }

            return Task.CompletedTask;
        }

        protected internal async Task HandleKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
        {
            if (GetDisabledState() || (MudRadioGroup?.GetReadOnlyState() ?? false))
            {
                return;
            }

            switch (keyboardEventArgs.Key)
            {
                case "Enter" or "NumpadEnter" or " ":
                    await SelectAsync();
                    break;
                case "Backspace":
                    {
                        if (MudRadioGroup is not null)
                        {
                            await MudRadioGroup.ResetAsync();
                        }

                        break;
                    }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (MudRadioGroup is not null)
            {
                await MudRadioGroup.RegisterRadioAsync(this);
            }
        }

        public void Dispose()
        {
            MudRadioGroup?.UnregisterRadio(this);
            if (IsJSRuntimeAvailable)
            {
                // TODO: Replace with IAsyncDisposable
                KeyInterceptorService.UnsubscribeAsync(_elementId).CatchAndLog();
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

                await KeyInterceptorService.SubscribeAsync(_elementId, options, KeyObserver.KeyDownIgnore(), KeyObserver.KeyUpIgnore());
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
