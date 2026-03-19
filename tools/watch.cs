#!/usr/bin/env dotnet

// This script extends the 'dotnet watch' functionality to also watch and rebuild
// MudBlazor JS/CSS assets using Bun whenever relevant source files change.
// 
// Usage examples:
//   dotnet tools/watch.cs
//   dotnet ../tools/watch.cs

using System.Diagnostics;
using System.Runtime.CompilerServices;

await Run();

static async Task Run()
{
    var repositoryRoot = GetRepositoryRoot();
    var toolsDirectory = Path.Combine(repositoryRoot, "tools");
    var srcDirectory = Path.Combine(repositoryRoot, "src");
    var mudblazorProjectDirectory = Path.Combine(srcDirectory, "MudBlazor");
    var mudblazorDocsProjectDirectory = Path.Combine(srcDirectory, "MudBlazor.Docs.Server");
    var assetBuildScript = Path.Combine(mudblazorProjectDirectory, "build.mjs");
    var buildPropsFile = Path.Combine(srcDirectory, "Directory.Build.props");
    var versions = GetVersions(buildPropsFile);
    await RestoreTools(repositoryRoot);

    using var docsProcess = new Process()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "watch",
            WorkingDirectory = mudblazorDocsProjectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Environment =
            {
                // Restart if hot reload is not possible
                ["DOTNET_WATCH_RESTART_ON_RUDE_EDIT"] = "1"
            }
        },
    };

    using var assetsProcess = new Process()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = mudblazorProjectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        },
    };
    string[] assetsProcessArgs =
    [
        "tool",
        "run",
        "bun",
        "--",
        "wrapper",
        "--version",
        versions.BunVersion,
        "--path",
        toolsDirectory,
        "--",
        assetBuildScript,
        "watch",
    ];

    foreach (var arg in assetsProcessArgs)
    {
        assetsProcess.StartInfo.ArgumentList.Add(arg);
    }

    docsProcess.Start();
    assetsProcess.Start();

    // Redirect output and error streams
    var docsOutputTask = RedirectStreams(docsProcess, "docs");
    var assetsOutputTask = RedirectStreams(assetsProcess, "assets");

    Console.WriteLine("Watching for changes. Press Ctrl+C to exit.");
    Console.CancelKeyPress += (sender, e) =>
    {
        e.Cancel = true;
        if (!docsProcess.HasExited)
        {
            docsProcess.Kill();
        }
        if (!assetsProcess.HasExited)
        {
            assetsProcess.Kill();
        }
    };

    await Task.WhenAll(
        docsOutputTask,
        assetsOutputTask,
        docsProcess.WaitForExitAsync(),
        assetsProcess.WaitForExitAsync()
    );
}

static string GetScriptPath([CallerFilePath] string? path = null)
{
    return path!;
}

static string GetRepositoryRoot()
{
    var scriptPath = GetScriptPath();
    var scriptDirectory = Path.GetDirectoryName(scriptPath);
    return Path.GetFullPath(Path.Combine(scriptDirectory!, "../"));
}

static Versions GetVersions(string buildPropsFile)
{
    var doc = System.Xml.Linq.XDocument.Load(buildPropsFile);
    return new Versions
    {
        BunVersion = doc.Descendants(nameof(Versions.BunVersion)).First().Value,
    };
}

static async Task RestoreTools(string repositoryRoot)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "tool restore",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        },
    };

    process.Start();
    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();
    await Task.WhenAll(outputTask, errorTask, process.WaitForExitAsync());
    var output = await outputTask;
    var error = await errorTask;

    if (!string.IsNullOrWhiteSpace(output))
    {
        Console.WriteLine(output.TrimEnd());
    }

    if (!string.IsNullOrWhiteSpace(error))
    {
        Console.Error.WriteLine(error.TrimEnd());
    }

    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"Failed to restore dotnet tools. Exit code: {process.ExitCode}");
    }
}

static async Task RedirectStreams(Process process, string prefix)
{
    var outputTask = Task.Run(async () =>
    {
        var line = await process.StandardOutput.ReadLineAsync();
        while (line is not null)
        {
            Console.WriteLine($"[{prefix}] {line}");
            line = await process.StandardOutput.ReadLineAsync();
        }
    });
    var errorTask = Task.Run(async () =>
    {
        var line = await process.StandardError.ReadLineAsync();
        while (line is not null)
        {
            Console.Error.WriteLine($"[{prefix}] {line}");
            line = await process.StandardError.ReadLineAsync();
        }
    });
    await Task.WhenAll(outputTask, errorTask);
}

class Versions
{
    public required string BunVersion { get; init; }
}
