namespace ElevatorConsoleApp.Models;

internal class ElevatorRequest
{
    public ElevatorRequest()
    {

    }
    public ElevatorRequest(string location, int numberOfFloors = 5, string? id = null)
    {
        Id = id;
        Location = location;
        NumberOfFloors = numberOfFloors;
    }
    public string? Id { get; set; }
    public string Location { get; set; } = null!;
    public int NumberOfFloors { get; set; }
}