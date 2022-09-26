namespace CASHMasters.RegisterLib;

public class CashRegisterOptions
{
    public DenominationOptions DenominationOptions { get; set; } 
}

public class DenominationOptions
{
    public string CurrentDenomination { get; set; }
    public IDictionary<string, decimal[]> ConfiguredDenominations { get; set; }
}