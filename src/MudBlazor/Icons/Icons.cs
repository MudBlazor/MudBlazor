using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public static partial class Icons
    {
        public static class Custom
        {
            public static readonly Brands Brands = new();
            public static readonly FileFormats FileFormats = new();
            public static readonly Uncategorized Uncategorized = new();
        }
        public static class Material
        {
            public static readonly Filled Filled = new();
            public static readonly Outlined Outlined = new();
            public static readonly Rounded Rounded = new();
            public static readonly Sharp Sharp = new();
            public static readonly TwoTone TwoTone = new();
        }

        public static readonly Filled Filled = new();
        public static readonly Outlined Outlined = new();
        public static readonly Rounded Rounded = new();
        public static readonly Sharp Sharp = new();
        public static readonly TwoTone TwoTone = new();
    }
}

