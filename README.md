
# MudBlazor
Blazor Component Library based on material-ui design. This is learning project for me and i wanted to change some stuff other librarys out there provided design vise.
I want to make it easier to change the design/style and my goal is as little JS as possible.

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
#### Client Side (WebAssembly)
In `index.html` head section
```
<link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
```

#### Server Side
In `_Host.cshtml` head section
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
