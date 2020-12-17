using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTextField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control").AddClass(Class)
           .Build();

    }

    public class MudTextFieldString : MudTextField<string> {}
}
