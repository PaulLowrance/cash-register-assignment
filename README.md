# CASHMasters Cash Register Application

This application takes input and provides the lowest number of bills or coins that are needed to make proper change for the sales transaction.  

## Usage

* Open a command prompt to the location of the `CASHMastersRegister.exe` file.  
* Enter `$> CASHMastersRegister --purchase-price <sale price> --customer-payment "<comma-separated list of values>"`
  * Example `$> CASHMastersRegister --purchase-price 5.25 --customer-payment "5.00, 1.00"`

## Configuration

The Denominations can be configured in the `appsettings.json` file within the `CashRegisterConfiguration` property.  
In the `DenominationOptions` property there are the following settings:
* `CurrentDenomination` - This is the denomination set that should be used for this installation of the register
* `ConfiguredDenominations` - These are the available denominations that can be used. More can be added by editing this file.

To change the denomination in use, simply update the `CurrentDenomination` to use the desired set from the `ConfiguredDenominations` collection.

## Extension

The service that is doing the work could easily be used in a Webservice or other application by referencing the `CASHMasters.RegisterLib` and configuring the services in application builder.  
Additional methods can be added to the CashRegisterService by adding the signatures to the `ICashRegisterService` interface, and then implementing them in the class.