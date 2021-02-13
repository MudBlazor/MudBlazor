using System;
using System.Net.Http;
using System.Text;
using RichardSzalay.MockHttp;

namespace MudBlazor.UnitTests.Mocks
{
    public static class MockHttpHelper
    {
        public static HttpClient GetMockHttpClient()
        {
            return new HttpClient(GetMockHttpMessageHandler()) { BaseAddress = new Uri("https://localhost/") };
        }

        private static HttpMessageHandler GetMockHttpMessageHandler()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            // Periodic table api
            mockHttpMessageHandler.When("https://localhost/webapi/periodictable")
                .Respond("application/json",
                Encoding.UTF8.GetString(Encoding.Default.GetBytes(SampleElementsJson)));
            // DialogScrollableExample
            mockHttpMessageHandler.When("https://raw.githubusercontent.com/Garderoben/MudBlazor/master/LICENSE")
                .Respond("text/plain; charset=utf-8", "Dummy Licence");
            return mockHttpMessageHandler;
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
