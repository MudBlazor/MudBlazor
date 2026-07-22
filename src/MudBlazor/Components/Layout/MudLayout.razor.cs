using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudLayout : MudDrawerContainer
    {
        protected override string Classname =>
            new CssBuilder("mud-layout")
                .AddClass(base.Classname)
                .Build();

        public MudLayout()
        {
            Fixed = true;
        }
    }
}
