using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Compiler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            bool success = 
                new CodeSnippets().Execute()
                && new DocStrings().Execute()
                && new ExamplesMarkup().Execute()
                && new TestsForExamples().Execute()
                && new TestsForApiPages().Execute();
            return success ? 0 : 1;
        }
    }
}