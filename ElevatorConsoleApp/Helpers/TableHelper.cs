using ElevatorConsoleApp.Models;
using Spectre.Console;

namespace ElevatorConsoleApp.Helpers;
internal static class TableHelper
{
    public static Table GenerateTable(IEnumerable<Elevator> elevators)
    {
        var table = new Table();

        table.AddColumn("Id");
        table.AddColumn("Floors");
        table.AddColumn("Address");
        table.AddColumn("ElevatorId");
        table.AddColumn("ConnectionString");

        // ReSharper disable once PossibleMultipleEnumeration

        foreach (var elevator in elevators)
        {
            table.AddRow(elevator.Id.ToString(),elevator.NumberOfFloors.ToString(), elevator.Location, elevator.ElevatorId ?? "NULL", elevator.ConnectionString ?? "NULL");
        }

        return table;
    }
}
