using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;
using static MudBlazor.CategoryTypes;

namespace MudBlazor
{
    public partial class MudList<T> : MudComponentBase, IDisposable
    {
        #region Parameters, Fields, Injected Services

        [Inject] IKeyInterceptorFactory KeyInterceptorFactory { get; set; }
        [Inject] IScrollManager ScrollManager { get; set; }

        // Fields used in more than one place (or protected and internal ones) are shown here.
        // Others are next to the relevant parameters. (Like _selectedValue)
        private string _elementId = "list_" + Guid.NewGuid().ToString().Substring(0, 8);
        private List<MudListItem<T>> _items = new();
        private List<MudList<T>> _childLists = new();
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

        [CascadingParameter] protected MudSelect<T> MudSelect { get; set; }
        [CascadingParameter] protected MudAutocomplete<T> MudAutocomplete { get; set; }
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
        /// Optional presentation template for items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItem<T>> ItemTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for selected items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItem<T>> ItemSelectedTemplate { get; set; }

        /// <summary>
        /// Optional presentation template for disabled items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.ListBehavior)]
        public RenderFragment<MudListItem<T>> ItemDisabledTemplate { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public DefaultConverter<T> Converter { get; set; } = new DefaultConverter<T>();

        private IEqualityComparer<T> _comparer;
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T> Comparer
        {
            get => _comparer;
            set
            {
                if (_comparer == value)
                    return;
                _comparer = value;
                // Apply comparer and refresh selected values
                if (_selectedValues == null)
                {
                    return;
                }
                _selectedValues = new HashSet<T>(_selectedValues, _comparer);
                SelectedValues = _selectedValues;
            }
        }

        /// <summary>
        /// Predefined enumerable items. If its not null, creates list items automatically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public ICollection<T> ItemCollection { get; set; } = null;

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

        private bool _multiSelection = false;
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
                if (_setParametersDone == false)
                {
                    return;
                }
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
        /// If true the active (hilighted) item select on tab key. Designed for only single selection. Default is true.
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
        public bool DisableSelectedItemStyle { get; set; }

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
        public string SelectAllText { get; set; } = "Select All";

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

        /// <summary>
        /// Fired on the OnFocusOut event.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnFocusOut { get; set; }

        #endregion


        #region Values & Items (Core: Be careful if you change something inside the region, it affects all logic and also Select and Autocomplete)

        bool _centralCommanderIsProcessing = false;
        bool _centralCommanderResultRendered = false;
        // CentralCommander has the simple aim: Prevent racing conditions. It has two mechanism to do this:
        // (1) When this method is running, it doesn't allow to run a second one. That guarantees to different value parameters can not call this method at the same time.
        // (2) When this method runs once, prevents all value setters until OnAfterRender runs. That guarantees to have proper values.
        protected void HandleCentralValueCommander(string changedValueType, bool updateStyles = true)
        {
            //Console.WriteLine("Central Value Started");
            if (_setParametersDone == false)
            {
                return;
            }
            if (_centralCommanderIsProcessing == true)
            {
                return;
            }
            _centralCommanderIsProcessing = true;

            if (changedValueType == nameof(SelectedValue))
            {
                if (MultiSelection == false)
                {
                    SelectedValues = new HashSet<T>(_comparer) { SelectedValue };
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == nameof(SelectedValues))
            {
                if (MultiSelection == true)
                {
                    SelectedValue = SelectedValues == null ? default(T) : SelectedValues.LastOrDefault();
                    UpdateSelectedItem();
                }
            }
            else if (changedValueType == nameof(SelectedItem))
            {
                if (MultiSelection == false)
                {
                    SelectedItems = new HashSet<MudListItem<T>>() { SelectedItem };
                    UpdateSelectedValue();
                }
            }
            else if (changedValueType == nameof(SelectedItems))
            {
                if (MultiSelection == true)
                {
                    SelectedItem = SelectedItems == null ? null : SelectedItems.LastOrDefault();
                    UpdateSelectedValue();
                }
            }
            else if (changedValueType == "MultiSelectionOff")
            {
                SelectedValue = SelectedValues.FirstOrDefault();
                var items = CollectAllMudListItems(true);
                SelectedValues = SelectedValue == null ? null : new HashSet<T>(_comparer) { SelectedValue };
                UpdateSelectedItem();
            }

            _centralCommanderResultRendered = false;
            _centralCommanderIsProcessing = false;
            //Console.WriteLine("Central Value ended");
            if (updateStyles)
            {
                UpdateSelectedStyles();
            }
        }

        protected internal void UpdateSelectedItem()
        {
            var items = CollectAllMudListItems(true);

            if (MultiSelection && (SelectedValues == null || SelectedValues.Count() == 0))
            {
                SelectedItem = null;
                SelectedItems = null;
                return;
            }

            SelectedItem = items.FirstOrDefault(x => SelectedValue == null ? x.Value == null : Comparer != null ? Comparer.Equals(x.Value, SelectedValue) : x.Value.Equals(SelectedValue));
            SelectedItems = SelectedValues == null ? null : items.Where(x => SelectedValues.Contains(x.Value, _comparer));
        }

        protected internal void UpdateSelectedValue()
        {
            if (MultiSelection == false && SelectedItem == null)
            {
                SelectedValue = default(T);
                SelectedValues = null;
                return;
            }

            SelectedValue = SelectedItem == null ? default(T) : SelectedItem.Value;
            SelectedValues = SelectedItems.Select(x => x.Value).ToHashSet(_comparer);
        }

        private T _selectedValue;
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
                if (Converter.Set(_selectedValue) != Converter.Set(default(T)) && _firstRendered == false)
                {
                    return;
                }
                if (_centralCommanderResultRendered == false && _firstRendered == true)
                {
                    return;
                }
                if (ParentList != null)
                {
                    return;
                }
                if ((_selectedValue != null && value != null && _selectedValue.Equals(value)) || (_selectedValue == null && value == null))
                {
                    return;
                }

                _selectedValue = value;
                HandleCentralValueCommander(nameof(SelectedValue));
                SelectedValueChanged.InvokeAsync(_selectedValue).AndForget();
                //UpdateSelectedStyles();
                //Console.WriteLine("SelectedValue setter ended");
            }
        }

