// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A selectable item displayed within a <see cref="MudComboBox{T}"/> component.
    /// </summary>
    /// <typeparam name="T">The type of value linked to this item. Must be the same type as the parent <see cref="MudSelect{T}"/>.</typeparam>
    public partial class MudComboBoxItem<T> : MudComponentBase, IDisposable
    {
        /// <summary>
        /// The <see cref="MudComboBox{T}"/> hosting this item.
        /// </summary>
        [CascadingParameter]
        internal MudComboBox<T>? ComboBox { get; set; }

        private bool _registered;
        private bool _isDisposed;

        /// <summary>
        /// The custom value associated with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// The custom content within this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public bool Disabled { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender && !_registered && !_isDisposed)
            {
                ComboBox?.RegisterItem(this);
                _registered = true;
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            ComboBox?.UnRegisterItem(this);
            _registered = false;
        }
    }
}
