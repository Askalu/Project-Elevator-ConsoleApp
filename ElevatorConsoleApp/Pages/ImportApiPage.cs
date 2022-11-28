using ElevatorConsoleApp.Helpers;
using ElevatorConsoleApp.Models;
using ElevatorConsoleApp.Services;
using Spectre.Console;

namespace ElevatorConsoleApp.Pages;
internal class ImportApiPage : IPage
{
    private readonly IApiService _apiService;
    private readonly IDatabaseService _databaseService;

    public ImportApiPage(IApiService apiService, IDatabaseService databaseService)
    {
        _apiService = apiService;
        _databaseService = databaseService;
    }
    public async Task DoWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            AnsiConsole.WriteLine("Import from api should only be used if the local database is empty");
            if (!AnsiConsole.Confirm("Are you sure you want to import from api?", false))
                break;

            AnsiConsole.Clear();

            await AnsiConsole.Status()
                        .Start("Loading elevators...", async ctx =>
                        {
                            AnsiConsole.MarkupLine("Loading from api...");

                            var elevators = await _apiService.ImportElevatorsFromApiAsync(cancellationToken);
                            if (elevators is null)
                                return null;
                            AnsiConsole.MarkupLine("Saving in database...");

                            foreach (var elevator in elevators)
                            {
                                await _databaseService.InsertAsync(new Elevator()
                                {
                                    ElevatorId = elevator.ElevatorId,
                                    Location = elevator.Location,
                                    NumberOfFloors = elevator.NumberOfFloors
                                });
                            }

                            AnsiConsole.MarkupLine("Generating Table...");
                            var newElevators = await _databaseService.GetAllAsync();

                            if (newElevators is null)
                                return ctx;

                            AnsiConsole.Write(TableHelper.GenerateTable(newElevators));
                            return ctx;
                        });
            break;
        }
    }
}
