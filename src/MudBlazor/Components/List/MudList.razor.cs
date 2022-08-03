using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudList<T> : MudComponentBase, IDisposable
    {
        [Inject] private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        private IKeyInterceptor _keyInterceptor;
        private string _elementId = "list_" + Guid.NewGuid().ToString().Substring(0, 8);
        private bool _multiSelection = false;
        private List<MudListItem<T>> _items = new();
        private List<MudList<T>> _childLists = new();
        private MudListItem<T> _selectedItem = new();
        private List<MudListItem<T>> _selectedItems = new();
        private T _selectedValue;
        private HashSet<T> _selectedValues = new();
        internal MudListItem<T> _lastActivatedItem;
        bool _allSelected = false;

        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudList<T> ParentList { get; set; }

        private ValueTask ScrollToItemAsync(MudListItem<T> item)
            => item != null ? ScrollManager.ScrollToListItemAsync(item.ItemId) : ValueTask.CompletedTask;

        protected internal ValueTask ScrollToMiddleAsync(MudListItem<T> item)
            => ScrollManager.ScrollToMiddleAsync(_elementId, item.ItemId);

        /// <summary>
        /// The color of the selected List Item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Allows multi selection and adds MultiSelectionComponent for each list item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool MultiSelection
        {
            get => _multiSelection;

            set
            {
                if (_multiSelection == value)
                {
                    return;
                }
                _multiSelection = value;
                if (_multiSelection == false)
                {
                    HandleCentralValueCommander("MultiSelectionOff").AndForget();
                    UpdateSelectedStyles();
                }
            }
        }

        /// <summary>
        /// The MultiSelectionComponent's placement. Accepts Align.Start and Align.End
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Align MultiSelectionAlign { get; set; } = Align.Start;

        /// <summary>
        /// The component which shows as a MultiSelection check.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public MultiSelectionComponent MultiSelectionComponent { get; set; } = MultiSelectionComponent.CheckBox;

        /// <summary>
        /// Set true to make the list items clickable. This is also the precondition for list selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool Clickable { get; set; }

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, change background color to secondary for all nested item headers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool SecondaryBackgroundColorForNestedItemHeader { get; set; }

        /// <summary>
        /// Fired on the KeyDown event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }


        public List<MudListItem<T>> AllItems
        {
            get => CollectAllMudListItems();
        }


        internal async Task HandleCentralValueCommander(string changedValueType)
        {
            await Task.Delay(1);
            if (changedValueType == "SelectedValue")
            {
                if (!MultiSelection)
                {
                    SelectedValues = new List<T>() { SelectedValue };
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == "SelectedValues")
            {
                if (MultiSelection)
                {
                    SelectedValue = SelectedValues == null ? default(T) : SelectedValues.LastOrDefault();
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == "MultiSelectionOff")
            {
                var items = CollectAllMudListItems();
                SelectedValues = new List<T>() { SelectedValue };
                SelectedItems = items.Where(x => SelectedValues.Contains(x.Value)).ToList();
            }
        }

        /// <summary>
        /// The current selected value.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                if ((_selectedValue != null && value != null && _selectedValue.ToString() == value.ToString()) || (_selectedValue == null && value == null))
                    return;
                _selectedValue = value;
                HandleCentralValueCommander("SelectedValue").AndForget();

                SelectedValueChanged.InvokeAsync(_selectedValue).AndForget();
                UpdateSelectedStyles();
            }
        }

        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                if (_selectedValues == null)
                {
                    return new List<T>();
                }
                return _selectedValues;
            }

            set
            {
                var set = value ?? new List<T>();
                if ((_selectedValues != null && value != null && _selectedValues == value) || (_selectedValues == null && value == null))
                {
                    return;
                }

                if (SelectedValues.Count() == set.Count() && _selectedValues != null && _selectedValues.All(x => set.Contains(x)))
                    return;

                _selectedValues = value == null ? null : value.ToHashSet();
                HandleCentralValueCommander("SelectedValues").AndForget();

                SelectedValuesChanged.InvokeAsync(_selectedValues).AndForget();
                UpdateSelectedStyles();
            }
        }

        /// <summary>
        /// The current selected list item.
        /// Note: make the list Clickable or MultiSelection or both for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public MudListItem<T> SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            }
        }
        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<MudListItem<T>> SelectedItemChanged { get; set; }

        /// <summary>
        /// The current selected listitems.
        /// Note: make the list Clickable for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<MudListItem<T>> SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (_selectedItems == value)
                    return;

                _selectedItems = value == null ? null : value.ToList();
                SelectedItemsChanged.InvokeAsync(_selectedItems).AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<List<MudListItem<T>>> SelectedItemsChanged { get; set; }

        private void UpdateSelectedValues(bool once = false)
        {
            SelectedValues = new List<T>() { SelectedValue };
        }

        internal void UpdateSelectedItem()
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null);

            if (MultiSelection && (SelectedValues == null || SelectedValues.Count() == 0))
            {
                SelectedItem = null;
                SelectedItems = null;
                return;
            }

            SelectedItem = items.FirstOrDefault(x => x.Value?.ToString() == SelectedValue?.ToString());
            SelectedItems = items.Where(x => SelectedValues.Contains(x.Value)).ToList();
        }

        private void GetSelectedItem()
        {
            var items = CollectAllMudListItems();
            if (!MultiSelection)
            {
                SelectedItem = items.FirstOrDefault(x => x.Value != null && x.Value.Equals(_selectedValue));
            }
            else
            {
                SelectedItems = items.Where(x => SelectedValues != null && SelectedValues.Contains(x.Value));
            }
        }
        

        /// <summary>
        /// Called whenever the selection changed
        /// </summary>
        [Parameter] public EventCallback<T> SelectedValueChanged { get; set; }

        /// <summary>
        /// Fires when SelectedValues changes.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
                //CanSelect = ParentList.CanSelect;
            }
            //else
            //{
            //    CanSelect = SelectedItemChanged.HasDelegate || SelectedValueChanged.HasDelegate || SelectedValue != null;
            //}
        }

        internal event Action ParametersChanged;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-list-item",
                    Keys = {
                        new KeyOptions { Key=" ", PreventDown = "key+none" }, //prevent scrolling page, toggle open/close
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead hilight previous item
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead hilight next item
                        new KeyOptions { Key="Home", PreventDown = "key+none" },
                        new KeyOptions { Key="End", PreventDown = "key+none" },
                        new KeyOptions { Key="Escape" },
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="a", PreventDown = "key+ctrl" }, // select all items instead of all page text
                        new KeyOptions { Key="A", PreventDown = "key+ctrl" }, // select all items instead of all page text
                        new KeyOptions { Key="/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                    },
                });
                //GetSelectedItem();
            }
   
        }

        internal void Register(MudListItem<T> item)
        {
            _items.Add(item);
            if (SelectedValue != null && object.Equals(item.Value, SelectedValue))
            {
                item.SetSelected(true);
                //TODO check if item is the selectable for a nested list, and deselect this.
                //SelectedItem = item;
                //SelectedItemChanged.InvokeAsync(item);
            }
        }
        
        internal void Unregister(MudListItem<T> item)
        {
            _items.Remove(item);
        }

        internal void Register(MudList<T> child)
        {
            _childLists.Add(child);
        }

        internal void Unregister(MudList<T> child)
        {
            _childLists.Remove(child);
        }

        //internal void SetSelectedValue(T value, bool force = false, bool changeValue = true)
        //{
        //    if ((!Clickable && !MultiSelection) && !force)
        //        return;

        //    //Make sure its the most parent one before continue method
        //    if (ParentList != null)
        //    {
        //        ParentList?.SetSelectedValue(value);
        //        return;
        //    }

        //    var items = CollectAllMudListItems();

        //    SelectedItem = items.FirstOrDefault(x => x.Value.Equals(value));
        //    if (changeValue)
        //    {
        //        SelectedValue = value;
        //    }

        //    var currentItem = items.FirstOrDefault(x => x.Value.Equals(value));

        //    if (!MultiSelection)
        //    {
        //        RemoveSelectedCSS(items);

        //        var selectedItem = items.FirstOrDefault(x => x.Value.Equals(value));
        //        if (selectedItem != null)
        //            selectedItem.SetSelected(true);
        //    }
        //    else
        //    {
        //        if (SelectedValues.Contains(value))
        //        {
        //            currentItem?.SetSelected(false);
        //            SelectedItems = SelectedItems?.Where(x => !x.Equals(currentItem));
        //            SelectedValues = SelectedValues?.Where(x => x == null ? false : !x.Equals(value));
        //        }
        //        else
        //        {
        //            currentItem?.SetSelected(true);
        //            SelectedItems = SelectedItems.Append(currentItem);
        //            SelectedValues = SelectedValues.Append(value);
        //        }

        //        //RemoveSelectedCSS(items.Where(x => !x.IsSelected).ToList());

        //    }


        //    _lastActivatedItem = currentItem;
        //}

        internal void SetSelectedValue(MudListItem<T> item, bool force = false)
        {
            if ((!Clickable && !MultiSelection) && !force)
                return;

            //Make sure its the most parent one before continue method
            if (ParentList != null)
            {
                ParentList?.SetSelectedValue(item);
                return;
            }

            //create a list of all MudListItems to use for selecting the right item
            var items = CollectAllMudListItems();


            //SelectedItem = item;
            SelectedValue = item.Value;

            if (!MultiSelection)
            {
                SelectedValues = new List<T>() { SelectedValue };
            }
            else
            {
                if (item.IsSelected)
                {
                    //item.SetSelected(false);
                    //SelectedItems = SelectedItems?.Where(x => !x.Equals(item));
                    SelectedValues = SelectedValues?.Where(x => x == null ? false : !x.Equals(item.Value));
                }
                else
                {
                    //item.SetSelected(true);
                    //SelectedItems = SelectedItems.Append(item);
                    SelectedValues = SelectedValues.Append(item.Value);
                }
            }

            //UpdateSelectedStyles();
            _lastActivatedItem = item;
        }

        internal void UpdateSelectedStyles()
        {
            var items = CollectAllMudListItems();
            DeselectAllItems(items);

            if (!IsSelectable())
            {
                return;
            }

            if (!MultiSelection)
            {
                items.FirstOrDefault(x => SelectedValue?.ToString() == x.Value?.ToString())?.SetSelected(true);
            }
            else
            {
                items.Where(x => SelectedValues.Contains(x.Value)).ToList().ForEach(x => x.SetSelected(true));
            }

            StateHasChanged();
        }

        private bool IsSelectable()
        {
            if (Clickable || MultiSelection)
            {
                return true;
            }

            return false;
        }

        public void Clear()
        {
            var items = CollectAllMudListItems();
            if (!MultiSelection)
            {
                SelectedValue = default(T);
            }
            else
            {
                SelectedValues = null;
            }
            
            
            //SelectedItem = null;
            //SelectedItems = null;
            DeselectAllItems(items);
            DeactiveAllItems();
        }

        //internal bool CanSelect { get; private set; }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }
        
        private void DeselectAllItems(List<MudListItem<T>> items)
        {
            foreach (var listItem in items)
                listItem?.SetSelected(false);
        }

        private List<MudListItem<T>> CollectAllMudListItems()
        {
            var items = _items.ToList();
            foreach (var list in _childLists)
                items.AddRange(list._items);
            if (ParentList != null)
                items.AddRange(ParentList._items);
            return items;
        }

        internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || (Clickable == false && MultiSelection == false))
                return;
            var key = obj.Key.ToLowerInvariant();
            if (key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await ActiveFirstItem(key);
                return;
            }
            switch (obj.Key)
            {
                case "ArrowUp":
                    await ActiveAdjacentItem(-1);
                    break;
                case "ArrowDown":
                    await ActiveAdjacentItem(1);
                    break;
                case "Home":
                    await ActiveFirstItem();
                    break;
                case "End":
                    await ActiveLastItem();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (_lastActivatedItem == null)
                    {
                        return;
                    }
                    SetSelectedValue(_lastActivatedItem);
                    break;
                case "a":
                case "A":
                    if (obj.CtrlKey == true)
                    {
                        if (MultiSelection)
                        {
                            SelectAllItems(_allSelected);
                        }
                    }
                    break;
            }
            OnKeyDown.InvokeAsync(obj).AndForget();
        }

        private void OnFocusOut()
        {
            DeactiveAllItems();
        }

        protected void SelectAllItems(bool deselect = false)
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (deselect == true)
            {
                foreach (var item in items)
                {
                    item.SetSelected(false);
                }
                _allSelected = false;
            }
            else
            {
                foreach (var item in items)
                {
                    item.SetSelected(true);
                }
                _allSelected = true;
            }

            SelectedValues = items.Select(x => x.Value).ToHashSet();
        }

        public int GetActiveItemIndex()
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (_lastActivatedItem == null)
            {
                return items.FindIndex(x => x.IsActive == true);
            }
            else
            {
                return items.FindIndex(x => x == _lastActivatedItem);
            }
        }

        internal void UpdateLastActivatedItem(T value)
        {
            if (value == null)
            {
                _lastActivatedItem = null;
                return;
            }
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            _lastActivatedItem = items.FirstOrDefault(x => x.Value.ToString() == value.ToString());
        }

        private void DeactiveAllItems()
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            foreach (var item in items)
            {
                item.SetActive(false);
            }
        }

        public async Task ActiveFirstItem(string startChar = null)
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (items == null || items.Count == 0 || items[0].Disabled == true)
            {
                return;
            }
            DeactiveAllItems();

            if (string.IsNullOrWhiteSpace(startChar))
            {
                items[0].SetActive(true);
                _lastActivatedItem = items[0];
                await ScrollToMiddleAsync(items[0]);
                return;
            }

            // find first item that starts with the letter
            var possibleItems = items.Where(x => (bool)x.Value?.ToString().ToLowerInvariant().StartsWith(startChar)).ToList();
            if (possibleItems == null || !possibleItems.Any())
            {
                _lastActivatedItem.SetActive(true);
                await ScrollToMiddleAsync(_lastActivatedItem);
                return;
            }

            var theItem = possibleItems.FirstOrDefault(x => x == _lastActivatedItem);
            if (theItem == null)
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                await ScrollToMiddleAsync(possibleItems[0]);
                return;
            }

            if (theItem == possibleItems.LastOrDefault())
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                await ScrollToMiddleAsync(possibleItems[0]);
            }
            else
            {
                var item = possibleItems[possibleItems.IndexOf(theItem) + 1];
                item.SetActive(true);
                _lastActivatedItem = item;
                await ScrollToMiddleAsync(item);
            }
        }

        public async Task ActiveAdjacentItem(int changeCount)
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (items == null || items.Count == 0)
            {
                return;
            }
            int index = GetActiveItemIndex();
            if (index + changeCount >= items.Count || 0 > index + changeCount)
            {
                return;
            }
            if (items[index + changeCount].Disabled == true)
            {
                // Recursive
                await ActiveAdjacentItem(changeCount > 0 ? changeCount + 1 : changeCount - 1);
                return;
            }
            DeactiveAllItems();
            items[index + changeCount].SetActive(true);
            _lastActivatedItem = items[index + changeCount];

            if (items[index + changeCount].ParentListItem != null && items[index + changeCount].ParentListItem.Expanded == false)
            {
#pragma warning disable BL0005
                items[index + changeCount].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[index + changeCount]);
        }

        public async Task ActivePreviousItem()
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (items == null || items.Count == 0)
            {
                return;
            }
            int index = GetActiveItemIndex();
            if (0 > index - 1)
            {
                return;
            }
            DeactiveAllItems();
            items[index - 1].SetActive(true);
            _lastActivatedItem = items[index - 1];

            if (items[index - 1].ParentListItem != null && items[index - 1].ParentListItem.Expanded == false)
            {
                items[index - 1].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[index - 1]);
        }

        public async Task ActiveLastItem()
        {
            var items = CollectAllMudListItems().Where(x => x.NestedList == null).ToList();
            if (items == null || items.Count == 0 || items[items.Count - 1].Disabled == true)
            {
                return;
            }
            DeactiveAllItems();
            items[items.Count - 1].SetActive(true);
            _lastActivatedItem = items[items.Count - 1];

            await ScrollToMiddleAsync(items[items.Count - 1]);
        }
    }
}
