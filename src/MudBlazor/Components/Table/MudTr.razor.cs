﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTr : MudComponentBase
    {
        internal bool _clickRowFirstTime;

        internal object _itemCopy;

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class).Build();

        [CascadingParameter] public TableContext Context { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public object Item { get; set; }

        [Parameter] public bool IsCheckable { get; set; }

        [Parameter] public bool IsEditable { get; set; }

        [Parameter] public bool IsHeader { get; set; }

        [Parameter] public bool IsFooter { get; set; }

        [Parameter]
        public EventCallback<bool> IsCheckedChanged { get; set; }

        private bool _checked;
        [Parameter]
        public bool IsChecked
        {
            get => _checked;
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    IsCheckedChanged.InvokeAsync(value);
                }
            }
        }

        public void OnRowClicked(MouseEventArgs args)
        {
            // Manage the Item Copy the first time the row is clicked
            if (!_clickRowFirstTime)
            {
                ManageItemCopy();
            }

            if (IsHeader || !(Context?.Table.Validator.IsValid ?? true))
                return;

            Context?.Table.SetSelectedItem(Item);
            Context?.Table.SetEditingItem(Item);

            if (Context?.Table.MultiSelection == true && !IsHeader)
            {
                IsChecked = !IsChecked;
            }
            Context?.Table.FireRowClickEvent(args, this, Item);
        }

        protected override Task OnInitializedAsync()
        {
            Context?.Add(this, Item);
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            Context?.Remove(this, Item);
        }

        public void SetChecked(bool b, bool notify)
        {
            if (notify)
                IsChecked = b;
            else
            {
                _checked = b;
                InvokeAsync(StateHasChanged);
            }
        }

        private void FinishEdit(MouseEventArgs ev)
        {
            _clickRowFirstTime = false;
            if (!Context?.Table.Validator.IsValid ?? true) return;
            Context?.Table.SetEditingItem(null);
            Context?.Table.OnCommitEditHandler(ev, Item);
        }

        private void CancelEdit(MouseEventArgs ev)
        {
            // The Item object is reset to its initial value from the Item Copy
            ManageItemCopy();

            // The edit mode is canceled
            Context?.Table.SetEditingItem(null);
            Context?.Table.OnCancelEditHandler(ev);
        }

        private void ManageItemCopy()
        {
            try
            {
                if (IsEditable && Item != null)
                {
                    if (!_clickRowFirstTime)
                    {
                        if (Item.GetType() == typeof(string))
                        {
                            _itemCopy = string.Empty;
                        }
                        else
                        {
                            _itemCopy = Activator.CreateInstance(Item.GetType());
                        }

                        CopyObjectData(Item, _itemCopy);
                        _clickRowFirstTime = true;
                    }
                    else
                    {
                        CopyObjectData(_itemCopy, Item);
                        _clickRowFirstTime = false;
                    }
                }
            }
            catch
            {
                /* ignore */
            }
        }

        private static void CopyObjectData(object source, object target)
        {
            var memberInfos = target.GetType().GetMembers();

            foreach (var field in memberInfos)
            {
                var memberInfoName = field.Name;

                if (field.MemberType == MemberTypes.Field)
                {
                    var sourceField = source.GetType().GetField(memberInfoName);

                    if (sourceField == null)
                    {
                        continue;
                    }

                    var sourceValue = sourceField.GetValue(source);
                    ((FieldInfo)field).SetValue(target, sourceValue);
                }
                else if (field.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = field as PropertyInfo;
                    var sourceField = source.GetType().GetProperty(memberInfoName);

                    if (sourceField == null)
                    {
                        continue;
                    }

                    if (propertyInfo.CanWrite && sourceField.CanRead)
                    {
                        var sourceValue = sourceField.GetValue(source, null);
                        propertyInfo.SetValue(target, sourceValue, null);
                    }
                }
            }
        }
    }
}
