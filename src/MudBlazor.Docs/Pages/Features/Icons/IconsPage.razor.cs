// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;
using MudBlazor.Interop;
using MudBlazor.Services;

namespace MudBlazor.Docs.Pages.Features.Icons
{
    public partial class IconsPage
    {
        [Inject] IResizeObserver ResizeObserver { get; set; }
        [Inject] protected IJsApiService JsApiService { get; set; }

        bool _iconDrawerOpen;
        List<MudIcons> _displayedIcons;
        private IconOrigin SelectedIconOrigin { get; set; } = IconOrigin.Material;
        private string SearchText { get; set; } = string.Empty;
        private double _iconCardWidth = 136.88; // single icon card width including margins
        private float _iconCardHeight = 144; // single icon card height including margins
        private int _cardsPerRow = 0;


        private ElementReference _killZone;

        private List<MudIcons> CustomAll { get; set; } = new List<MudIcons>();
        private List<MudIcons> CustomBrands { get; set; } = new List<MudIcons>();
        private List<MudIcons> CustomFileFormats { get; set; } = new List<MudIcons>();
        private List<MudIcons> CustomUncategorized { get; set; } = new List<MudIcons>();

        private List<MudIcons> MaterialFilled { get; set; }
        private List<MudIcons> MaterialOutlined { get; set; }
        private List<MudIcons> MaterialRounded { get; set; }
        private List<MudIcons> MaterialSharp { get; set; }
        private List<MudIcons> MaterialTwoTone { get; set; }

        private MudIcons SelectedIcon { get; set; } = new MudIcons("", "", "");
        private string IconCodeOutput { get; set; }
        private Size PreviewIconSize { get; set; } = Size.Medium;
        private Color PreviewIconColor { get; set; } = Color.Dark;
        private List<MudVirtualizedIcons> SelectedIcons => string.IsNullOrWhiteSpace(SearchText)
            ? GetVirtualizedIcons(_displayedIcons)
            : GetVirtualizedIcons(_displayedIcons.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList());

        private List<MudVirtualizedIcons> GetVirtualizedIcons(List<MudIcons> iconlist)
        {
            if (_cardsPerRow <= 0)
                return new List<MudVirtualizedIcons>();
            return iconlist.Chunk(_cardsPerRow).Select(row => new MudVirtualizedIcons(row)).ToList();
        }

        private readonly IconStorage _iconTypes = new()
        {
            { IconType.Filled, typeof(MudBlazor.Icons.Material.Filled) },
            { IconType.Outlined, typeof(MudBlazor.Icons.Material.Outlined) },
            { IconType.Rounded, typeof(MudBlazor.Icons.Material.Rounded) },
            { IconType.Sharp, typeof(MudBlazor.Icons.Material.Sharp) },
            { IconType.TwoTone, typeof(MudBlazor.Icons.Material.TwoTone) },
            { IconType.Brands, typeof(MudBlazor.Icons.Custom.Brands) },
            { IconType.FileFormats, typeof(MudBlazor.Icons.Custom.FileFormats) },
            { IconType.Uncategorized, typeof(MudBlazor.Icons.Custom.Uncategorized) }
        };

        protected override async Task OnInitializedAsync()
        {
            MaterialFilled = await LoadMaterialIcons(IconType.Filled);
            _displayedIcons = MaterialFilled;

            MaterialOutlined = await LoadMaterialIcons(IconType.Outlined);
            MaterialRounded = await LoadMaterialIcons(IconType.Rounded);
            MaterialSharp = await LoadMaterialIcons(IconType.Sharp);
            MaterialTwoTone = await LoadMaterialIcons(IconType.TwoTone);

            await LoadCustomIcons();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                await ResizeObserver.Observe(_killZone);

                ResizeObserver.OnResized += OnResized;

                SetCardsPerRow();
                StateHasChanged();
            }
        }

        private async void OnResized(IDictionary<ElementReference, BoundingClientRect> changes)
        {
            SetCardsPerRow();
            await InvokeAsync(StateHasChanged);
        }

        private void SetCardsPerRow()
        {
            _cardsPerRow = Convert.ToInt32(ResizeObserver.GetWidth(_killZone) / _iconCardWidth);
        }

        public async Task<List<MudIcons>> LoadMaterialIcons(string type)
        {
            var result = new List<MudIcons>();
            var icons = _iconTypes[type];
            var iconsInstance = Activator.CreateInstance(icons);

            foreach (var prop in icons.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                result.Add(new MudIcons(prop.Name, prop.GetValue(iconsInstance).ToString(), type));
            }
            foreach (var prop in icons.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                result.Add(new MudIcons(prop.Name, prop.GetRawConstantValue().ToString(), type));
            }

            await Task.WhenAll();

            return result;
        }

