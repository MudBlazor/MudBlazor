# ![MudBlazor](content/MudBlazor-GitHub.png)
# Material Design components for Blazor
![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/gardnet-nu/4cba3d30-858f-4653-a80d-736a8adc5daf/1/master?label=azure%20pipelines&style=flat-square)
[![GitHub](https://img.shields.io/github/license/garderoben/mudblazor?color=%23594ae2&style=flat-square)](https://github.com/Garderoben/MudBlazor/blob/master/LICENSE)
[![Gitter](https://img.shields.io/gitter/room/MudBlazor/community?style=flat-square)](https://gitter.im/MudBlazor/community)
[![GitHub Repo stars](https://img.shields.io/github/stars/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor)

MudBlazor is an ambitious Material Design component framework for Blazor with an emphasis on ease of use and clear structure. It is perfect for .NET developers who want to rapidly build web applications without having to struggle with CSS and Javascript. MudBlazor, being written entirely in C#, empowers them to adapt, fix or extend the framework and the multitude of examples in the documentation makes learning MudBlazor very easy. 
	
Design goals:
 - Clean and aesthetic graphic design based on Material Design
 - Clear and easy to understand structure
 - Good documentation with many examples and source snippets
 - All components are written entirely in C#, no JavaScript allowed (except absolutely necessary)
 - Users can make beautiful apps without needing CSS (but they can of course use CSS too)
 - No dependencies on other component libraries, 100% control over components and features
 - Stability! We strive for a complete test coverage. 
 - Releasing often so developers can get their PRs and fixes in a timely fashion

## Documentation & Demo
- [MudBlazor.com](https://mudblazor.com)
- [Try.MudBlazor.com](https://try.mudblazor.com/)


## Prerequisites

- .NET Core 3.1
- Visual Studio 2019 with the ASP.NET and Web development.


## Installation 
#### Full installation instructions
At mudblazor.com we have more in depth installation instructions, [available here.](https://mudblazor.com/getting-started/installation)

#### Simplified installation instructions
```
Install-Package MudBlazor
```
In `_Imports.razor`
```
@using MudBlazor
```

Add the following link's in your head section. For **Client Side (WebAssembly)** you do that in `index.html` and for **Server Side** `_Host.cshtml`
```
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
```

In `MainLayout.razor` or `App.razor`
```
<MudThemeProvider/>
```

## Usage
```html
<Typography Typo="Typo.h6">MudBlazor is @Text</Typography>
<Button Variant="Variant.Contained" Color="Color.Primary" OnClick="ButtonOnClick">@ButtonText</Button>

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
