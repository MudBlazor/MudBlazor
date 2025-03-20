using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.TestData
{
    public static class MouseEventArgsTestCase
    {
        public static TestCaseData[] AllCombinations()
        {
            return
                [
                    new TestCaseData(new MouseEventArgs { Button = 0}),
                    new TestCaseData (new MouseEventArgs { Button = 1 }),
                    new TestCaseData (new MouseEventArgs { Button = 2 }),
                ];
        }
    }
}
