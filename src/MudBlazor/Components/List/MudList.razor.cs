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
        #region Parameters, Fields, Injected Services

        [Inject] private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        private IKeyInterceptor _keyInterceptor;
        private string _elementId = "list_" + Guid.NewGuid().ToString().Substring(0, 8);
        private bool _multiSelection = false;
        private List<MudListItem<T>> _items = new();
        private List<MudList<T>> _childLists = new();
        private MudListItem<T> _selectedItem = new();
        private HashSet<MudListItem<T>> _selectedItems = new();
        private T _selectedValue;
        private HashSet<T> _selectedValues = new();
        internal MudListItem<T> _lastActivatedItem;
        internal bool? _allSelected = false;

        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        protected string Stylename =>
        new StyleBuilder()
            .AddStyle("max-height", $"{MaxItems * (Dense == false ? 48 : 36) + (DisablePadding == true ? 0 : 16)}px", MaxItems != null)
            .AddStyle("overflow-y", "auto", MaxItems != null)
            .AddStyle(Style)
            .Build();

        [CascadingParameter] protected MudList<T> ParentList { get; set; }

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
        /// Predefined enumerable items. If its not null, creates list items automatically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public ICollection<T> Items { get; set; } = null;

        /// <summary>
        /// Allows virtualization. Only work is Items parameter is not null.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Virtualize { get; set; }

        /// <summary>
        /// Set max items to show in list. Other items can be scrolled. Works if list items populated with Items parameter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public int? MaxItems { get; set; } = null;

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
                if (ParentList != null)
                {
                    _multiSelection = ParentList.MultiSelection;
                    return;
                }
                if (_multiSelection == value)
                {
                    return;
                }
                _multiSelection = value;
                if (_multiSelection == false)
                {
                    if (!_centralCommanderIsProcessing)
                    {
                        HandleCentralValueCommander("MultiSelectionOff");
                    }
                    
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
        /// If true the active (hilighted) item select on tab key.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public bool SelectValueOnTab { get; set; } = true;

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, selected items doesn't have a selected background color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableSelectedBackground { get; set; }

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
        /// If set to true and the MultiSelection option is set to true, a "select all" checkbox is added at the top of the list of items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public bool SelectAll { get; set; }

        /// <summary>
        /// Define the text of the Select All option.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string SelectAllText { get; set; } = "Select all";

        /// <summary>
        /// If true, change background color to secondary for all nested item headers.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool SecondaryBackgroundForNestedItemHeader { get; set; }

        /// <summary>
        /// Fired on the KeyDown event.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        #endregion


        #region Values & Items

        bool _centralCommanderIsProcessing = false;
        bool _centralCommanderResultRendered = true;
        // CentralCommander has the simple aim: Prevent racing conditions. It has two mechanism to do this:
        // (1) When this method is running, it doesn't allow to run a second one. That guarantees to different value parameters can not call this method at the same time.
        // (2) When this method runs once, prevents all value setters until OnAfterRender runs. That guarantees to have proper values.
        protected void HandleCentralValueCommander(string changedValueType)
        {
            //Console.WriteLine("Central Value Started");
            if (_centralCommanderIsProcessing == true)
            {
                return;
            }
            _centralCommanderIsProcessing = true;

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
                SelectedValue = SelectedValues.FirstOrDefault();
                //var items = CollectAllMudListItems(true);
                //SelectedValues = new List<T>() { SelectedValue };
                //SelectedItems = items.Where(x => SelectedValues.Contains(x.Value)).ToList();
            }

            _centralCommanderResultRendered = false;
            _centralCommanderIsProcessing = false;
            //Console.WriteLine("Central Value ended");
        }

        private void UpdateSelectedValues(bool once = false)
        {
            SelectedValues = new List<T>() { SelectedValue };
        }

        internal void UpdateSelectedItem()
        {
            var items = CollectAllMudListItems(true);

            if (MultiSelection && (SelectedValues == null || SelectedValues.Count() == 0))
            {
                SelectedItem = null;
                SelectedItems = null;
                return;
            }

            SelectedItem = items.FirstOrDefault(x => x.Value?.ToString() == SelectedValue?.ToString());
            SelectedItems = items.Where(x => SelectedValues.Contains(x.Value)).ToList();
        }

        /// <summary>
        /// The current selected value.
        /// Note: Make the list Clickable or set MultiSelection true for item selection to work.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T SelectedValue
        {
            get => _selectedValue;
            set
            {
                //Console.WriteLine("SelectedValue setter Started");
                if (!_centralCommanderResultRendered)
                {
                    return;
                }
                if (ParentList != null)
                {
                    //Console.WriteLine("SelectedValue setter returned");
                    return;
                }
                if ((_selectedValue != null && value != null && _selectedValue.ToString() == value.ToString()) || (_selectedValue == null && value == null))
                {
                    //Console.WriteLine("SelectedValue setter returned");
                    return;
                }
                _selectedValue = value;

                HandleCentralValueCommander("SelectedValue");

                SelectedValueChanged.InvokeAsync(_selectedValue).AndForget();
                UpdateSelectedStyles();
                //Console.WriteLine("SelectedValue setter ended");
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
                //Console.WriteLine("SelectedValues setter Started");
                if (!_centralCommanderResultRendered)
                {
                    return;
                }
                if (ParentList != null)
                {
                    //Console.WriteLine("SelectedValues setter returned");
                    return;
                }
                var set = value ?? new List<T>();
                if ((_selectedValues != null && value != null && _selectedValues == value) || (_selectedValues == null && value == null))
                {
                    //Console.WriteLine("SelectedValues setter returned");
                    return;
                }

                if (SelectedValues.Count() == set.Count() && _selectedValues != null && _selectedValues.All(x => set.Contains(x)))
                {
                    //Console.WriteLine("SelectedValues setter returned");
                    return;
                }

                _selectedValues = value == null ? null : value.ToHashSet();

                HandleCentralValueCommander("SelectedValues");

                SelectedValuesChanged.InvokeAsync(_selectedValues).AndForget();
                UpdateSelectedStyles();
                //Console.WriteLine("SelectedValues setter ended");
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
                if (!_centralCommanderResultRendered)
                {
                    return;
                }
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            }
        }

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
                if (!_centralCommanderResultRendered)
                {
                    return;
                }
                if (_selectedItems == value)
                    return;

                _selectedItems = value == null ? null : value.ToHashSet();
                SelectedItemsChanged.InvokeAsync(_selectedItems).AndForget();
            }
        }

        /// <summary>
        /// Called whenever the selection changed. Can also be called even MultiSelection is true.
        /// </summary>
        [Parameter] public EventCallback<T> SelectedValueChanged { get; set; }

        /// <summary>
        /// Called whenever selected values changes. Can also be called even MultiSelection is false.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Called whenever the selected item changed. Can also be called even MultiSelection is true.
        /// </summary>
        [Parameter] public EventCallback<MudListItem<T>> SelectedItemChanged { get; set; }

        /// <summary>
        /// Called whenever the selected items changed. Can also be called even MultiSelection is false.
        /// </summary>
        [Parameter] public EventCallback<IEnumerable<MudListItem<T>>> SelectedItemsChanged { get; set; }

        public List<MudListItem<T>> GetAllItems
        {
            get => CollectAllMudListItems();
        }

        public List<MudListItem<T>> GetItems
        {
            get => CollectAllMudListItems(true);
        }

        #endregion


        #region Lifecycle Methods & Register

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
            }
            _centralCommanderResultRendered = true;
            //Console.WriteLine("Rendered");
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

        #endregion


        #region Events (Key, Focus)

        internal async Task HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || (Clickable == false && MultiSelection == false))
                return;
            if (ParentList != null)
            {
                //await ParentList.HandleKeyDown(obj);
                return;
            }
            var key = obj.Key.ToLowerInvariant();
            if (key.Length == 1 && key != " " && !(obj.CtrlKey || obj.ShiftKey || obj.AltKey || obj.MetaKey))
            {
                await ActiveFirstItem(key);
                return;
            }
            switch (obj.Key)
            {
                case "Tab":
                    if (!MultiSelection)
                    {
                        SetSelectedValue(_lastActivatedItem);
                    }
                    break;
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
            await OnKeyDown.InvokeAsync(obj);
        }

        private void OnFocusOut()
        {
            //if (ParentList != null)
            //{
            //    DeactiveAllItems();
            //}
            DeactiveAllItems();
        }

        #endregion


        #region Select

        internal void SetSelectedValue(T value, bool force = false)
        {
            if ((!Clickable && !MultiSelection) && !force)
                return;

            //Make sure its the most parent one before continue method
            if (ParentList != null)
            {
                ParentList?.SetSelectedValue(value);
                return;
            }

            if (!MultiSelection)
            {
                SelectedValue = value;
            }
            else
            {
                if (SelectedValues.Contains(value))
                {
                    SelectedValues = SelectedValues?.Where(x => x == null ? false : !x.Equals(value));
                }
                else
                {
                    SelectedValues = SelectedValues.Append(value);
                }
            }
            UpdateLastActivatedItem(value);
            //_lastActivatedItem = item;
        }

        internal void SetSelectedValue(MudListItem<T> item, bool force = false)
        {
            if (item == null)
            {
                return;
            }

            if ((!Clickable && !MultiSelection) && !force)
                return;

            //Make sure its the most parent one before continue method
            if (ParentList != null)
            {
                ParentList?.SetSelectedValue(item);
                return;
            }
            //create a list of all MudListItems to use for selecting the right item
            //var items = CollectAllMudListItems(true);


            //SelectedItem = item;


            if (!MultiSelection)
            {
                //SelectedValues = new List<T>() { SelectedValue };
                SelectedValue = item.Value;
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
            //UpdateLastActivatedItem(SelectedValue);
            UpdateSelectAllState();
            _lastActivatedItem = item;
        }

        //internal bool CanSelect { get; private set; }

        internal void UpdateSelectedStyles()
        {
            var items = CollectAllMudListItems(true);
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

        private void DeselectAllItems(List<MudListItem<T>> items)
        {
            foreach (var listItem in items)
                listItem?.SetSelected(false);
        }

        public List<MudListItem<T>> CollectAllMudListItems(bool exceptNestedAndExceptional = false)
        {
            var items = new List<MudListItem<T>>();
            
            if (ParentList != null)
            {
                items.AddRange(ParentList._items);
                foreach (var list in ParentList._childLists)
                    items.AddRange(list._items);
            }
            else
            {
                items.AddRange(_items);
                foreach (var list in _childLists)
                    items.AddRange(list._items);
            }

            if (exceptNestedAndExceptional == false)
            {
                return items;
            }
            else
            {
                return items.Where(x => x.NestedList == null && x.Exceptional == false).ToList();
            }
        }

        #endregion


        #region SelectAll

        protected internal void UpdateSelectAllState()
        {
            if (MultiSelection && SelectAll)
            {
                var oldState = _allSelected;
                if (_selectedValues == null || !_selectedValues.Any())
                {
                    _allSelected = false;
                }
                else if (CollectAllMudListItems(true).Count() == _selectedValues.Count)
                {
                    _allSelected = true;
                }
                else
                {
                    _allSelected = null;
                }
            }
        }

        protected string SelectAllCheckBoxIcon
        {
            get
            {
                return _allSelected.HasValue ? _allSelected.Value ? CheckedIcon : UncheckedIcon : IndeterminateIcon;
            }
        }

        /// <summary>
        /// Custom checked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string CheckedIcon { get; set; } = Icons.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string UncheckedIcon { get; set; } = Icons.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListAppearance)]
        public string IndeterminateIcon { get; set; } = Icons.Filled.IndeterminateCheckBox;



        protected void SelectAllItems(bool? deselect = false)
        {
            var items = CollectAllMudListItems(true);
            if (deselect == true)
            {
                foreach (var item in items)
                {
                    if (item.IsSelected == true)
                    {
                        item.SetSelected(false, false);
                    }
                }
                _allSelected = false;
            }
            else
            {
                foreach (var item in items)
                {
                    if (item.IsSelected == false)
                    {
                        item.SetSelected(true, false);
                    }
                }
                _allSelected = true;
            }

            SelectedValues = items.Where(x => x.IsSelected == true).Select(y => y.Value).ToHashSet();
            //StateHasChanged();
        }

        #endregion


        #region Active (Hilight)

        public int GetActiveItemIndex()
        {
            var items = CollectAllMudListItems(true);
            if (_lastActivatedItem == null)
            {
                var a = items.FindIndex(x => x.IsActive == true);
                return a;
            }
            else
            {
                var a = items.FindIndex(x => x.Value?.ToString() == _lastActivatedItem.Value.ToString());
                return a;
            }
        }

        public T GetActiveItemValue()
        {
            var items = CollectAllMudListItems(true);
            if (_lastActivatedItem == null)
            {
                return items.FirstOrDefault(x => x.IsActive == true).Value;
            }
            else
            {
                return _lastActivatedItem.Value;
            }
        }

        internal void UpdateLastActivatedItem(T value)
        {
            if (value == null)
            {
                _lastActivatedItem = null;
                return;
            }
            var items = CollectAllMudListItems(true);
            _lastActivatedItem = items.FirstOrDefault(x => x.Value?.ToString() == value.ToString());
        }

        private void DeactiveAllItems()
        {
            var items = CollectAllMudListItems(true);
            foreach (var item in items)
            {
                item.SetActive(false);
            }
        }

        public async Task ActiveFirstItem(string startChar = null)
        {
            var items = CollectAllMudListItems(true);
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
            var items = CollectAllMudListItems(true);
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
            var items = CollectAllMudListItems(true);
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
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0 || items[items.Count - 1].Disabled == true)
            {
                return;
            }
            DeactiveAllItems();
            items[items.Count - 1].SetActive(true);
            _lastActivatedItem = items[items.Count - 1];

            await ScrollToMiddleAsync(items[items.Count - 1]);
        }

        #endregion


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
            UpdateSelectAllState();
        }

        //private ValueTask ScrollToItemAsync(MudListItem<T> item)
        //    => item != null ? ScrollManager.ScrollToListItemAsync(item.ItemId) : ValueTask.CompletedTask;

        protected internal ValueTask ScrollToMiddleAsync(MudListItem<T> item)
            => ScrollManager.ScrollToMiddleAsync(_elementId, item.ItemId);

        //private void GetSelectedItem()
        //{
        //    var items = CollectAllMudListItems(true);
        //    if (!MultiSelection)
        //    {
        //        SelectedItem = items.FirstOrDefault(x => x.Value != null && x.Value.Equals(_selectedValue));
        //    }
        //    else
        //    {
        //        SelectedItems = items.Where(x => SelectedValues != null && SelectedValues.Contains(x.Value));
        //    }
        //}

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }
    }
}
