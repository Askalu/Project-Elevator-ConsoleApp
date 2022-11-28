using AutoMapper;
using ElevatorConsoleApp.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace ElevatorConsoleApp.Services;

internal interface IElevatorService
{
    public Task InitializeElevatorAsync(Elevator elevator, CancellationToken cancellationToken, string? id = null);
}

internal class ElevatorService : IElevatorService
{
    private readonly IApiService _apiService;
    private readonly IDatabaseService _databaseService;
    private readonly IMapper _mapper;
    private Elevator? _elevator;
    private DeviceClient? _deviceClient;


    public ElevatorService(IApiService apiService, IDatabaseService databaseService, IMapper mapper)
    {
        _apiService = apiService;
        _databaseService = databaseService;
        _mapper = mapper;
    }


    public async Task InitializeElevatorAsync(Elevator elevator, CancellationToken cancellationToken, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(elevator.ConnectionString))
        {
            var response =
                await _apiService.RegisterElevatorAsync(_mapper.Map<ElevatorRequest>(elevator), cancellationToken)
                ?? throw new ArgumentException(nameof(ElevatorResponse));

            elevator = _mapper.Map(response, elevator);

            await _databaseService.Update(elevator);
        }

        _elevator = elevator;
        _deviceClient = DeviceClient.CreateFromConnectionString(elevator.ConnectionString,
            options: new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset },
            transportType: TransportType.Mqtt);


        await ConnectElevatorAsync(cancellationToken);
    }


    private async Task ConnectElevatorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {

                Console.WriteLine($"Ping from: {_elevator?.Location} : {DateTime.Now}");
            }
        }
    }
}