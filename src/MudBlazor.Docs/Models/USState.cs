#nullable enable

namespace MudBlazor.Examples.Data.Models;

public class USState
{
    /// <summary>
    /// Gets or sets the numeric identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the state name.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the number of counties.
    /// </summary>
    public int Counties { get; set; }

    /// <summary>
    /// Gets or sets the population.
    /// </summary>
    public int Population { get; set; }

    /// <summary>
    /// Gets or sets the primary industry.
    /// </summary>
    public string? PrimaryIndustry { get; set; }

    /// <summary>
    /// Gets or sets the geographic region.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the climate classification.
    /// </summary>
    public string? Climate { get; set; }

    /// <summary>
    /// Gets or sets the year inducted.
    /// </summary>
    public int YearInducted { get; set; }
}
