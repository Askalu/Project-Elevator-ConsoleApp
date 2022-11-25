using ElevatorConsoleApp.Helpers;
using ElevatorConsoleApp.Pages;
using ElevatorConsoleApp.Services;
using Microsoft.Extensions.Hosting;
using Spectre.Console;


namespace ElevatorConsoleApp;

internal class HostedService : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IDatabaseService _databaseService;
    private readonly GeneratePage _generatePage;
    private readonly ImportApiPage _importApiPage;
    private readonly ElevatorPage _elevatorPage;

    public HostedService(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime,
        IDatabaseService databaseService,
        GeneratePage generatePage,
        ImportApiPage importApiPage,
        ElevatorPage elevatorPage)
    {
        _applicationLifetime = applicationLifetime;
        _databaseService = databaseService;
        _generatePage = generatePage;
        _importApiPage = importApiPage;
        _elevatorPage = elevatorPage;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _databaseService.BootstrapDatabaseAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();

            var inp = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .Mode(SelectionMode.Independent)
                    .AddChoices("Run Elevators", "List DB", "Import from API", "Generate Elevators", "Quit"));

            switch (inp.ToLower())
            {
                case "generate elevators":
                    await _generatePage.DoWork(stoppingToken);
                    break;
                case "import from api":
                    await _importApiPage.DoWork(stoppingToken);
                    AnsiConsole.Write("Enter to continue...");
                    Console.Read();
                    break;
                case "run elevators":
                    await _elevatorPage.DoWork(stoppingToken);
                    break;
                case "list db":
                    var elevators = await _databaseService.GetAllAsync();

                    if (elevators is null)
                    {
                        AnsiConsole.WriteLine("DB is empty");
                        AnsiConsole.Write("Enter to continue...");
                        Console.Read();
                        break;
                    }

                    AnsiConsole.Write(TableHelper.GenerateTable(elevators));
                    AnsiConsole.Write("Enter to continue...");
                    Console.Read();
                    break;
                case "quit":
                    if (AnsiConsole.Confirm("Are you sure you want to quit?"))
                        _applicationLifetime.StopApplication();
                    break;
                default:
                    break;
            }
        }
    }
}