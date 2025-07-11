// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models
{
    /// <summary>
    /// The content for the team page
    /// </summary>
    /// <remarks>
    /// Data is taken from GitHub and updated manually. In the future this could be automated.
    /// </remarks>
    public class TeamMemberData
    {
        public static TeamMember[] GetCoreTeam() =>
        [
            new TeamMember
            {
                Name = "Jonny Larsson (Creator)",
                From = "Örebro, Sweden",
                GitHub = "Garderoben",
                LinkedIn = "https://www.linkedin.com/in/jonny-larsson-b72480161/",
                Bio = "DevOps Engineer.\r\nRecently been doing more developing in free time and for in house applications for my company to automate processes.",
            },
            new TeamMember
            {
                Name = "Meinrad Recheis (Co-Creator)",
                From = "Vienna, Austria",
                GitHub = "henon",
                LinkedIn = "https://www.linkedin.com/in/meinrad-recheis-6a9885171/",
                Bio = "Entrepreneur and Open Source contributor. Co-founder of MudBlazor. Creator of Numpy.NET and Python.Included. ",
            },
            new TeamMember
            {
                Name = "András Tangl",
                From = "Szombathely, Hungary",
                GitHub = "tungi52",
                LinkedIn = "https://www.linkedin.com/in/andr%C3%A1s-tangl-683a20215/",
                Bio = null,
            },
            new TeamMember
            {
                Name = "Mike Surcouf",
                From = "Jersey, Channel Islands",
                GitHub = "mikes-gh",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Benjamin Kappel",
                From = "Malaysia",
                GitHub = "just-the-benno",
                LinkedIn = "https://www.linkedin.com/in/benjamin-kappel-558428168/",
                Bio = "A long time ago, there was an enormous book with an even more significant discount on it, titled C#. That is the starting point of still ongoing love.",
            },
            new TeamMember
            {
                Name = "Jonas B.",
                From = "Germany",
                GitHub = "JonBunator",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Riley Nielsen",
                From = "Minnesota, United States",
                GitHub = "Mr-Technician",
                LinkedIn = "https://www.linkedin.com/in/riley-nielsen-a57399223/",
                Bio = null,
            },
            new TeamMember
            {
                Name = "Artyom M.",
                From = "Estonia, Tallinn",
                GitHub = "ScarletKuro",
                GitHubSponsor = true,
                LinkedIn = "https://www.linkedin.com/in/artyommelnikov/",
                Bio = ".NET/Java developer",
            },
            new TeamMember
            {
                Name = "Daniel Chalmers",
                From = "Texas, United States",
                GitHub = "danielchalmers",
                LinkedIn = "https://www.linkedin.com/in/daniel-c-5799252b1",
                Bio = "Full stack engineer specializing in .NET, Blazor, CSS, WPF. Building accessible apps with a focus on modern UI/UX",
            },
        ];

        public static TeamMember[] GetContributionTeam() =>
        [
            new TeamMember
            {
                Name = "Henrique Clausing",
                From = "Brazil",
                GitHub = "HClausing",
                LinkedIn = "https://www.linkedin.com/in/henrique-clausing-cunha-45085944/",
                Bio = "Accountant and Full-Stack .NET developer. Founder of Eficaz Sistemas.",
            },
            new TeamMember
            {
                Name = "Porkopek",
                From = "Fundão, Portugal",
                GitHub = "porkopek",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Mehmet Can Karagöz",
                From = "Alanya, Turkey",
                GitHub = "mckaragoz",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Jon Person",
                From = "Denver, CO",
                GitHub = "jperson2000",
                LinkedIn = null,
                Bio = "I'm a full-stack C# developer for the U.S. Department of Defense who specializes in Blazor, durable back-end systems, and reusable frameworks.",
            },
            new TeamMember
            {
                Name = "Lukas Klinger",
                From = "Germany",
                GitHub = "Flaflo",
                LinkedIn = null,
                Bio = ".NET & Java Full-Stack Developer",
            },
            new TeamMember
            {
                Name = "Jason Rebelo",
                From = "Luxembourg",
                GitHub = "igotinfected",
                GitHubSponsor = true,
                LinkedIn = null,
                Bio = "Web & Mobile Analyst Developer with a faible for Open Source and Cybersecurity.",
            },
            new TeamMember
            {
                Name = "Samuel Meenzen",
                From = "Germany",
                GitHub = "meenzen",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Justin Lampe",
                From = null,
                GitHub = "xC0dex",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Roman Alvarez",
                From = "Uruguay",
                GitHub = "ralvarezing",
                LinkedIn = null,
                Bio = "DevOps at SONDA & Computing Engineering student.",
            },
            new TeamMember
            {
                Name = "Versile Johnson II",
                From = "Texas, United States",
                GitHub = "versile2",
                GitHubSponsor = true,
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "Anu6is",
                From = "Trinidad & Tobago",
                GitHub = "anu6is",
                LinkedIn = null,
                Bio = null,
            },
            new TeamMember
            {
                Name = "digitaldirk",
                From = "the woods",
                GitHub = "digitaldirk",
                LinkedIn = null,
                Bio = "C# and game things",
            },
        ];
    }
}
