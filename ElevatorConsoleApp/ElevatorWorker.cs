using ElevatorConsoleApp.Services;
using Microsoft.Extensions.Hosting;

namespace ElevatorConsoleApp;

internal class ElevatorWorker : IHostedService
{
    private readonly IElevatorService _elevatorService;


    public ElevatorWorker(IElevatorService elevatorService)
    {
        _elevatorService = elevatorService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Intializing application...");

        Task.Run(async () =>
        {
            await Task.Delay(2000, cancellationToken);
            await InitializeWorker(cancellationToken);

        }, cancellationToken).ConfigureAwait(false);

        return Task.CompletedTask;
    }

    private async Task InitializeWorker(CancellationToken cancellationToken)
    {

        Task.Run(async () =>
        {
            await _elevatorService.InitializeElevatorAsync("test", 5, "e5cc355b-79be-4596-fc85-08dace4026d0");

        }).ConfigureAwait(false);

        Task.Run(async () =>
        {
            await _elevatorService.InitializeElevatorAsync("test", 5, "f1f67b86-7d9e-4daf-fc83-08dace4026d0");

        }).ConfigureAwait(false);
        while (!cancellationToken.IsCancellationRequested)
        {
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}