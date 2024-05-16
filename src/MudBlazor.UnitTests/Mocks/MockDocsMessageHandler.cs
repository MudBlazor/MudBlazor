using System.Text;
using RichardSzalay.MockHttp;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockDocsMessageHandler : MockHttpMessageHandler
    {
        public MockDocsMessageHandler()
        {
            // Periodic table api
            this.When("https://localhost/webapi/periodictable")
                .Respond("application/json",
                Encoding.UTF8.GetString(Encoding.Default.GetBytes(SampleElementsJson)));
            // DialogScrollableExample
            this.When("https://raw.githubusercontent.com/MudBlazor/MudBlazor/master/LICENSE")
                .Respond("text/plain", "Dummy License");
        }

        private const string SampleElementsJson = @"
        [
            {
                ""group"": """",
                ""position"": 0,
                ""name"": ""Hydrogen"",
                ""number"": 1,
                ""small"": ""H"",
                ""molar"": 1.00794,
                ""electrons"": [
                1
                ]
            },
            {
                ""group"": ""Element Noble p"",
                ""position"": 17,
                ""name"": ""Helium"",
                ""number"": 2,
                ""small"": ""He"",
                ""molar"": 4.002602,
                ""electrons"": [
                2
                ]
            }
        ]";
    }
}
