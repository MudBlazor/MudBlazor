using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Pages.Features.Icons
{
    public partial class IconsPage : ComponentBase
    {
        public List<MudIcons> MudIconsMaterial;
        public List<MudIcons> MudIconsCustom;

        private string SelectedIconType { get; set; } = IconType.Filled;
        private string SearchText { get; set; } = string.Empty;
        private List<MudIcons> SelectedIcons => string.IsNullOrWhiteSpace(SearchText)
            ? MudIconsMaterial
            : MudIconsMaterial.Where(m => m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        private readonly IDictionary<string, Type> IconTypes = new Dictionary<string, Type>()
        {
            { IconType.Filled, typeof(MudBlazor.Icons.Material.Filled)},
            { IconType.Outlined, typeof(MudBlazor.Icons.Material.Outlined)},
            { IconType.Rounded, typeof(MudBlazor.Icons.Material.Rounded)},
            { IconType.Sharp, typeof(MudBlazor.Icons.Material.Sharp)},
            { IconType.TwoTone, typeof(MudBlazor.Icons.Material.TwoTone)},
        };

        protected override void OnInitialized()
        {
            LoadMaterialIcons(SelectedIconType);
            LoadCustomIcons();
        }

        public void LoadMaterialIcons(string selectedIcoType)
        {
            MudIconsMaterial = GetIconsFromType(IconTypes.Where(x => x.Key == selectedIcoType).FirstOrDefault().Value);
        }

        public void LoadCustomIcons()
        {
            var result = new List<MudIcons>();
            result.AddRange(GetIconsFromType(typeof(MudBlazor.Icons.Custom.Brands)));
            result.AddRange(GetIconsFromType(typeof(MudBlazor.Icons.Custom.FileFormats)));
            result.AddRange(GetIconsFromType(typeof(MudBlazor.Icons.Custom.Uncategorized)));
            MudIconsCustom = result;
        }

        public void HandleSelectedOptionChanged(string e)
        {
            SelectedIconType = e;
            LoadMaterialIcons(SelectedIconType);
        }

        private struct IconType
        {
            public const string Filled = "Filled",
                                Outlined = "Outlined",
                                Rounded = "Rounded",
                                Sharp = "Sharp",
                                TwoTone = "TwoTone";
        }

        public static List<MudIcons> GetIconsFromType(Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => new MudIcons(x.Name, (string)x.GetRawConstantValue()))
                .ToList();
        }
    }
}
