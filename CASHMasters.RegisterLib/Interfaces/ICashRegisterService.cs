namespace CASHMasters.RegisterLib.Interfaces;

public interface ICashRegisterService
{
    /// <summary>
    /// Returns the lowest possible number of coins/bills that are required to give correct change
    /// </summary>
    /// <param name="totalCharges">The total of the sale</param>
    /// <param name="providedCoins">All of the bills/coins that the customer paid with</param>
    /// <returns></returns>
    Task<int> GetChangeWithLeastNumberOfCoins(decimal totalCharges, decimal[] providedCoins);
}