using ElevatorConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorConsoleApp.Pages;
internal class ElevatorPage : IPage
{
    private readonly IDatabaseService _databaseService;
    private readonly IElevatorService _elevatorService;
    private readonly IServiceProvider _serviceProvider;

    public ElevatorPage(IDatabaseService databaseService, IElevatorService elevatorService,IServiceProvider serviceProvider)
    {
        _databaseService = databaseService;
        _elevatorService = elevatorService;
        _serviceProvider = serviceProvider;
    }
    public async Task DoWork(CancellationToken cancellationToken)
    {

        var elevators = await _databaseService.GetAllAsync();

        elevators?.ForEach(x =>
        {
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                await scope.ServiceProvider.GetService<IElevatorService>()
                    ?.InitializeElevatorAsync(x, cancellationToken)!;
            }, cancellationToken).ConfigureAwait(false);
        });

        while (!cancellationToken.IsCancellationRequested)
        {

        }
    }
}
