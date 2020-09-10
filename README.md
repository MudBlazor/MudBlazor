# ![MudBlazor](content/MudBlazor-GitHub.png)
# Material Design components for Blazor
Component Library based on material-ui and Material design. The goal is to do more Blazor, CSS and less Javascript.
Simple and easy to use components that is highly customizable.


## Documentation & Demo
- [MudBlazor.com](https://mudblazor.com)


## Prerequisites

- .NET Core 3.1
- Visual Studio 2019 with the ASP.NET and Web development.


## Installation 

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
