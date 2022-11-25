namespace ElevatorConsoleApp.Models;

public class ElevatorResponse
{
    public string Id { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int NumberOfFloors { get; set; } = 5;
    public string? ConnectionString { get; set; }
}