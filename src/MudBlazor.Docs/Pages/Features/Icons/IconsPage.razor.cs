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
#nullable enable
    public partial class IconsPage
    {
        private int _cardsPerRow;
        private bool _iconDrawerOpen;
        private ElementReference _killZone;
        private List<MudIcons> _displayedIcons = new();
        private const double IconCardWidth = 136.88; // single icon card width including margins
        private const float IconCardHeight = 144; // single icon card height including margins

        [Inject]
        protected IResizeObserver ResizeObserver { get; set; } = null!;

        [Inject]
        protected IJsApiService JsApiService { get; set; } = null!;

        private List<MudIcons> CustomAll { get; } = new();

        private List<MudIcons> CustomBrands { get; } = new();

        private List<MudIcons> CustomFileFormats { get; } = new();

        private List<MudIcons> CustomUncategorized { get; } = new();

        private List<MudIcons> MaterialFilled { get; set; } = new();

        private List<MudIcons> MaterialOutlined { get; set; } = new();

        private List<MudIcons> MaterialRounded { get; set; } = new();

        private List<MudIcons> MaterialSharp { get; set; } = new();

        private List<MudIcons> MaterialTwoTone { get; set; } = new();

        private MudIcons SelectedIcon { get; set; } = MudIcons.Empty;

        private Size PreviewIconSize { get; set; } = Size.Medium;

        private Color PreviewIconColor { get; set; } = Color.Dark;

        private IconOrigin SelectedIconOrigin { get; set; } = IconOrigin.Material;

        private string IconCodeOutput { get; set; } = string.Empty;

        private string SearchText { get; set; } = string.Empty;

        private List<MudVirtualizedIcons> SelectedIcons => string.IsNullOrWhiteSpace(SearchText)
            ? GetVirtualizedIcons(_displayedIcons)
            : GetVirtualizedIcons(_displayedIcons.Where(mudIcon => mudIcon.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList());

        private List<MudVirtualizedIcons> GetVirtualizedIcons(List<MudIcons> iconList)
        {
            if (_cardsPerRow <= 0)
            {
                return new List<MudVirtualizedIcons>();
            }

            return iconList.Chunk(_cardsPerRow).Select(row => new MudVirtualizedIcons(row)).ToList();
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
            _displayedIcons = MaterialFilled = await LoadMaterialIcons(IconType.Filled);
            MaterialOutlined = await LoadMaterialIcons(IconType.Outlined);
            MaterialRounded = await LoadMaterialIcons(IconType.Rounded);
            MaterialSharp = await LoadMaterialIcons(IconType.Sharp);
            MaterialTwoTone = await LoadMaterialIcons(IconType.TwoTone);

            await LoadCustomIcons();
            await base.OnInitializedAsync();
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

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void OnResized(IDictionary<ElementReference, BoundingClientRect> changes)
        {
            SetCardsPerRow();
            await InvokeAsync(StateHasChanged);
        }

        private void SetCardsPerRow() => _cardsPerRow = Convert.ToInt32(ResizeObserver.GetWidth(_killZone) / IconCardWidth);

        private async Task<List<MudIcons>> LoadMaterialIcons(string type)
        {
            var iconType = _iconTypes[type];
            var result = GetMudIconsByTypeCategory(iconType, type);

            await Task.WhenAll();

            return result;
        }

        private async Task LoadCustomIcons()
        {
            CustomBrands.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.Brands), IconType.Brands));
            CustomAll.AddRange(CustomBrands);

            CustomFileFormats.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.FileFormats), IconType.FileFormats));
            CustomAll.AddRange(CustomFileFormats);

            CustomUncategorized.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.Uncategorized), IconType.Uncategorized));
            CustomAll.AddRange(CustomUncategorized);

            await Task.WhenAll();
        }

        private List<MudIcons> GetMudIconsByTypeCategory(Type iconType, string category)
        {
            return iconType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Select(prop => new MudIcons(prop.Name, GetIconCodeOrDefault(prop), category))
                .ToList();
        }

        private string GetIconCodeOrDefault(FieldInfo fieldInfo) => fieldInfo.GetRawConstantValue()?.ToString() ?? string.Empty;

        private void ChangeIconCategory(string type)
        {
            _displayedIcons = type switch
            {
                IconType.Filled => MaterialFilled,
                IconType.Outlined => MaterialOutlined,
                IconType.Rounded => MaterialRounded,
                IconType.Sharp => MaterialSharp,
                IconType.TwoTone => MaterialTwoTone,
                IconType.All => CustomAll,
                IconType.Brands => CustomBrands,
                IconType.FileFormats => CustomFileFormats,
                IconType.Uncategorized => CustomUncategorized,
                _ => _displayedIcons
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

        private void SetIconDrawer(MudIcons icon)
        {
            _iconDrawerOpen = true;
            SelectedIcon = new MudIcons(icon.Name, icon.Code, icon.Category);
            IconCodeOutput = $"@Icons{(SelectedIconOrigin == IconOrigin.Material ? ".Material" : ".Custom")}.{icon.Category}.{icon.Name}";
        }

        private void CloseIconDrawer() => _iconDrawerOpen = false;

        private async Task CopyTextToClipboard() => await JsApiService.CopyToClipboardAsync(IconCodeOutput);

        private string GetKillZoneStyle() => "height:65vh;width:100%;position:sticky;top:0px;";

        private struct IconType
        {
            public const string Filled = "Filled";
            public const string Outlined = "Outlined";
            public const string Rounded = "Rounded";
            public const string Sharp = "Sharp";
            public const string TwoTone = "TwoTone";
            public const string All = "All";
            public const string Brands = "Brands";
            public const string FileFormats = "FileFormats";
            public const string Uncategorized = "Uncategorized";
        }
        private enum IconOrigin
        {
            Custom,
            Material
        }
    }
}
