using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.TestData
{
    public class MouseEventArgsTestCase
    {
        public static IEnumerable<Func<MouseEventArgs>> AllCombinations()
        {
            yield return () => new MouseEventArgs { Button = 0 };
            yield return () => new MouseEventArgs { Button = 1 };
            yield return () => new MouseEventArgs { Button = 2 };
        }
    }
}
