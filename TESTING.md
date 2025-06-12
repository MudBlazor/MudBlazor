# 🔧 Testing a PR Locally

1. **Clone and check out the PR:**

```bash
git clone https://github.com/MudBlazor/MudBlazor.git
cd MudBlazor
git fetch origin pull/PR_NUMBER/head:pr-branch
git checkout pr-branch
```

2. **Pack MudBlazor with a custom version:**

```bash
dotnet pack src/MudBlazor/MudBlazor.csproj -c Release -o ./LocalNuGet -p:Version=8.0.0-custom
```

3. **Add a `nuget.config` in your app folder:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalMud" value="C:\Path\To\MudBlazor\LocalNuGet" />
    </packageSources>
</configuration>
```

4. **Update your app's `.csproj`:**

```xml
<PackageReference Include="MudBlazor" Version="8.0.0-custom" />
```

5. **Restore and build:**

```bash
dotnet restore
dotnet build
```

✅ You're all set!
