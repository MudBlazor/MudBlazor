namespace MudBlazor.Examples.DataGridState.Data;

public sealed class Person
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Company { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public int Age { get; init; }

    public decimal Salary { get; init; }

    public DateTime Joined { get; init; }
}
