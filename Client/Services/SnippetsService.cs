namespace BlazorRepl.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class SnippetsService
    {
        private const int SnippetIdCharsCount = 18;
        private static readonly IDictionary<char, char> LetterToDigitIdMappings = new Dictionary<char, char>
        {
            ['a'] = '0',
            ['k'] = '0',
            ['u'] = '0',
            ['E'] = '0',
            ['O'] = '0',
            ['Y'] = '0',
            ['b'] = '1',
            ['l'] = '1',
            ['v'] = '1',
            ['F'] = '1',
            ['P'] = '1',
            ['c'] = '2',
            ['m'] = '2',
            ['w'] = '2',
            ['G'] = '2',
            ['Q'] = '2',
            ['d'] = '3',
            ['n'] = '3',
            ['x'] = '3',
            ['H'] = '3',
            ['R'] = '3',
            ['e'] = '4',
            ['o'] = '4',
            ['y'] = '4',
            ['I'] = '4',
            ['S'] = '4',
            ['f'] = '5',
            ['p'] = '5',
            ['z'] = '5',
            ['J'] = '5',
            ['T'] = '5',
            ['g'] = '6',
            ['q'] = '6',
            ['A'] = '6',
            ['K'] = '6',
            ['U'] = '6',
            ['h'] = '7',
            ['r'] = '7',
            ['B'] = '7',
            ['L'] = '7',
            ['V'] = '7',
            ['i'] = '8',
            ['s'] = '8',
            ['C'] = '8',
            ['M'] = '8',
            ['W'] = '8',
            ['j'] = '9',
            ['t'] = '9',
            ['D'] = '9',
            ['N'] = '9',
            ['X'] = '9',
            ['Z'] = '9',
        };

        private readonly HttpClient httpClient;

        public SnippetsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetSnippetContentAsync(string snippetId)
        {
            if (string.IsNullOrWhiteSpace(snippetId) || snippetId.Length != SnippetIdCharsCount)
            {
                throw new ArgumentException("Invalid snippet ID", nameof(snippetId));
            }

            var yearFolder = DecodeDateIdPart(snippetId.Substring(0, 2));
            var monthFolder = DecodeDateIdPart(snippetId.Substring(2, 2));
            var dayAndHourFolder = DecodeDateIdPart(snippetId.Substring(4, 4));

            var id = snippetId.Substring(8);
            var snippetContent = await this.httpClient.GetStringAsync(
                $"https://blazorrepl.blob.core.windows.net/snippets-staging/{yearFolder}/{monthFolder}/{dayAndHourFolder}/{id}.txt"); // TODO: Env var

            return snippetContent;
        }

        private static string DecodeDateIdPart(string encodedPart)
        {
            var decodedPart = string.Empty;

            foreach (var letter in encodedPart)
            {
                if (!LetterToDigitIdMappings.TryGetValue(letter, out var digit))
                {
                    throw new InvalidDataException("Invalid snippet ID");
                }

                decodedPart += digit;
            }

            return decodedPart;
        }
    }
}
