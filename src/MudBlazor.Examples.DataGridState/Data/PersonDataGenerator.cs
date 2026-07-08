using Bogus;

namespace MudBlazor.Examples.DataGridState.Data;

public static class PersonDataGenerator
{
    private const int Seed = 8675309;
    private static readonly DateTime ReferenceDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<Person> CreatePeople(int count = 250)
    {
        var faker = new Faker<Person>()
            .UseSeed(Seed)
            .RuleFor(person => person.Id, faker => faker.Random.Guid())
            .RuleFor(person => person.FullName, faker => faker.Name.FullName())
            .RuleFor(person => person.Email, (faker, person) => faker.Internet.Email(person.FullName))
            .RuleFor(person => person.Company, faker => faker.Company.CompanyName())
            .RuleFor(person => person.City, faker => faker.Address.City())
            .RuleFor(person => person.Age, faker => faker.Random.Int(22, 65))
            .RuleFor(person => person.Salary, faker => faker.Random.Decimal(45000, 160000))
            .RuleFor(person => person.Joined, faker => faker.Date.PastOffset(8, ReferenceDate).DateTime);

        return faker.Generate(count);
    }
}
