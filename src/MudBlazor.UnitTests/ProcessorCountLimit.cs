// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.UnitTests;
using TUnit.Core.Interfaces;

[assembly: ParallelLimiter<ProcessorCountLimit>]

namespace MudBlazor.UnitTests;

public class ProcessorCountLimit : IParallelLimit
{
    public int Limit => Environment.ProcessorCount;
}
