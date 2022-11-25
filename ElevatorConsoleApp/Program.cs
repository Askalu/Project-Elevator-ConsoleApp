using System.Net.Http.Headers;
using ElevatorConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElevatorConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Information))
            .ConfigureServices((_, services) => services.ConfigureServices());

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpClient("APIClient", opt =>
        {
            opt.BaseAddress = new Uri("https://localhost:7016/api/");
            opt.DefaultRequestHeaders.Accept.Clear();
            opt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddTransient<IApiService, ApiService>();
        services.AddScoped<IElevatorService, ElevatorService>();

        services.AddHostedService<ElevatorWorker>();
        return services;
    }
}