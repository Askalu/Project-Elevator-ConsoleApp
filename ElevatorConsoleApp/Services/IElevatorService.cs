using ElevatorConsoleApp.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;

namespace ElevatorConsoleApp.Services;
internal interface IElevatorService
{
    public Task ConnectElevatorAsync();
    public Task InitializeElevatorAsync(string location, int numberOfFloors, string? id = null);
    public Task DisconnectElevatorAsync();
}

internal class ElevatorService : IElevatorService
{
    private readonly IApiService _apiService;
    private ElevatorResponse _elevator;

    public ElevatorService(IApiService apiService)
    {
        _apiService = apiService;
    }



    public async Task InitializeElevatorAsync(string location, int numberOfFloors, string? id = null)
    {
        if (string.IsNullOrWhiteSpace(location) || location.Length is > 200 or < 2 || numberOfFloors is < 2 or > 20)
            throw new ArgumentNullException(nameof(location), nameof(numberOfFloors));



        //TODO Kolla om den finns i databas    

        var elevator = await _apiService.RegisterElevatorAsync(new ElevatorRequest(location, numberOfFloors, id))
                       ?? throw new ArgumentNullException(nameof(ElevatorResponse));

        _elevator = elevator;

        var deviceClient = DeviceClient.CreateFromConnectionString(elevator.ConnectionString,
            options: new ClientOptions { SdkAssignsMessageId = SdkAssignsMessageId.WhenUnset },
            transportType: TransportType.Mqtt);


        await deviceClient.SetMethodHandlerAsync("CurrentCheck", TestMethodHandler, null);

        await ConnectElevatorAsync();
    }

    private Task<MethodResponse> TestMethodHandler(MethodRequest methodrequest, object usercontext)
    {
        var data = Encoding.UTF8.GetString(methodrequest.Data);
        var result = "{\"result\":\"Invalid parameter\"}";
        Console.WriteLine("FROM: " + _elevator.Id);

        if (!bool.TryParse(data, out var sendingState))
        {
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }



        result = "{\"result\":\"Executed method\"}";
        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
    }


    public async Task ConnectElevatorAsync()
    {
        while (true)
        {
            Console.WriteLine("from connectElevator");
            await Task.Delay(10000);
        }
    }


    public Task DisconnectElevatorAsync()
    {
        throw new NotImplementedException();
    }

}