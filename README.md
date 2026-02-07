# ![MudBlazor Logo](content/MudBlazor-GitHub-NoBg-Dark.png)

# Material Design components for Blazor
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/mudblazor/mudblazor/build-test-mudblazor.yml?branch=dev&logo=github&style=flat-square)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=MudBlazor_MudBlazor&metric=alert_status)](https://sonarcloud.io/summary/overall?id=MudBlazor_MudBlazor)
[![Codecov](https://img.shields.io/codecov/c/github/MudBlazor/MudBlazor)](https://app.codecov.io/github/MudBlazor/MudBlazor)
[![GitHub](https://img.shields.io/github/license/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/MudBlazor/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/MudBlazor/stargazers)
[![Contributors](https://img.shields.io/github/contributors/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/mudblazor/graphs/contributors)
[![Discussions](https://img.shields.io/github/discussions/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/mudblazor/discussions)
[![Discord](https://img.shields.io/discord/786656789310865418?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/mudblazor)
[![Twitter](https://img.shields.io/twitter/follow/MudBlazor?color=1DA1F2&label=Twitter&logo=Twitter&style=flat-square)](https://twitter.com/MudBlazor)
[![NuGet version](https://img.shields.io/nuget/v/MudBlazor?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)
[![NuGet downloads](https://img.shields.io/nuget/dt/MudBlazor?color=ff4081&label=nuget%20downloads&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)

Build stunning, interactive web applications with MudBlazor — the open-source Material Design library for Blazor.

**🌐 [Documentation](https://mudblazor.com/docs/overview) ⚡ [Interactive Playground](https://try.mudblazor.com)**

## 💎 Why Choose MudBlazor?

📖 Comprehensive documentation and examples  
🎨 Beautiful Material Design components  
💻 Write in C# with minimal JavaScript  
✅ Extensive test coverage  

## 📊 Repo Stats

![Repobeats analytics image](https://repobeats.axiom.co/api/embed/db53a44092e88fc34a4c0f37db12773b6787ec7e.svg)

## 🚀 Getting Started

Use one of our [templates](https://github.com/MudBlazor/Templates), or follow the quick install guide below:

### Installation

Install Package:

```bash
dotnet add package MudBlazor
```

Add to `_Imports.razor`:

```razor
@using MudBlazor
```

Add to `MainLayout.razor` or `App.razor`:

```razor
<MudThemeProvider/>
<MudPopoverProvider/>
<MudDialogProvider/>
<MudSnackbarProvider/>
```

Add to HTML `head` section (`index.html`/`_Layout.cshtml`/`_Host.cshtml`/`App.razor`):

```razor
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
```

Add to the default Blazor script at the end of the HTML `body` section:

```razor
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

Add to `Program.cs`:

```c#
using MudBlazor.Services;
```

```c#
builder.Services.AddMudServices();
```

### Example Usage

```razor
<MudText Typo="Typo.h6">
    MudBlazor is @Text
</MudText>

<MudButton Variant="Variant.Filled" 
           Color="Color.Primary" 
           OnClick="ButtonOnClick">
    @ButtonText
</MudButton>

@code {
    string Text { get; set; } = "????";
    string ButtonText { get; set; } = "Click Me";
    int ClickCount { get; set; }

    void ButtonOnClick()
    {
        ClickCount += 1;
        Text = $"Awesome x {ClickCount}";
        ButtonText = "Click Me Again";
    }
}
```

For more details, see the [full installation guide](https://mudblazor.com/getting-started/installation).

## 🤝 Contributing

Contributions from the community are what make MudBlazor successful.  

💬 Feel free to chat with us [on Discord](https://discord.gg/mudblazor) to get feedback before diving in.  
📚 Check out our [contribution guidelines](/CONTRIBUTING.md) to get started and learn more about how the project works.  
🧪 If a PR fixes something you reported, [locally test it](https://github.com/MudBlazor/MudBlazor/discussions/12085) to ensure your app works as expected.

## ⚙️ Version Support

| MudBlazor | .NET | Support |
| :--- | :---: | :---: |
| 5.x.x | .NET 5 | Ended Jan 2022 |
| 6.x.x | [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0), [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0), [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) | Ended Jan 2025 |
| 7.x.x | [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0), [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) | Ended Jan 2026 |
| 8.x.x | [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0), [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) | Limited Support |
| 9.x.x | [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0), [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0), [.NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) | ✅ Full Support |

> [!NOTE]
> 1. Upgrading? Check our [Migration Guide](https://github.com/MudBlazor/MudBlazor/discussions/12086) for help with breaking changes.  
> 2. Static rendering is not supported. [Learn more](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes)
> 3. Use an up-to-date browser. [Blazor supported platforms](https://learn.microsoft.com/aspnet/core/blazor/supported-platforms)
> 4. Want to test the latest features? Check out our [nightly builds](/NIGHTLY-BUILDS.md)!
