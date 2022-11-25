using ElevatorConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElevatorConsoleApp;

internal class HostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public HostedService(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWorkAsync(stoppingToken);
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        Task.Run(async () =>
        {
            var service = scope.ServiceProvider.GetService<IElevatorService>();
            await service.InitializeElevatorAsync("test", 5, "f1f67b86-7d9e-4daf-fc83-08dace4026d0");
        }, stoppingToken).ConfigureAwait(false);

        Task.Run(async () =>
        {
            var service = scope.ServiceProvider.GetService<IElevatorService>();
            await service.InitializeElevatorAsync("test", 5, "3b491759-c60b-406b-fc86-08dace4026d0");
        }, stoppingToken).ConfigureAwait(false);

        while (true)
        {
        }

    }
}