# ![MudBlazor](content/MudBlazor-GitHub-NoBg.png)
# Material Design components for Blazor
![Azure DevOps builds (master)](https://img.shields.io/azure-devops/build/gardnet-nu/4cba3d30-858f-4653-a80d-736a8adc5daf/1/master?label=azure%20pipelines&style=flat-square)
![Azure DevOps coverage (develop)](https://img.shields.io/azure-devops/coverage/gardnet-nu/MudBlazor/1/dev)
[![GitHub](https://img.shields.io/github/license/garderoben/mudblazor?color=%23594ae2&style=flat-square)](https://github.com/Garderoben/MudBlazor/blob/master/LICENSE)
[![Gitter](https://img.shields.io/gitter/room/MudBlazor/community?style=flat-square)](https://gitter.im/MudBlazor/community)
[![Discord](https://img.shields.io/discord/786656789310865418?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square)](https://discord.gg/mudblazor)
[![GitHub Repo stars](https://img.shields.io/github/stars/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor)
[![Nuget](https://img.shields.io/nuget/v/MudBlazor?style=flat-square)](https://www.nuget.org/packages/MudBlazor/)

MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure. It is perfect for .NET developers who want to rapidly build web applications without having to struggle with CSS and Javascript. MudBlazor, being written entirely in C#, empowers you to adapt, fix or extend the framework. There are plenty of examples in the documentation, which makes understanding and learning MudBlazor very easy.
### Design goals:
 - Clean and aesthetic graphic design based on Material Design.
 - Clear and easy to understand structure.
 - Good documentation with many examples and source snippets.
 - All components are written entirely in C#, no JavaScript allowed (except where absolutely necessary).
 - Users can make beautiful apps without needing CSS (but they can of course use CSS too)
 - No dependencies on other component libraries, 100% control over components and features.
 - Stability! We strive for a complete test coverage.
 - Releasing often so developers can get their PRs and fixes in a timely fashion.
## Documentation & Demo
- [MudBlazor.com](https://mudblazor.com)
- [Try.MudBlazor.com](https://try.mudblazor.com/)
## Prerequisites
- [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) or [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) (legacy)
## Getting Started
- Full installation instructions can be found at [mudblazor.com](https://mudblazor.com/getting-started/installation)  
- Alternatively use one of our templates from the [MudBlazor.Templates](https://github.com/Garderoben/MudBlazor.Templates) repo.
### Quick Installation Guide
#### Common Configuration (Client-Side or Server-Side)
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
#### Client-Side Configuration(WebAssembly)
Add the following to the relevant sections of `Program.cs`
```c#
using MudBlazor.Services;
```
```c#
builder.Services.AddMudServices();
```
#### Server-Side Configuration
Add the following to the relevant sections of `Startup.cs`
```c#
using MudBlazor.Services;
```
```c#
services.AddMudServices();
```
### Usage
```html
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