        private HashSet<T> _selectedValues;
        /// <summary>
        /// The current selected values. Holds single value (SelectedValue) if MultiSelection is false.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public IEnumerable<T> SelectedValues
        {
            get
            {
                return _selectedValues;
            }

            set
            {
                if (value == null && _firstRendered == false)
                {
                    return;
                }
                if (_centralCommanderResultRendered == false && _firstRendered == true)
                {
                    return;
                }
                if (ParentList != null)
                {
                    return;
                }
                //var set = value ?? new List<T>();
                if (value == null && _selectedValues == null)
                {
                    return;
                }

                if (value != null && _selectedValues != null && _selectedValues.SetEquals(value))
                {
                    return;
                }

                //if (SelectedValues.Count() == set.Count() && _selectedValues != null && _selectedValues.All(x => set.Contains(x)))
                //{
                //    return;
                //}

                _selectedValues = value == null ? null : new HashSet<T>(value, _comparer);
                if (_setParametersDone == false)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedValues));
                SelectedValuesChanged.InvokeAsync(SelectedValues == null ? null : new HashSet<T>(SelectedValues, _comparer)).AndForget();
                //UpdateSelectedStyles();
                //Console.WriteLine("SelectedValues setter ended");
            }
        }

        private MudListItem<T> _selectedItem;
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
                if (_centralCommanderResultRendered == false && _firstRendered == true)
                {
                    return;
                }
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                if (_setParametersDone == false)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedItem));
                //if (_firstRendered == true)
                //{
                //    SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
                //}
                SelectedItemChanged.InvokeAsync(_selectedItem).AndForget();
            }
        }

        private HashSet<MudListItem<T>> _selectedItems;
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
                if (_centralCommanderResultRendered == false && _firstRendered == true)
                {
                    return;
                }

                if (value == null && _selectedItems == null)
                {
                    return;
                }

                if (value != null && _selectedItems != null && _selectedItems.SetEquals(value))
                    return;

                _selectedItems = value == null ? null : value.ToHashSet();
                if (_setParametersDone == false)
                {
                    return;
                }
                HandleCentralValueCommander(nameof(SelectedItems));
                //if (_firstRendered == true)
                //{
                //    SelectedItemsChanged.InvokeAsync(_selectedItems).AndForget();
                //}
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

        /// <summary>
        /// Get all MudListItems in the list.
        /// </summary>
        public List<MudListItem<T>> GetAllItems
        {
            get => CollectAllMudListItems();
        }

        /// <summary>
        /// Get all items that holds value.
        /// </summary>
        public List<MudListItem<T>> GetItems
        {
            get => CollectAllMudListItems(true);
        }

        #endregion


        #region Lifecycle Methods, Dispose & Register

        bool _setParametersDone = false;
        public override Task SetParametersAsync(ParameterView parameters)
        {
            if (_centralCommanderIsProcessing == true)
            {
                return Task.CompletedTask;
            }

            if (MudSelect != null)
            {
                return Task.CompletedTask;
            }

            //if (parameters.TryGetValue<T>(nameof(SelectedValue), out var value))
            //{
            //    if (value != null)
            //    {
            //        _selectedValue = value;
            //    }
            //}
            base.SetParametersAsync(parameters).AndForget();
            //if (parameters.Contains<T>("SelectedValue"))
            //{
            //    _selectedValue = parameters.GetValueOrDefault("SelectedValue", default(T));
            //}
            _setParametersDone = true;
            return Task.CompletedTask;
        }

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

        private IKeyInterceptor _keyInterceptor;
        private bool _firstRendered = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                _firstRendered = false;
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

                if (MudSelect == null && MudAutocomplete == null)
                {
                    if (MultiSelection == false && SelectedValue != null)
                    {
                        HandleCentralValueCommander(nameof(SelectedValue));
                    }
                    else if (MultiSelection == true && SelectedValues != null)
                    {
                        HandleCentralValueCommander(nameof(SelectedValues));
                    }
                }

                if (MudSelect != null || MudAutocomplete != null)
                {
                    if (MultiSelection)
                    {
                        UpdateSelectAllState();
                        //UpdateSelectedStyles();
                        if (MudSelect != null)
                        {
                            SelectedValues = MudSelect.SelectedValues;
                        }
                        else if (MudAutocomplete != null)
                        {
                            SelectedValues = MudAutocomplete.SelectedValues;
                        }
                        HandleCentralValueCommander(nameof(SelectedValues));
                    }
                    else
                    {
                        //UpdateSelectedStyles();
                        UpdateLastActivatedItem(SelectedValue);
                    }
                }
                if (SelectedValues != null)
                {
                    UpdateLastActivatedItem(SelectedValues.LastOrDefault());
                }
                if (_lastActivatedItem != null && !(MultiSelection && _allSelected == true))
                {
                    await ScrollToMiddleAsync(_lastActivatedItem);
                }
                _firstRendered = true;
            }
            _centralCommanderResultRendered = true;
            Console.WriteLine("List Rendered");
            //if (MudAutocomplete != null)
            //{
            //    UpdateSelectedStyles();
            //}

        }

        public void Dispose()
        {
            ParametersChanged = null;
            ParentList?.Unregister(this);
        }

        protected internal void Register(MudListItem<T> item)
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

        protected internal void Unregister(MudListItem<T> item)
        {
            _items.Remove(item);
        }

        protected internal void Register(MudList<T> child)
        {
            _childLists.Add(child);
        }

        protected internal void Unregister(MudList<T> child)
        {
            _childLists.Remove(child);
        }

        #endregion


        #region Events (Key, Focus)

        protected internal async Task HandleKeyDown(KeyboardEventArgs obj)
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
                    if (!MultiSelection && SelectValueOnTab)
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
                            //Console.WriteLine("List Select All started");
                        }
                    }
                    break;
            }
            await OnKeyDown.InvokeAsync(obj);
        }

        protected async Task HandleOnFocusOut()
        {
            //if (ParentList != null)
            //{
            //    DeactiveAllItems();
            //}
            DeactiveAllItems();
            await OnFocusOut.InvokeAsync();
        }

        #endregion


        #region Select

        protected internal void SetSelectedValue(T value, bool force = false)
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
                if (SelectedValues.Contains(value, _comparer))
                {
                    SelectedValues = SelectedValues?.Where(x => Comparer != null ? !Comparer.Equals(x, value) : !x.Equals(value)).ToHashSet(_comparer);
                }
                else
                {
                    SelectedValues = SelectedValues.Append(value).ToHashSet(_comparer);
                }
            }
            UpdateLastActivatedItem(value);
        }

        protected internal void SetSelectedValue(MudListItem<T> item, bool force = false)
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

            if (MultiSelection == false)
            {
                SelectedValue = item.Value;
            }
            else
            {
                if (item.IsSelected)
                {
                    SelectedValues = SelectedValues?.Where(x => Comparer != null ? !Comparer.Equals(x, item.Value) : !x.Equals(item.Value));
                }
                else
                {
                    if (SelectedValues == null)
                    {
                        SelectedValues = new HashSet<T>(_comparer) { item.Value };
                    }
                    else
                    {
                        SelectedValues = SelectedValues.Append(item.Value).ToHashSet(_comparer);
                    }
                }
            }

            UpdateSelectAllState();
            _lastActivatedItem = item;
        }

        protected internal void UpdateSelectedStyles(bool deselectFirst = true)
        {
            var items = CollectAllMudListItems(true);
            if (deselectFirst)
            {
                DeselectAllItems(items);
            }

            if (IsSelectable() == false)
            {
                return;
            }

            if (MultiSelection == false)
            {
                items.FirstOrDefault(x => SelectedValue == null ? x.Value == null : SelectedValue.Equals(x == null ? null : x.Value))?.SetSelected(true);
            }
            else if (SelectedValues != null)
            {
                items.Where(x => SelectedValues.Contains(x.Value, Comparer == null ? null : Comparer)).ToList().ForEach(x => x.SetSelected(true));
            }

            StateHasChanged();
        }

        protected bool IsSelectable()
        {
            if (Clickable || MultiSelection)
            {
                return true;
            }

            return false;
        }

        protected void DeselectAllItems(List<MudListItem<T>> items)
        {
            foreach (var listItem in items)
                listItem?.SetSelected(false);
        }

        protected List<MudListItem<T>> CollectAllMudListItems(bool exceptNestedAndExceptional = false)
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
                return items.Where(x => x.NestedList == null && x.IsFunctional == false).ToList();
            }
        }

        #endregion


        #region SelectAll

        protected internal void UpdateSelectAllState()
        {
            if (MultiSelection)
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
                        item.SetSelected(false);
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
                        item.SetSelected(true);
                    }
                }
                _allSelected = true;
            }

            SelectedValues = items.Where(x => x.IsSelected == true).Select(y => y.Value).ToHashSet(_comparer);
            if (MudSelect != null)
            {
                MudSelect.BeginValidate();
            }
            //StateHasChanged();
        }

        #endregion


        #region Active (Hilight)

        protected int GetActiveItemIndex()
        {
            var items = CollectAllMudListItems(true);
            if (_lastActivatedItem == null)
            {
                var a = items.FindIndex(x => x.IsActive == true);
                return a;
            }
            else
            {
                var a = items.FindIndex(x => _lastActivatedItem.Value == null ? x.Value == null : Comparer != null ? Comparer.Equals(_lastActivatedItem.Value, x.Value) : _lastActivatedItem.Value.Equals(x.Value));
                return a;
            }
        }

        protected T GetActiveItemValue()
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

        protected internal void UpdateLastActivatedItem(T value)
        {
            //if (value == null)
            //{
            //    _lastActivatedItem = null;
            //    return;
            //}
            var items = CollectAllMudListItems(true);
            _lastActivatedItem = items.FirstOrDefault(x => value == null ? x.Value == null : Comparer != null ? Comparer.Equals(value, x.Value) : value.Equals(x.Value));
        }

        protected void DeactiveAllItems(List<MudListItem<T>> items = null)
        {
            if (items == null)
            {
                items = CollectAllMudListItems(true);
            }
            
            foreach (var item in items)
            {
                item.SetActive(false);
            }
        }

