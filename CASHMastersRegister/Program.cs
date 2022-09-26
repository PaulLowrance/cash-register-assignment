using CASHMasters.RegisterLib;
using CASHMasters.RegisterLib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CASHMastersRegister;

public static class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        await using var serviceProvider = services.BuildServiceProvider();

        var validationResult = ValidateInputs(args);

        if (!validationResult.isValid)
        {
            PrintHelpText();
            return;
        }

        var result = await serviceProvider.GetService<ICashRegisterService>()
            .GetChangeWithLeastNumberOfCoins(validationResult.totalCharges, validationResult.providedCoins);

        Console.WriteLine($"Result: {result}");
    }

    static (bool isValid, decimal totalCharges, decimal[] providedCoins) ValidateInputs(string[] args)
    {
        if (args.Any(arg => arg.ToLower().Equals("--help")))
        {
            return (false, decimal.MinValue, Array.Empty<decimal>());
        }

        if (args.Contains("--purchase-amount") && args.Contains("--customer-payment"))
        {
            var argList = args.ToList();
            if (!decimal.TryParse(argList[argList.IndexOf("--purchase-amount") + 1], out var purchaseAmount))
            {
                Console.WriteLine($"Unable to convert Purchase Price to decimal");
                return (false, Decimal.MinValue, Array.Empty<decimal>());
            }
            
            if (ConvertDenominationArgToArray(argList[argList.IndexOf("--customer-payment") + 1], out decimal[] customerPayment))
            {
                return (true, purchaseAmount, customerPayment);
            }
        }

        PrintHelpText();
        return (false, decimal.MinValue, Array.Empty<decimal>());
    }

    private static void PrintHelpText()
    {
        Console.WriteLine("USAGE: CASHMastersRegister --purchase-amount <total cost of items> --customer-payment <comma-separated list of bills/coins from the customer>");
        Console.WriteLine("EXAMPLE: CASHMastersRegister --purchase-amount 5.25 --customer-payment \"5.00, 1.00\"");
    }

    private static bool ConvertDenominationArgToArray(string customerPaymentString, out decimal[] denominationArray)
    {
        var splitString =
            customerPaymentString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var bills = new List<decimal>();
        foreach (var item in splitString)
        {
            if (decimal.TryParse(item, out var decimalItem))
            {
                bills.Add(decimalItem);
            }
            else
            {
                Console.WriteLine($"Provided value {item}, is not a decimal number");
                denominationArray = Array.Empty<decimal>();
                return false;
            }
        }

        denominationArray = bills.ToArray();
        return true;
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(bldr =>
        {
            bldr.AddConsole();
            bldr.AddDebug();
        });

        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        services.Configure<CashRegisterOptions>(configuration.GetSection("CashRegisterConfiguration"));
        services.AddTransient<ICashRegisterService, CashRegisterService>();
    }
}