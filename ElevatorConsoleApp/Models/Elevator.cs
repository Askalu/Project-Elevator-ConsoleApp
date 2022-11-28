using Dapper.Contrib.Extensions;

namespace ElevatorConsoleApp.Models;
internal class Elevator
{
    [Key]
    public int Id { get; set; }
    public string? ElevatorId { get; set; }
    public string Location { get; set; } = null!;
    public int NumberOfFloors { get; set; }
    public string? ConnectionString { get; set; }
}
