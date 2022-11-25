using ElevatorConsoleApp.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Spectre.Console;


namespace ElevatorConsoleApp.Services;
internal interface IElevatorService
{
    public Task InitializeElevatorAsync(Elevator elevator, CancellationToken cancellationToken, string? id = null);
}

internal class ElevatorService : IElevatorService
{
    private readonly IApiService _apiService;
    private Elevator _elevator;
    private bool _doorsIsOpen = false;



    public ElevatorService(IApiService apiService)
    {
        _apiService = apiService;
    }



    public async Task InitializeElevatorAsync(Elevator elevator, CancellationToken cancellationToken, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(elevator.ConnectionString))
        {
            var response = await _apiService.RegisterElevatorAsync(new ElevatorRequest()
            {
                Id = elevator.ElevatorId,
                Location = elevator.Location,
                NumberOfFloors = elevator.NumberOfFloors

            }, cancellationToken)
                               ?? throw new ArgumentNullException(nameof(ElevatorResponse));
            elevator.ElevatorId = response.Id;
            elevator.Location = response.Location;
            elevator.ConnectionString = response.ConnectionString;
            elevator.NumberOfFloors = response.NumberOfFloors;
        }


        //TODO Kolla om den finns i databas    


        _elevator = elevator;
        var deviceClient = DeviceClient.CreateFromConnectionString(elevator.ConnectionString,
            options: new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset },
            transportType: TransportType.Mqtt);



        await ConnectElevatorAsync(cancellationToken);
    }


    public async Task ConnectElevatorAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));


            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                Console.WriteLine($"Ping from: {_elevator.Location} : {DateTime.Now}");
            }

            await Task.Delay(10000, cancellationToken);
        }
    }

}