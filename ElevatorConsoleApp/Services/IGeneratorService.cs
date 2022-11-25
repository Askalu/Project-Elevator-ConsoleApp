using Bogus;
using ElevatorConsoleApp.Models;

namespace ElevatorConsoleApp.Services;
internal interface IGeneratorService
{
    /// <summary>
    /// Generates fake elevators
    /// </summary>
    /// <param name="amount">Amount of generators to create must be between 1 - 50</param>
    /// <returns>IEnumerable of Elevator</returns>
    public IEnumerable<Elevator>? Generate(int amount);
}

internal class GeneratorService : IGeneratorService
{

    public IEnumerable<Elevator>? Generate(int amount)
    {
        if (amount is < 1 or > 50)
            return null;

        var elevators = new List<Elevator>();

        for (var i = 0; i < amount; i++)
        {
            elevators.Add(new Faker<Elevator>("sv")
                .RuleFor(x => x.Location, y => $"{y.Address.StreetAddress()}, {y.Address.City()} {y.Address.ZipCode()}")
                .RuleFor(x => x.NumberOfFloors, y => y.Random.Int(2, 20)));
        }

        return elevators;
    }
}
