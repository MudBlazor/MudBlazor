// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace MudBlazor.Benchmarks;

/// <summary>
/// Benchmarks for Identifier generation to test different implementation strategies.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class IdentifierBenchmark
{
    private const string TestPrefix = "test";
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int CharsLength = 36;
    private const int RandomStringLength = 8;

    /// <summary>
    /// Benchmark: Current implementation with prefix
    /// </summary>
    [Benchmark(Baseline = true)]
    public string Current_WithPrefix()
    {
        ReadOnlySpan<char> prefix = TestPrefix;
        Span<char> identifierSpan = stackalloc char[prefix.Length + RandomStringLength];
        prefix.CopyTo(identifierSpan);
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[prefix.Length + i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Current implementation without prefix (always uses 'a')
    /// </summary>
    [Benchmark]
    public string Current_NoPrefix()
    {
        ReadOnlySpan<char> prefix = ['a'];
        Span<char> identifierSpan = stackalloc char[prefix.Length + RandomStringLength];
        prefix.CopyTo(identifierSpan);
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[prefix.Length + i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Improved version with random prefix (single random char)
    /// </summary>
    [Benchmark]
    public string Improved_RandomPrefix_SingleChar()
    {
        Span<char> identifierSpan = stackalloc char[1 + RandomStringLength];
        // Generate random prefix
        identifierSpan[0] = Chars[Random.Shared.Next(26)]; // Only letters for prefix
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[1 + i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Improved version with random prefix (two random chars)
    /// </summary>
    [Benchmark]
    public string Improved_RandomPrefix_TwoChars()
    {
        Span<char> identifierSpan = stackalloc char[2 + RandomStringLength];
        // Generate random prefix (2 chars)
        identifierSpan[0] = Chars[Random.Shared.Next(26)]; // Only letters
        identifierSpan[1] = Chars[Random.Shared.Next(26)]; // Only letters
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[2 + i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: All random chars (no separate prefix logic)
    /// </summary>
    [Benchmark]
    public string Improved_AllRandom()
    {
        Span<char> identifierSpan = stackalloc char[RandomStringLength];
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Using Guid as baseline comparison
    /// </summary>
    [Benchmark]
    public string Guid_ToString()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Benchmark: Using Guid.NewGuid() with custom format (N - no dashes)
    /// </summary>
    [Benchmark]
    public string Guid_ToStringN()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Benchmark: Hybrid approach - use first char from timestamp + random
    /// </summary>
    [Benchmark]
    public string Improved_TimestampBased()
    {
        var ticks = DateTime.UtcNow.Ticks;
        Span<char> identifierSpan = stackalloc char[1 + RandomStringLength];
        // Use ticks to get a varied first character
        identifierSpan[0] = Chars[(int)((ticks >> 8) % 26)]; // Only letters
        for (var i = 0; i < RandomStringLength; i++)
        {
            var index = Random.Shared.Next(CharsLength);
            identifierSpan[1 + i] = Chars[index];
        }
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Batch random number generation (potentially faster)
    /// </summary>
    [Benchmark]
    public string Improved_BatchRandom()
    {
        Span<char> identifierSpan = stackalloc char[RandomStringLength];
        Span<int> randomIndices = stackalloc int[RandomStringLength];
        
        // Generate all random numbers at once
        for (var i = 0; i < RandomStringLength; i++)
        {
            randomIndices[i] = Random.Shared.Next(CharsLength);
        }
        
        // Then convert to chars
        for (var i = 0; i < RandomStringLength; i++)
        {
            identifierSpan[i] = Chars[randomIndices[i]];
        }
        
        return identifierSpan.ToString();
    }

    /// <summary>
    /// Benchmark: Using NextInt64 to generate multiple chars at once
    /// </summary>
    [Benchmark]
    public string Improved_Int64Based()
    {
        Span<char> identifierSpan = stackalloc char[RandomStringLength];
        
        // Generate random long and extract multiple indices
        var random1 = Random.Shared.NextInt64();
        var random2 = Random.Shared.NextInt64();
        
        for (var i = 0; i < 4; i++)
        {
            identifierSpan[i] = Chars[(int)((random1 >> (i * 8)) % CharsLength)];
        }
        for (var i = 4; i < 8; i++)
        {
            identifierSpan[i] = Chars[(int)((random2 >> ((i - 4) * 8)) % CharsLength)];
        }
        
        return identifierSpan.ToString();
    }
}
