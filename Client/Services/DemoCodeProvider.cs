namespace BlazorRepl.Client.Services
{
    using System.Collections;
    using System.Collections.Generic;

    public static class DemoCodeProvider
    {
        public static readonly IDictionary<int, string> DemoCodeMapping = new Dictionary<int, string>()
        {
            {
                1,
                @"<h1>Counter Demo</h1>

<button class=""btn btn-primary"" @onclick=""@Increment"">Increment</button>

<div>Counter: @Counter</div>

@code
        {
    public int Counter { get; set; }

        public void Increment()
        {
            if (Counter < 10)
            {
                Counter++;
            }
            else if (Counter < 100)
            {
                Counter += 10;
            }
            else
            {
                Counter += 100;
            }
        }
    }"
            },
            {
                2,
                @"@inject HttpClient Http

<h1>HTTP Demo</h1>

<button class=""btn btn-primary"" @onclick=""CallHttp"">Click me</button>

<div>Words:</div>
<ul>
    @foreach (var word in Words)
    {
        <li>@word</li>
    }
</ul>

@code {
    public IEnumerable<string> Words { get; set; } = Enumerable.Empty<string>();

        private async Task CallHttp()
        {
            var result = await Http.GetAsync(""https://random-word-api.herokuapp.com/word?number=5"");

            Words = await result.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }
    }"
            },
            {
                3,
                @"<h2>New Ship Entry Form</h2>

<EditForm Model=""@StarshipModel"" OnValidSubmit=""@OnSubmit"">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class=""form-group"">
        <label for=""identifier"">Identifier:</label>
        <InputText @bind-Value=""StarshipModel.Identifier"" class=""form-control"" id=""identifier""/>
    </div>
    <div class=""form-group"">
        <label for=""desc"">Description(optional) :</label>
            <InputTextArea @bind-Value=""StarshipModel.Description"" class=""form-control"" id=""desc""/>
    </div>
    <div class=""form-group"">
        <label for=""classification"">Primary Classification:</label>
        <InputSelect @bind-Value=""StarshipModel.Classification"" class=""form-control"" id=""classification"">
            <option value = """" > Select classification...</option>
            <option value = ""Exploration"" > Exploration </option>
            <option value=""Diplomacy"">Diplomacy</option>
            <option value = ""Defense"" > Defense </option>
        </InputSelect>
    </div>
    <div class=""form-group"">
        <label for=""accommodation"">Maximum Accommodation:</label>
        <InputNumber @bind-Value=""StarshipModel.MaximumAccommodation"" class=""form-control"" id=""accommodation""/>
    </div>
    <div class=""form-group"">
        <label for=""is-approved"">Engineering Approval:</label>
        <InputCheckbox @bind-Value=""StarshipModel.IsValidatedDesign"" class=""form-control"" id=""is-approved""/>
    </div>
    <div class=""form-group"">
        <label for=""production-date"">Production Date:</label>
        <InputDate @bind-Value=""StarshipModel.ProductionDate"" class=""form-control"" id=""production-date""/>
    </div>

    <button type = ""submit"" class=""btn btn-primary"">Submit</button>
</EditForm>

<p>@FormMessage</p>

@code
        {
    public Starship StarshipModel { get; set; } = new Starship();
        public string FormMessage { get; set; }
        private async Task OnSubmit()
        {
            FormMessage = ""You successfully submitted the form"";
            StarshipModel = new Starship();
            await Task.Delay(3000);
            FormMessage = string.Empty;
        }

        public class Starship
        {
            [Required]
            [StringLength(16, ErrorMessage = ""Identifier too long (16 character limit)."")]
            public string Identifier { get; set; }

            public string Description { get; set; }

            [Required]
            public string Classification { get; set; }

            [Range(1, 100000, ErrorMessage = ""Accommodation invalid (1-100000)."")]
            public int MaximumAccommodation { get; set; }

            [Required]
            [Range(typeof(bool), ""true"", ""true"",
                ErrorMessage = ""This form disallows unapproved ships."")]
            public bool IsValidatedDesign { get; set; }

            [Required]
            public DateTime ProductionDate { get; set; }
        }
    }"
            },
        };
        }
}
