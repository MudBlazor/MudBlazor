// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

// Enable test fixture parallelization for faster test execution
[assembly: Parallelizable(ParallelScope.Fixtures)]

// Set the maximum number of parallel workers based on available processors
[assembly: LevelOfParallelism(8)]
