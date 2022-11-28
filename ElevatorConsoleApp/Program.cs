using System.Net.Http.Headers;
using System.Reflection;
using ElevatorConsoleApp;
using ElevatorConsoleApp.Pages;
using ElevatorConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using var host =
    Host.CreateDefaultBuilder(args)
        .UseEnvironment("Development")
        .ConfigureLogging(options =>
        {
            options.SetMinimumLevel(LogLevel.Trace);
            options.AddFilter("Microsoft", LogLevel.Warning);
            options.AddFilter("System", LogLevel.Error);
            options.AddFilter("Engine", LogLevel.Warning);
        })
        .ConfigureServices((context,services) =>
        {
            services.AddHttpClient("APIClient", opt =>
            {
                opt.BaseAddress = new Uri(context.Configuration["ApiURL"] ?? throw new ArgumentException());
                opt.DefaultRequestHeaders.Accept.Clear();
                opt.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            // Services
            services.AddSingleton<IGeneratorService, GeneratorService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddTransient<IDatabaseService, SqliteDatabaseService>();
            services.AddTransient<IElevatorService, ElevatorService>();

            // Pages
            services.AddTransient<GeneratePage>();
            services.AddTransient<ImportApiPage>();
            services.AddTransient<ElevatorPage>();

            services.AddHostedService<HostedService>();
        }).Build();

await host.RunAsync();