#pragma warning disable BL0005
        public async Task ActiveFirstItem(string startChar = null)
        {
            var items = CollectAllMudListItems(true);
            if (items == null || items.Count == 0 || items[0].Disabled == true)
            {
                return;
            }
            DeactiveAllItems(items);

            if (string.IsNullOrWhiteSpace(startChar))
            {
                items[0].SetActive(true);
                _lastActivatedItem = items[0];
                if (items[0].ParentListItem != null && items[0].ParentListItem.Expanded == false)
                {
                    items[0].ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(items[0]);
                return;
            }

            // find first item that starts with the letter
            var possibleItems = items.Where(x => (x.Text ?? Converter.Set(x.Value) ?? "").StartsWith(startChar, StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (possibleItems == null || !possibleItems.Any())
            {
                if (_lastActivatedItem == null)
                {
                    return;
                }
                _lastActivatedItem.SetActive(true);
                if (_lastActivatedItem.ParentListItem != null && _lastActivatedItem.ParentListItem.Expanded == false)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(_lastActivatedItem);
                return;
            }

            var theItem = possibleItems.FirstOrDefault(x => x == _lastActivatedItem);
            if (theItem == null)
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                if (_lastActivatedItem.ParentListItem != null && _lastActivatedItem.ParentListItem.Expanded == false)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(possibleItems[0]);
                return;
            }

            if (theItem == possibleItems.LastOrDefault())
            {
                possibleItems[0].SetActive(true);
                _lastActivatedItem = possibleItems[0];
                if (_lastActivatedItem.ParentListItem != null && _lastActivatedItem.ParentListItem.Expanded == false)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
                await ScrollToMiddleAsync(possibleItems[0]);
            }
            else
            {
                var item = possibleItems[possibleItems.IndexOf(theItem) + 1];
                item.SetActive(true);
                _lastActivatedItem = item;
                if (_lastActivatedItem.ParentListItem != null && _lastActivatedItem.ParentListItem.Expanded == false)
                {
                    _lastActivatedItem.ParentListItem.Expanded = true;
                }
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
            DeactiveAllItems(items);
            items[index + changeCount].SetActive(true);
            _lastActivatedItem = items[index + changeCount];

            if (items[index + changeCount].ParentListItem != null && items[index + changeCount].ParentListItem.Expanded == false)
            {
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
            DeactiveAllItems(items);
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
            if (items == null || items.Count == 0)
            {
                return;
            }
            var properLastIndex = items.Count - 1;
            DeactiveAllItems(items);
            for (int i = 0; i < items.Count; i++)
            {
                if (items[properLastIndex - i].Disabled != true)
                {
                    properLastIndex -= i;
                    break;
                }
            }
            items[properLastIndex].SetActive(true);
            _lastActivatedItem = items[properLastIndex];

            if (items[properLastIndex].ParentListItem != null && items[properLastIndex].ParentListItem.Expanded == false)
            {
                items[properLastIndex].ParentListItem.Expanded = true;
            }

            await ScrollToMiddleAsync(items[properLastIndex]);
        }
#pragma warning restore BL0005

        #endregion


        #region Others (Clear, Scroll)

        /// <summary>
        /// Clears value(s) and item(s) and deactive all items.
        /// </summary>
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

        protected internal ValueTask ScrollToMiddleAsync(MudListItem<T> item)
            => ScrollManager.ScrollToMiddleAsync(_elementId, item.ItemId);

        #endregion
    }
}
