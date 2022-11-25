using System.Text;
using ElevatorConsoleApp.Models;
using ElevatorConsoleApp.Responses;
using Newtonsoft.Json;

namespace ElevatorConsoleApp.Services;

internal interface IApiService
{
    public Task<ElevatorResponse?> RegisterElevatorAsync(ElevatorRequest request,
        CancellationToken cancellationToken);

    public Task<IEnumerable<ElevatorResponse>?> ImportElevatorsFromApiAsync(CancellationToken cancellationToken);
}

internal class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }




    public async Task<ElevatorResponse?> RegisterElevatorAsync(ElevatorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient("APIClient");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "devices/connect")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            Console.WriteLine($"Sending HTTP Request for elevator: {request.Location}");


            await Task.Delay(1000, cancellationToken);

            var httpResponse = await client.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                return null;

            var response =
                JsonConvert.DeserializeObject<HttpResponse<ElevatorResponse>>(
                    await httpResponse.Content.ReadAsStringAsync(cancellationToken));

            return response?.Data;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<ElevatorResponse>?> ImportElevatorsFromApiAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient("APIClient");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "elevators/ids");

            var httpResponse = await client.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                return Enumerable.Empty<ElevatorResponse>();

            var response =
                JsonConvert.DeserializeObject<HttpResponse<IEnumerable<ElevatorResponse>>>(
                    await httpResponse.Content.ReadAsStringAsync(cancellationToken));

            return response?.Data;
        }
        catch
        {
            return Enumerable.Empty<ElevatorResponse>();
        }
    }
}