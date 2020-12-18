using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTextField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control").AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }
    }

    public class MudTextFieldString : MudTextField<string> {}
}
