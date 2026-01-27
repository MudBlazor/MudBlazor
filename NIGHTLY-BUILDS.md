# MudBlazor Nightly Builds

Nightly builds are automatically published to GitHub Packages whenever changes are merged to the `dev` branch. These builds allow you to test the latest features and bug fixes before they are officially released.

## 📦 Version Format

Nightly builds use the following version format:
```
{latest-tag}-nightly.{run-number}
```

For example, if the latest release tag is `v9.0.0-preview.2`, a nightly build might be:
```
9.0.0-preview.2-nightly.123
```

## 🚀 How to Use Nightly Builds

### 1. Configure NuGet Source

Add the MudBlazor GitHub Packages source to your NuGet configuration:

```bash
dotnet nuget add source "https://nuget.pkg.github.com/MudBlazor/index.json" \
  --name "MudBlazor-GitHub" \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_PAT \
  --store-password-in-clear-text
```

**Note:** You'll need a GitHub Personal Access Token (PAT) with `read:packages` scope. [Create one here](https://github.com/settings/tokens).

Alternatively, you can add a `nuget.config` file to your project:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="MudBlazor-GitHub" value="https://nuget.pkg.github.com/MudBlazor/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <MudBlazor-GitHub>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </MudBlazor-GitHub>
  </packageSourceCredentials>
</configuration>
```

### 2. Install the Nightly Package

Install the latest nightly build:

```bash
dotnet add package MudBlazor --version 9.0.0-preview.2-nightly.* --prerelease
```

Or specify an exact nightly version:

```bash
dotnet add package MudBlazor --version 9.0.0-preview.2-nightly.123 --prerelease
```

### 3. Update Your Project File

You can also manually edit your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="MudBlazor" Version="9.0.0-preview.2-nightly.*" />
</ItemGroup>
```

## ⚠️ Important Notes

- **Stability**: Nightly builds are development versions and may contain bugs or breaking changes.
- **Use for Testing**: These builds are intended for testing and early feedback, not for production use.
- **No Support**: Nightly builds are provided as-is without official support.
- **Retention**: Nightly build artifacts are retained for 30 days in the workflow runs.

## 🔍 Finding Nightly Versions

You can browse available nightly versions in several ways:

1. **GitHub Packages**: Visit [MudBlazor Packages](https://github.com/orgs/MudBlazor/packages?repo_name=MudBlazor)
2. **GitHub Actions**: Check the [Actions tab](https://github.com/MudBlazor/MudBlazor/actions/workflows/deploy-mudblazor-nightly.yml) for recent nightly build runs
3. **NuGet CLI**: List available versions:
   ```bash
   dotnet list package --outdated --include-prerelease
   ```

## 🐛 Reporting Issues

If you encounter issues with a nightly build:

1. Note the exact nightly version you're using
2. Check if the issue exists in the stable release
3. [Open an issue](https://github.com/MudBlazor/MudBlazor/issues/new/choose) with the nightly version number
4. Consider testing with the latest nightly to see if it's already fixed

## 🔄 Switching Back to Stable

To switch back to a stable release:

```bash
dotnet add package MudBlazor --version 9.0.0-preview.2
```

Or update your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="MudBlazor" Version="9.0.0-preview.2" />
</ItemGroup>
```

Then restore packages:

```bash
dotnet restore
```
