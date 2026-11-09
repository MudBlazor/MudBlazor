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

MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure. It is perfect for .NET developers who want to rapidly build web applications without having to struggle with CSS and Javascript. MudBlazor, being written entirely in C#, empowers you to adapt, fix or extend the framework. There are plenty of examples in the documentation, which makes understanding and learning MudBlazor very easy.

**🌐 [Documentation](https://mudblazor.com/docs/overview) ⚡ [Interactive Playground](https://try.mudblazor.com)**

## 💎 Why Choose MudBlazor?

- Clean and aesthetic graphic design based on Material Design.
- Clear and easy to understand structure.
- Good documentation with many examples and source snippets.
- All components are written entirely in C#, no JavaScript allowed (except where absolutely necessary).
- Users can make beautiful apps without needing CSS (but they can of course use CSS too).
- No dependencies on other component libraries, 100% control over components and features.
- Stability! We strive for a complete test coverage.
- Releasing often so developers can get their PRs and fixes in a timely fashion.

## 📊 Repo Stats

![Repobeats analytics image](https://repobeats.axiom.co/api/embed/db53a44092e88fc34a4c0f37db12773b6787ec7e.svg)

## 🚀 Getting Started

See the [installation guide](https://mudblazor.com/getting-started/installation) to get started.

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
> 4. Want to test the latest features? Learn about our [nightly builds](https://github.com/MudBlazor/MudBlazor/discussions/12621)!
