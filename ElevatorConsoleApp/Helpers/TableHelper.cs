using ElevatorConsoleApp.Models;
using Spectre.Console;

namespace ElevatorConsoleApp.Helpers;
internal static class TableHelper
{
    public static Table GenerateTable(IEnumerable<Elevator> elevators)
    {
        var table = new Table();

        table.AddColumn("Floors");
        table.AddColumn("Address");
        table.AddColumn("Id");
        table.AddColumn("ConnectionString");

        // ReSharper disable once PossibleMultipleEnumeration

        foreach (var elevator in elevators)
        {
            table.AddRow(elevator.NumberOfFloors.ToString(), elevator.Location, elevator.ElevatorId ?? "NULL", elevator.ConnectionString ?? "NULL");
        }

        return table;
    }
}
