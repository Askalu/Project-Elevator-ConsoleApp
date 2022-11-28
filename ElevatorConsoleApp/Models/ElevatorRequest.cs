namespace ElevatorConsoleApp.Models;

internal class ElevatorRequest
{
    public string? Id { get; set; }
    public string Location { get; set; } = null!;
    public int NumberOfFloors { get; set; }
}