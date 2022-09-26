using CASHMasters.RegisterLib.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CASHMasters.RegisterLib.Services;

public class CashRegisterService : ICashRegisterService
{
    private ILogger<ICashRegisterService> _logger;
    private string _currentDenomination;
    private IDictionary<string, decimal[]> _configuredDenominations;

    public CashRegisterService(ILogger<ICashRegisterService> logger, IOptions<CashRegisterOptions> registerSettings)
    {
        _logger = logger;
        var registerSettings1 = registerSettings;
        _currentDenomination = registerSettings1.Value.DenominationOptions.CurrentDenomination;
        _configuredDenominations = registerSettings1.Value.DenominationOptions.ConfiguredDenominations;
    }

    public async Task<int> GetChangeWithLeastNumberOfCoins(decimal totalCharges, decimal[] providedCoins)
    {
        _logger.LogDebug("Starting GetChangeWithLeastNumberOfCoins with {TotalCharges} and {@ProvidedCoins}", totalCharges, providedCoins);
        
        var totalPayment = providedCoins.Sum(c => c);
        var coins = _configuredDenominations[_currentDenomination];
        var changeDue = totalPayment - totalCharges;

        _logger.LogDebug("Total of Payment: {TotalPayment}", totalPayment);
        _logger.LogDebug("Configured Denominations: {@Denominations}", coins);
        _logger.LogDebug("Change Due: {ChangeDueCustomer}", changeDue);
        
        if (changeDue < 0)
            throw new ArgumentException(
                "Total of provided coins and bills must be greater than or equal to total charges");

        var invalidDenoms = ValidateProvidedDenoms(coins, providedCoins).ToList();
        if(invalidDenoms.Any())
        {
            var invalidDenomString = string.Join(',', invalidDenoms);
            throw new ArgumentException($"Customer Payment contained invalid denominations: '{invalidDenomString}'");
        }

        return GetNumberOfCoinsForChange(coins, changeDue);
    }

    private IEnumerable<decimal> ValidateProvidedDenoms(decimal[] configuredDenoms, decimal[] providedCoins)
    {
        return providedCoins.Except(configuredDenoms);
    }

    private int GetNumberOfCoinsForChange(decimal[] coins, decimal changeDue)
    {
        var result = new List<decimal>();
        var roundedChange = decimal.Round(changeDue, 2);

        for (var i = coins.Length - 1; i >= 0; i--)
        {
            var coin = decimal.Round(coins[i], 2);

            while (roundedChange >= coin)
            {
                roundedChange -= coin;
                result.Add(coin);
            }
        }

        _logger.LogDebug("Denominations for Change: {@Denominations}", result);
        return result.Count;
    }
}