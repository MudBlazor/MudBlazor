#nullable enable

using System.Collections.Generic;

public class Company
{
    /// <summary>
    /// Gets or sets the department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the team.
    /// </summary>
    public string? Team { get; set; }

    /// <summary>
    /// Gets or sets the role.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the employee name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the score.
    /// </summary>
    public int Score { get; set; }

    public static IReadOnlyCollection<Company> GetRecords() =>
    [
        new() { Department = "Engineering", Team = "Platform", Role = "Backend", Name = "Alice", Score = 120 },
        new() { Department = "Engineering", Team = "Platform", Role = "Backend", Name = "Bob", Score = 115 },
        new() { Department = "Engineering", Team = "Platform", Role = "Frontend", Name = "Carol", Score = 110 },
        new() { Department = "Engineering", Team = "Platform", Role = "Frontend", Name = "Dave", Score = 108 },
        new() { Department = "Engineering", Team = "Data", Role = "Analytics", Name = "Eve", Score = 105 },
        new() { Department = "Engineering", Team = "Data", Role = "Analytics", Name = "Frank", Score = 102 },
        new() { Department = "Sales", Team = "Enterprise", Role = "Account Exec", Name = "Grace", Score = 150 },
        new() { Department = "Sales", Team = "Enterprise", Role = "Account Exec", Name = "Henry", Score = 145 },
        new() { Department = "Sales", Team = "SMB", Role = "Account Exec", Name = "Ivy", Score = 130 },
        new() { Department = "Sales", Team = "SMB", Role = "Sales Dev", Name = "Jack", Score = 125 },
        new() { Department = "Support", Team = "Tier1", Role = "Agent", Name = "Kelly", Score = 95 },
        new() { Department = "Support", Team = "Tier2", Role = "Agent", Name = "Liam", Score = 100 }
    ];
}
