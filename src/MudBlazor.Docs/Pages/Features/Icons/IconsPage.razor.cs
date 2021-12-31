// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using MudBlazor.Docs.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor.Interop;

namespace MudBlazor.Docs.Pages.Features.Icons
{
    public partial class IconsPage
    {
        [Inject] IResizeObserver ResizeObserver { get; set; }
        [Inject] protected IJsApiService JsApiService { get; set; }

        bool iconDrawerOpen;
        List<MudIcons> DisplayedIcons;
        private IconOrigin SelectedIconOrigin { get; set; } = IconOrigin.Material;
        private string SearchText { get; set; } = string.Empty;
        private double _iconCardWidth = 136.88; // single icon card width includin margins
        private float _iconCardHeight = 144; // single icon card height includin margins
        private int CardsPerRow = 0;


        private ElementReference killZone;

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
            ? GetVirtualizedIcons(DisplayedIcons)
            : GetVirtualizedIcons(DisplayedIcons.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList());

        private List<MudVirtualizedIcons> GetVirtualizedIcons(List<MudIcons> iconlist)
        {
            if (CardsPerRow <= 0)
                return new List<MudVirtualizedIcons>();
            return iconlist.Chunk(CardsPerRow).Select(row => new MudVirtualizedIcons(row)).ToList();
        }

        private readonly IDictionary<string, object> IconTypes = new Dictionary<string, object>()
        {
            { IconType.Filled,new Filled()},
            { IconType.Outlined, new Outlined()},
            { IconType.Rounded, new Rounded()},
            { IconType.Sharp, new Sharp()},
            { IconType.TwoTone, new TwoTone()},
            { IconType.Brands, new Brands()},
            { IconType.FileFormats, new FileFormats()},
            { IconType.Uncategorized, new Uncategorized()}
        };

        protected override async Task OnInitializedAsync()
        {
            MaterialFilled = await LoadMaterialIcons(IconType.Filled);
            DisplayedIcons = MaterialFilled;

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

                await ResizeObserver.Observe(killZone);

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
            CardsPerRow = Convert.ToInt32(ResizeObserver.GetWidth(killZone) / _iconCardWidth);
        }

        public async Task<List<MudIcons>> LoadMaterialIcons(string type)
        {
            var result = new List<MudIcons>();
            var icons = IconTypes[type];

            foreach (var prop in icons.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                result.Add(new MudIcons(prop.Name, prop.GetValue(icons).ToString(), type));
            }

            await Task.WhenAll();

            return result;
        }

        public async Task LoadCustomIcons()
        {
            var brands = new Brands();

            foreach (var prop in typeof(Brands).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomBrands.Add(new MudIcons(prop.Name, prop.GetValue(brands).ToString(), IconType.Brands));
            }

            CustomAll.AddRange(CustomBrands);

            var fileFormats = new FileFormats();

            foreach (var prop in typeof(FileFormats).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomFileFormats.Add(new MudIcons(prop.Name, prop.GetValue(fileFormats).ToString(), IconType.FileFormats));
            }
                
            CustomAll.AddRange(CustomFileFormats);

            var uncategorized = new Uncategorized();

            foreach (var prop in typeof(Uncategorized).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                CustomUncategorized.Add(new MudIcons(prop.Name, prop.GetValue(uncategorized).ToString(), IconType.Uncategorized));
            }
                
            CustomAll.AddRange(CustomUncategorized);

            await Task.WhenAll();
        }

        public void ChangeIconCategory(string type)
        {
            switch (type)
            {
                case IconType.Filled:
                    DisplayedIcons = MaterialFilled;
                    break;
                case IconType.Outlined:
                    DisplayedIcons = MaterialOutlined;
                    break;
                case IconType.Rounded:
                    DisplayedIcons = MaterialRounded;
                    break;
                case IconType.Sharp:
                    DisplayedIcons = MaterialSharp;
                    break;
                case IconType.TwoTone:
                    DisplayedIcons = MaterialTwoTone;
                    break;
                case IconType.All:
                    DisplayedIcons = CustomAll;
                    break;
                case IconType.Brands:
                    DisplayedIcons = CustomBrands;
                    break;
                case IconType.FileFormats:
                    DisplayedIcons = CustomFileFormats;
                    break;
                case IconType.Uncategorized:
                    DisplayedIcons = CustomUncategorized;
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
            iconDrawerOpen = true;
            SelectedIcon = new MudIcons(icon.Name, icon.Code, icon.Category);
            IconCodeOutput = $"@Icons{(SelectedIconOrigin == IconOrigin.Material ? "" : ".Custom")}.{icon.Category}.{icon.Name}";
        }
        void CloseIconDrawer()
        {
            iconDrawerOpen = false;
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
