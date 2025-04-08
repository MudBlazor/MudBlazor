<h1>
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="content/MudBlazor-GitHub-NoBg-Dark.png">
    <source media="(prefers-color-scheme: light)" srcset="content/MudBlazor-GitHub-NoBg.png">
    <img alt="MudBlazor" src="content/MudBlazor-GitHub-NoBg.png">
  </picture>
</h1>

# Material Design components for Blazor
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/mudblazor/mudblazor/build-test-mudblazor.yml?branch=dev&logo=github&style=flat-square)
[![Codecov](https://img.shields.io/codecov/c/github/MudBlazor/MudBlazor)](https://app.codecov.io/github/MudBlazor/MudBlazor)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=MudBlazor_MudBlazor&metric=alert_status)](https://sonarcloud.io/summary/overall?id=MudBlazor_MudBlazor)
[![GitHub](https://img.shields.io/github/license/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/MudBlazor/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/MudBlazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/mudblazor)
[![Contributors](https://img.shields.io/github/contributors/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/mudblazor/graphs/contributors)
[![Discussions](https://img.shields.io/github/discussions/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/mudblazor/discussions)
[![Discord](https://img.shields.io/discord/786656789310865418?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/mudblazor)
[![Twitter](https://img.shields.io/twitter/follow/MudBlazor?color=1DA1F2&label=Twitter&logo=Twitter&style=flat-square)](https://twitter.com/MudBlazor)
[![NuGet version](https://img.shields.io/nuget/v/MudBlazor?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)
[![NuGet downloads](https://img.shields.io/nuget/dt/MudBlazor?color=ff4081&label=nuget%20downloads&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)

MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure. It is perfect for .NET developers who want to rapidly build web applications without having to struggle with CSS and Javascript. MudBlazor, being written entirely in C#, empowers you to adapt, fix or extend the framework. There are plenty of examples in the documentation, which makes understanding and learning MudBlazor very easy.

## 📘 Documentation & Demo
- 🌐 [MudBlazor.com](https://mudblazor.com) – Full documentation
- ⚡ [Try.MudBlazor.com](https://try.mudblazor.com/) – Interactive playground

## 💎 Why is MudBlazor so successful?
- Aesthetic design that follows Material Design principles.
- Intuitive, consistent component structure.
- Rich documentation with tons of examples and code snippets.
- Fully written in C# with minimal JavaScript.
- Build beautiful UIs without CSS (but fully customizable when needed).
- No third-party component dependencies – maximum flexibility.
- Strive for stability with extensive test coverage.
- Frequent releases so devs get their fixes and features fast.

## ⚙️ Prerequisites
| MudBlazor | .NET | Support |
| :--- | :---: | :---: |
| 1.x.x - 2.0.x | .NET 3.1 | Ended 03/2021 |
| 5.x.x | .NET 5 | Ended 01/2022 |
| 6.x.x | [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0), [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0), [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) | Ended 01/2025 |
| 7.x.x | [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0), [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) | Limited |
| 8.x.x | [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0), [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) | :heavy_check_mark: |

> [!TIP]
> Upgrading? Check our [Migration Guide](https://github.com/MudBlazor/MudBlazor/blob/dev/MIGRATION.md) for help with breaking changes.  

> [!WARNING]
> 1. Static rendering is not supported - [Learn more](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes).
> 2. Older browsers may not be supported. Use a modern, up-to-date browser - [Blazor supported platforms](https://learn.microsoft.com/aspnet/core/blazor/supported-platforms).

## 📊 Repo Stats
![Alt](https://repobeats.axiom.co/api/embed/db53a44092e88fc34a4c0f37db12773b6787ec7e.svg "Repobeats analytics image")

## 🤝 Contributing
Thanks for wanting to contribute! 👋  
Contributions from the community are what makes MudBlazor successful.

If you're comfortable with C#, Blazor, JavaScript, or CSS, we'd love your help!  
Whether it's fixing bugs, adding features, or improving documentation, every contribution counts.

We aim to review and merge non-breaking pull requests quickly.  
For larger features or changes, feel free to chat with us [on Discord](https://discord.gg/mudblazor) first to get feedback before diving in.

📚 Check out our [contribution guidelines](/CONTRIBUTING.md) to get started and learn more about how the project works.

## 🚀 Getting Started
We have ready-to-go templates at the [MudBlazor.Templates](https://github.com/mudblazor/Templates) repository, or follow the quick install guide to set things up manually:

### 🛠️ Quick Install
Install Package:
```
dotnet add package MudBlazor
```

Add to `_Imports.razor`:
```razor
@using MudBlazor
```

Add to the `MainLayout.razor` or `App.razor`:
```razor
<MudThemeProvider/>
<MudPopoverProvider/>
<MudDialogProvider/>
<MudSnackbarProvider/>
```

Add to your HTML `head` section:
```razor
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
```
It's either `index.html` or `_Layout.cshtml`/`_Host.cshtml`/`App.razor` depending on whether you're running WebAssembly or Server.
 
Next, add to the default Blazor script at the end of the `body`:
```razor
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

Add to the relevant sections of `Program.cs`:
```c#
using MudBlazor.Services;
```
```c#
builder.Services.AddMudServices();
```

### 🔗 Full Setup Guide
For more details, see the [complete installation guide](https://mudblazor.com/getting-started/installation) on our website.

### 💻 Example Usage
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
