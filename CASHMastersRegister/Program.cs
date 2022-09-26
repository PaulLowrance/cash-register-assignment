using CASHMasters.RegisterLib;
using CASHMasters.RegisterLib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

static void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(bldr =>
    {
        bldr.AddConsole();
        bldr.AddDebug();
    });

    var configuration = new ConfigurationBuilder().
        SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

    services.Configure<CashRegisterOptions>(configuration.GetSection("CashRegisterConfiguration"));
    services.AddTransient<ICashRegisterService, CashRegisterService>();
    
}

var services = new ServiceCollection();
ConfigureServices(services);

await using var serviceProvider = services.BuildServiceProvider();

var result = await serviceProvider.GetService<ICashRegisterService>().GetChangeWithLeastNumberOfCoins(5.15m, new decimal[]{5.00m, 1.00m});

Console.WriteLine($"Result: {result}");
