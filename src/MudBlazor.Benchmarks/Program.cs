// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Running;

namespace MudBlazor.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        // Run all benchmarks by default
        if (args.Length == 0)
        {
            Console.WriteLine("MudBlazor ParameterState Performance Benchmarks");
            Console.WriteLine("===============================================");
            Console.WriteLine();
            Console.WriteLine("Available benchmark suites:");
            Console.WriteLine("  1. Basic Operations (--basic)");
            Console.WriteLine("  2. Large Scale (--largescale)");
            Console.WriteLine("  3. Comparer Strategies (--comparer)");
            Console.WriteLine("  4. Identifier Generation (--identifier)");
            Console.WriteLine("  5. All benchmarks (--all or no arguments)");
            Console.WriteLine();
            Console.WriteLine("Usage: dotnet run -c Release -- [--basic|--largescale|--comparer|--identifier|--all]");
            Console.WriteLine();
            Console.WriteLine("Running all benchmarks...");
            Console.WriteLine();

            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
        else if (args.Contains("--basic"))
        {
            BenchmarkRunner.Run<ParameterStateBasicOperationsBenchmark>();
        }
        else if (args.Contains("--largescale"))
        {
            BenchmarkRunner.Run<ParameterStateLargeScaleBenchmark>();
        }
        else if (args.Contains("--comparer"))
        {
            BenchmarkRunner.Run<ParameterStateComparerBenchmark>();
        }
        else if (args.Contains("--identifier"))
        {
            BenchmarkRunner.Run<IdentifierBenchmark>();
        }
        else if (args.Contains("--all"))
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
        else
        {
            Console.WriteLine("Unknown argument. Use --basic, --largescale, --comparer, --identifier, or --all");
        }
    }
}
