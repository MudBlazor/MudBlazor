using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    [ExcludeFromCodeCoverage]
    public static partial class Icons
    {
        public static class Custom
        {
            public static readonly Brands Brands = new Brands();
            public static readonly FileFormats FileFormats = new FileFormats();
            public static readonly Uncategorized Uncategorized = new Uncategorized();
        }
        public static class Material
        {
            public static readonly Filled Filled = new Filled();
            public static readonly Outlined Outlined = new Outlined();
            public static readonly Rounded Rounded = new Rounded();
            public static readonly Sharp Sharp = new Sharp();
            public static readonly TwoTone TwoTone = new TwoTone();
        }

        public static readonly Filled Filled = new Filled();
        public static readonly Outlined Outlined = new Outlined();
        public static readonly Rounded Rounded = new Rounded();
        public static readonly Sharp Sharp = new Sharp();
        public static readonly TwoTone TwoTone = new TwoTone();
    }
}

