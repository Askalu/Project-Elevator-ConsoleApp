using Newtonsoft.Json;

namespace ElevatorConsoleApp.Models;

public class ElevatorResponse
{
    [JsonProperty("id")]
    public string ElevatorId { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int NumberOfFloors { get; set; } = 5;
    public string? ConnectionString { get; set; }
}