        public async Task LoadCustomIcons()
        {
            var brands = new MudBlazor.Icons.Custom.Brands();

            foreach (var prop in typeof(MudBlazor.Icons.Custom.Brands).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomBrands.Add(new MudIcons(prop.Name, prop.GetValue(brands).ToString(), IconType.Brands));
            }
            foreach (var prop in typeof(MudBlazor.Icons.Custom.Brands).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                CustomBrands.Add(new MudIcons(prop.Name, prop.GetRawConstantValue().ToString(), IconType.Brands));
            }

            CustomAll.AddRange(CustomBrands);

            var fileFormats = new MudBlazor.Icons.Custom.FileFormats();

            foreach (var prop in typeof(MudBlazor.Icons.Custom.FileFormats).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomFileFormats.Add(new MudIcons(prop.Name, prop.GetValue(fileFormats).ToString(), IconType.FileFormats));
            }
            foreach (var prop in typeof(MudBlazor.Icons.Custom.FileFormats).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                CustomFileFormats.Add(new MudIcons(prop.Name, prop.GetRawConstantValue().ToString(), IconType.FileFormats));
            }

            CustomAll.AddRange(CustomFileFormats);

            var uncategorized = new MudBlazor.Icons.Custom.Uncategorized();

            foreach (var prop in typeof(MudBlazor.Icons.Custom.Uncategorized).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomUncategorized.Add(new MudIcons(prop.Name, prop.GetValue(uncategorized).ToString(), IconType.Uncategorized));
            }
            foreach (var prop in typeof(MudBlazor.Icons.Custom.Uncategorized).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                CustomUncategorized.Add(new MudIcons(prop.Name, prop.GetRawConstantValue().ToString(), IconType.Uncategorized));
            }


            CustomAll.AddRange(CustomUncategorized);

            await Task.WhenAll();
        }

        public void ChangeIconCategory(string type)
        {
            switch (type)
            {
                case IconType.Filled:
                    _displayedIcons = MaterialFilled;
                    break;
                case IconType.Outlined:
                    _displayedIcons = MaterialOutlined;
                    break;
                case IconType.Rounded:
                    _displayedIcons = MaterialRounded;
                    break;
                case IconType.Sharp:
                    _displayedIcons = MaterialSharp;
                    break;
                case IconType.TwoTone:
                    _displayedIcons = MaterialTwoTone;
                    break;
                case IconType.All:
                    _displayedIcons = CustomAll;
                    break;
                case IconType.Brands:
                    _displayedIcons = CustomBrands;
                    break;
                case IconType.FileFormats:
                    _displayedIcons = CustomFileFormats;
                    break;
                case IconType.Uncategorized:
                    _displayedIcons = CustomUncategorized;
                    break;
            };
        }

        private void OnSelectedValue(IconOrigin origin)
        {
            switch (origin)
            {
                case IconOrigin.Material:
                    ChangeIconCategory(IconType.Filled);
                    break;
                case IconOrigin.Custom:
                    ChangeIconCategory(IconType.All);
                    break;
            }
            SelectedIconOrigin = origin;
        }

        void SetIconDrawer(MudIcons icon)
        {
            _iconDrawerOpen = true;
            SelectedIcon = new MudIcons(icon.Name, icon.Code, icon.Category);
            IconCodeOutput = $"@Icons{(SelectedIconOrigin == IconOrigin.Material ? ".Material" : ".Custom")}.{icon.Category}.{icon.Name}";
        }
        void CloseIconDrawer()
        {
            _iconDrawerOpen = false;
        }

        private async Task CopyTextToClipboard()
        {
            await JsApiService.CopyToClipboardAsync(IconCodeOutput);
        }

        private struct IconType
        {
            public const string Filled = "Filled", Outlined = "Outlined", Rounded = "Rounded", Sharp = "Sharp", TwoTone = "TwoTone", All = "All", Brands = "Brands", FileFormats = "FileFormats", Uncategorized = "Uncategorized";
        }
        private enum IconOrigin
        {
            Custom,
            Material
        }

        private string GetKillZoneStyle(bool debugg)
        {
            if (debugg)
            {
                return $"height:65vh;width:100%;position:sticky;top:0px;border-color:#ff0000;border-style:dashed;border-width:4px;border-radius:8px;";
            }
            else
            {
                return $"height:65vh;width:100%;position:sticky;top:0px;";
            }
        }
    }
}
