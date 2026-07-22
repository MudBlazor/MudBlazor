using System.Collections.Generic;

namespace MudBlazor.UnitTests.Mocks
{
#nullable enable
    public class MockIntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
}
