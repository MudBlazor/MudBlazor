#!/usr/bin/env dotnet

// This is a wrapper script to setup and run Bun. It will use the globally installed Bun if available.
// If it is not available, or the version is older than the minimum required version, it will download
// and install Bun locally in the user's AppData folder.
// Usage: "dotnet tools/bun.cs -- [bun arguments]"

using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

const string BunMinVersion = "1.3.0";
Dictionary<Platform, string> DownloadUrls = new()
{
    {
        Platform.WindowsX64,
        "https://github.com/oven-sh/bun/releases/latest/download/bun-windows-x64-baseline.zip"
    },
    {
        Platform.LinuxX64,
        "https://github.com/oven-sh/bun/releases/latest/download/bun-linux-x64-baseline.zip"
    },
    {
        Platform.LinuxArm64,
        "https://github.com/oven-sh/bun/releases/latest/download/bun-linux-aarch64.zip"
    },
    {
        Platform.MacOsX64,
        "https://github.com/oven-sh/bun/releases/latest/download/bun-darwin-x64.zip"
    },
    {
        Platform.MacOsArm64,
        "https://github.com/oven-sh/bun/releases/latest/download/bun-darwin-aarch64.zip"
    },
};

var workingDirectory = Environment.CurrentDirectory;
var arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();

Platform platform =
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    && Environment.Is64BitOperatingSystem
    && RuntimeInformation.ProcessArchitecture == Architecture.X64
        ? Platform.WindowsX64
    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
    && Environment.Is64BitOperatingSystem
    && RuntimeInformation.ProcessArchitecture == Architecture.X64
        ? Platform.MacOsX64
    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
    && Environment.Is64BitOperatingSystem
    && RuntimeInformation.ProcessArchitecture == Architecture.Arm64
        ? Platform.MacOsArm64
    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
    && Environment.Is64BitOperatingSystem
    && RuntimeInformation.ProcessArchitecture == Architecture.X64
        ? Platform.LinuxX64
    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
    && Environment.Is64BitOperatingSystem
    && RuntimeInformation.ProcessArchitecture == Architecture.Arm64
        ? Platform.LinuxArm64
    : throw new NotSupportedException("Unsupported platform");

Console.WriteLine($"MudBlazor Bun Wrapper");
Console.WriteLine($"Platform: {platform}");
Console.WriteLine($"Working Directory: {workingDirectory}");

// Check if Bun is already installed globally
try
{
    if (await IsGlobalInstalledAsync() && await IsSupportedVersionAsync("bun"))
    {
        await RunBunAsync("bun", arguments);
        return;
    }
}
catch
{
    // Ignore
}

var bunExecutable = await InstallAsync();
await RunBunAsync(bunExecutable, arguments);
return;

async Task RunBunAsync(string bunExecutable, string[] args)
{
    string joinedArgs = string.Join(" ", args);
    Console.WriteLine($"Running: {bunExecutable} {joinedArgs}" + Environment.NewLine);
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = bunExecutable,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory,
        },
    };
    process.StartInfo.ArgumentList.Clear();
    foreach (var a in args)
        process.StartInfo.ArgumentList.Add(a);

    process.Start();
    await process.WaitForExitAsync();
    Environment.Exit(process.ExitCode);
}

async Task<bool> IsGlobalInstalledAsync()
{
    var which = platform switch
    {
        Platform.WindowsX64 => "where",
        _ => "which",
    };

    // first check if bunExecutable exists
    var whichProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = which,
            Arguments = "bun",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory,
        },
    };
    whichProcess.Start();
    string whichOutput = await whichProcess.StandardOutput.ReadToEndAsync();
    await whichProcess.WaitForExitAsync();
    if (whichProcess.ExitCode != 0 || string.IsNullOrWhiteSpace(whichOutput))
    {
        return false;
    }

    return true;
}

async Task<bool> IsSupportedVersionAsync(string bunExecutable)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = bunExecutable,
            Arguments = "--version",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory,
        },
    };
    process.Start();
    string output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();
    if (process.ExitCode != 0)
    {
        return false;
    }

    var (major, minor, patch) = ParseVersion(output.Trim());
    var (minMajor, minMinor, minPatch) = ParseVersion(BunMinVersion);
    if (
        major < minMajor
        || (major == minMajor && minor < minMinor)
        || (major == minMajor && minor == minMinor && patch < minPatch)
    )
    {
        Console.WriteLine(
            $"Bun version {output.Trim()} is older than the required version {BunMinVersion}"
        );
        return false;
    }

    return true;
}

