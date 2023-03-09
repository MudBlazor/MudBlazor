<h1>
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="content/MudBlazor-GitHub-NoBg-Dark.png">
    <source media="(prefers-color-scheme: light)" srcset="content/MudBlazor-GitHub-NoBg.png">
    <img alt="MudBlazor" src="content/MudBlazor-GitHub-NoBg.png">
  </picture>
</h1>

# Material Design components for Blazor
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/mudblazor/mudblazor/build-test-mudblazor.yml?branch=dev&logo=github&style=flat-square)
![Codecov](https://img.shields.io/codecov/c/github/MudBlazor/MudBlazor?logo=codecov&logoColor=white&style=flat-square&token=4BBNQPPATD)
[![GitHub](https://img.shields.io/github/license/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/MudBlazor/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/MudBlazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/mudblazor)
[![Contributors](https://img.shields.io/github/contributors/mudblazor/mudblazor?color=594ae2&style=flat-square&logo=github)](https://github.com/mudblazor/mudblazor/graphs/contributors)
[![Discussions](https://img.shields.io/github/discussions/mudblazor/mudblazor?color=594ae2&logo=github&style=flat-square)](https://github.com/mudblazor/mudblazor/discussions)
[![Discord](https://img.shields.io/discord/786656789310865418?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/mudblazor)
[![Twitter](https://img.shields.io/twitter/follow/MudBlazor?color=1DA1F2&label=Twitter&logo=Twitter&style=flat-square)](https://twitter.com/MudBlazor)
[![Nuget version](https://img.shields.io/nuget/v/MudBlazor?color=ff4081&label=nuget%20version&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)
[![Nuget downloads](https://img.shields.io/nuget/dt/MudBlazor?color=ff4081&label=nuget%20downloads&logo=nuget&style=flat-square)](https://www.nuget.org/packages/MudBlazor/)

MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure. It is perfect for .NET developers who want to rapidly build web applications without having to struggle with CSS and Javascript. MudBlazor, being written entirely in C#, empowers you to adapt, fix or extend the framework. There are plenty of examples in the documentation, which makes understanding and learning MudBlazor very easy.

## Documentation & Demo
- [MudBlazor.com](https://mudblazor.com)
- [Try.MudBlazor.com](https://try.mudblazor.com/)

### Why is MudBlazor so successful?
 - Clean and aesthetic graphic design based on Material Design.
 - Clear and easy to understand structure.
 - Good documentation with many examples and source snippets.
 - All components are written entirely in C#, no JavaScript allowed (except where absolutely necessary).
 - Users can make beautiful apps without needing CSS (but they can of course use CSS too).
 - No dependencies on other component libraries, 100% control over components and features.
 - Stability! We strive for a complete test coverage.
 - Releasing often so developers can get their PRs and fixes in a timely fashion.

## Prerequisities
| MudBlazor | .NET | Support |
| :--- | :---: | :---: |
| 1.x.x - 2.0.x | .NET 3.1 | Ended 03/2021 |
| 5.x.x | .NET 5 | Ended 01/2022 |
| 6.0.x | [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) | :heavy_check_mark: |
| 6.1.x | [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) & [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) | :heavy_check_mark: |

## Stats
![Alt](https://repobeats.axiom.co/api/embed/db53a44092e88fc34a4c0f37db12773b6787ec7e.svg "Repobeats analytics image")

## Contributing
- Check out the [contribution guidelines](/CONTRIBUTING.md) if you want to help improve MudBlazor.

## Getting Started
- Full installation instructions can be found at [mudblazor.com](https://mudblazor.com/getting-started/installation)  
- Alternatively use one of our templates from the [MudBlazor.Templates](https://github.com/mudblazor/Templates) repo.
### Quick Installation Guide

Install Package
```
dotnet add package MudBlazor
```
Add the following to `_Imports.razor`
```razor
@using MudBlazor
```
Add the following to the `MainLayout.razor` or `App.razor`
```razor
<MudThemeProvider/>
<MudDialogProvider/>
<MudSnackbarProvider/>
```
Add the following to `index.html` (client-side) or `_Host.cshtml` (server-side) in the `head`
```razor
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
```
Add the following to `index.html` or `_Host.cshtml` in the `body`
```razor
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

Add the following to the relevant sections of `Program.cs`
```c#
using MudBlazor.Services;
```
```c#
builder.Services.AddMudServices();
```

### Usage
```razor
<MudText Typo="Typo.h6">MudBlazor is @Text</MudText>
<MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="ButtonOnClick">@ButtonText</MudButton>

@code {
  public string Text { get; set; } = "????";
  public string ButtonText { get; set; } = "Click Me";
  public int ButtonClicked { get; set; }

  void ButtonOnClick()
  {
      ButtonClicked += 1;
      Text = $"Awesome x {ButtonClicked}";
      ButtonText = "Click Me Again";
  }
}
```
