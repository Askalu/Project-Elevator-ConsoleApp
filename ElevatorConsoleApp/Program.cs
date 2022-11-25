using System.Net.Http.Headers;
using ElevatorConsoleApp;
using ElevatorConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using var host =
    Host.CreateDefaultBuilder(args)
        .UseEnvironment("Development")
        .ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Information))
        .ConfigureServices(services =>
        {
            services.AddHttpClient("APIClient", opt =>
            {
                opt.BaseAddress = new Uri("https://localhost:7016/api/");
                opt.DefaultRequestHeaders.Accept.Clear();
                opt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddSingleton<IApiService, ApiService>(); 
            services.AddTransient<IElevatorService, ElevatorService>();

            services.AddHostedService<HostedService>();
        }).Build();

await host.RunAsync();