using ElevatorConsoleApp.Helpers;
using Spectre.Console;
using ElevatorConsoleApp.Services;

namespace ElevatorConsoleApp.Pages;
internal class GeneratePage : IPage
{
    private readonly IGeneratorService _generator;
    private readonly IDatabaseService _databaseService;

    public GeneratePage(IGeneratorService generator, IDatabaseService databaseService)
    {
        _generator = generator;
        _databaseService = databaseService;
    }

    /// <summary>
    /// Generates Elevators and posts them to api
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task DoWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Console.Clear();
            var amount = AnsiConsole.Prompt(
                 new TextPrompt<int>("How many [green]Elevators[/] do you want to create?")
                     //.PromptStyle("green")
                     .ValidationErrorMessage("[red]That's not a valid amount[/]")
                     .Validate(age =>
                     {
                         return age switch
                         {
                             < 1 => ValidationResult.Error("[red]You must create at least 1 elevator[/]"),
                             > 50 => ValidationResult.Error("[red]You can create max 50 elevators[/]"),
                             _ => ValidationResult.Success(),
                         };
                     }));

            var listOfElevators = AnsiConsole.Status()
                 .Start("Generating...", ctx =>
                 {
                     AnsiConsole.MarkupLine("Generating addresses...");
                     Thread.Sleep(100);

                     var elevators = _generator.Generate(amount)
                                ?? throw new ArgumentException(nameof(GeneratorService));

                     ctx.Status("Generating Citys...");
                     ctx.Spinner(Spinner.Known.Star);
                     ctx.SpinnerStyle(Style.Parse("green"));

                     AnsiConsole.MarkupLine("Generating Zipcodes...");
                     Thread.Sleep(100);

                     AnsiConsole.MarkupLine("Generating Table...");

                     var fakerElevators = elevators.ToList();

                     AnsiConsole.Write(TableHelper.GenerateTable(fakerElevators));

                     return fakerElevators;
                 });

            if (AnsiConsole.Confirm("Do you want to save these to database?"))
            {
                foreach (var elevator in listOfElevators)
                {
                    await _databaseService.InsertAsync(elevator);
                }
            }

            break;
        }
    }
}
