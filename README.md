# ![MudBlazor](content/MudBlazor-GitHub.png)
# Material Design components for Blazor
![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/gardnet-nu/4cba3d30-858f-4653-a80d-736a8adc5daf/1/master?label=azure%20pipelines&style=flat-square)
[![GitHub](https://img.shields.io/github/license/garderoben/mudblazor?color=%23594ae2&style=flat-square)](https://github.com/Garderoben/MudBlazor/blob/master/LICENSE)
[![Gitter](https://img.shields.io/gitter/room/MudBlazor/community?style=flat-square)](https://gitter.im/MudBlazor/community)
[![GitHub Repo stars](https://img.shields.io/github/stars/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor/stargazers)
[![GitHub last commit](https://img.shields.io/github/last-commit/garderoben/mudblazor?style=flat-square)](https://github.com/Garderoben/MudBlazor)

Component Library based on material-ui and Material design. The goal is to do more Blazor, CSS and less Javascript.
Simple and easy to use components that is highly customizable.

## Documentation & Demo
- [MudBlazor.com](https://mudblazor.com)


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