(int, int, int) ParseVersion(string version)
{
    var m = Regex.Match(version, @"\b(\d+)\.(\d+)\.(\d+)\b");
    if (!m.Success)
        throw new FormatException("Invalid version format");
    return (
        int.Parse(m.Groups[1].Value),
        int.Parse(m.Groups[2].Value),
        int.Parse(m.Groups[3].Value)
    );
}

async Task<string> InstallAsync()
{
    string installDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MudBlazor",
        "bun"
    );
    Directory.CreateDirectory(installDir);
    var exeName = platform switch
    {
        Platform.WindowsX64 => "bun.exe",
        _ => "bun",
    };

    // Check if bun is already installed
    string bunExecutable = Path.Combine(installDir, exeName);
    if (File.Exists(bunExecutable))
    {
        try
        {
            if (await IsSupportedVersionAsync(bunExecutable))
            {
                return bunExecutable;
            }
        }
        catch
        {
            // Ignore
        }
    }

    using var _ = await AcquireInstallLockAsync(TimeSpan.FromMinutes(5));
    // Check if bun is installed again (another process may have installed it while we were waiting for the lock)
    bunExecutable = Path.Combine(installDir, exeName);
    if (File.Exists(bunExecutable))
    {
        try
        {
            if (await IsSupportedVersionAsync(bunExecutable))
            {
                return bunExecutable;
            }
        }
        catch
        {
            // Ignore
        }
    }
    Directory.Delete(installDir, recursive: true);
    Directory.CreateDirectory(installDir);

    // Download zip file to temp directory
    string downloadUrl = DownloadUrls[platform];
    Console.WriteLine($"Downloading Bun from {downloadUrl}...");
    string tempZipPath = Path.Combine(Path.GetTempPath(), $"bun-{Guid.NewGuid()}.zip");
    using var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(downloadUrl);
    response.EnsureSuccessStatusCode();
    using var zipFileStream = new FileStream(
        tempZipPath,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None
    );
    await response.Content.CopyToAsync(zipFileStream);
    zipFileStream.Close();

    // Find bun executable in zip file and extract it
    using var zipArchive = System.IO.Compression.ZipFile.OpenRead(tempZipPath);
    var bunEntry = zipArchive.Entries.FirstOrDefault(e => e.Name == exeName);
    if (bunEntry == null)
    {
        throw new FileNotFoundException("Bun executable not found in the downloaded zip file.");
    }
    await bunEntry.ExtractToFileAsync(bunExecutable, overwrite: true);
    File.Delete(tempZipPath);

    return bunExecutable;
}

async Task<IDisposable> AcquireInstallLockAsync(TimeSpan timeout)
{
    // ensure directory exists (keeps previous behavior for where lock metadata may live)
    Directory.CreateDirectory(Path.GetDirectoryName(MutexReleaser.InstallLockPath)!);

    // derive a stable mutex name from the lock path
    string mutexName;
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(MutexReleaser.InstallLockPath));
    mutexName = "MudBlazor.BunInstall." + Convert.ToHexString(hash).Substring(0, 16);

    var mutex = new Mutex(false, mutexName);
    bool acquired = false;
    try
    {
        try
        {
            acquired = mutex.WaitOne(timeout);
        }
        catch (AbandonedMutexException)
        {
            // previous holder crashed — consider lock acquired
            acquired = true;
        }

        if (!acquired)
        {
            mutex.Dispose();
            throw new TimeoutException("Timeout acquiring bun install lock");
        }

        // write pid info as best-effort metadata (non-essential)
        try
        {
            File.WriteAllText(
                MutexReleaser.InstallLockPath,
                $"{Process.GetCurrentProcess().Id}|{DateTime.UtcNow:o}"
            );
        }
        catch
        {
            // best-effort
        }

        return new MutexReleaser(mutex);
    }
    catch
    {
        try
        {
            mutex.Dispose();
        }
        catch { }
        throw;
    }
}

class MutexReleaser : IDisposable
{
    public static string InstallLockPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MudBlazor",
        "bun",
        "install.lock"
    );

    private readonly Mutex _mutex;

    public MutexReleaser(Mutex mutex) => _mutex = mutex;

    public void Dispose()
    {
        try
        {
            _mutex.ReleaseMutex();
        }
        catch
        {
            // best-effort
        }
        try
        {
            _mutex.Dispose();
        }
        catch
        {
            // best-effort
        }
    }
}

enum Platform
{
    WindowsX64,
    LinuxX64,
    LinuxArm64,
    MacOsX64,
    MacOsArm64,
}
