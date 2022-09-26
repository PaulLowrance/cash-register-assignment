

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CASHMasters.RegisterLib.Tests;

using System;
using System.Collections.Generic;
using CASHMasters.RegisterLib.Interfaces;
using CASHMasters.RegisterLib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
    
[TestClass]    
public class RegisterService_MinimumNumberOfCoinsMexicoConfiguration
{
    private IOptions<CashRegisterOptions> _registerSettings;
    private Mock<ILogger<ICashRegisterService>> _loggerMock;
    private Mock<IDictionary<string, decimal[]>> _configuredDenominationsMock;

    [TestInitialize]
    public void InitializeMocks()
    {
        _loggerMock = new Mock<ILogger<ICashRegisterService>>();
        _configuredDenominationsMock = new Mock<IDictionary<string, decimal[]>>();

        var mockDenomOptions = new DenominationOptions
        {
            CurrentDenomination = "Mexico",
            ConfiguredDenominations = new Dictionary<string, decimal[]>()
        };
        mockDenomOptions.ConfiguredDenominations.Add("USA",
            new[] {0.01m, 0.05m, 0.10m, 0.25m, 0.50m, 1.00m, 2.00m, 5.00m, 10.00m, 20.00m, 50.00m, 100.00m });
        mockDenomOptions.ConfiguredDenominations.Add("Mexico",
            new[] { 0.05m, 0.10m, 0.25m, 0.50m, 1.00m, 2.00m, 5.00m, 10.00m, 20.00m, 50.00m, 100.00m });

        _registerSettings = new OptionsWrapper<CashRegisterOptions>(new CashRegisterOptions()
            { DenominationOptions = mockDenomOptions });
    }

    [TestMethod]
    public void HandleExactChange()
    {
        var service = new CashRegisterService(_loggerMock.Object, _registerSettings);

        var result = service.GetChangeWithLeastNumberOfCoins(5.15m, new[] { 5.00m, 0.10m, 0.05m }).Result;

        Assert.AreEqual(0, result);
    }


    [TestMethod]
    [ExpectedException(typeof(AggregateException),
        noExceptionMessage: "An exception is expected due to remaining payment needed")]
    public void NotEnoughPayment()
    {
        var service = new CashRegisterService(_loggerMock.Object, _registerSettings);

        var result = service.GetChangeWithLeastNumberOfCoins(5.15m, new[] { 5.00m, 0.05m }).Result;
    }
    
    [TestMethod]
    public void ExampleCase1FromAssignment()
    {
        var service = new CashRegisterService(_loggerMock.Object, _registerSettings);

        var result = service.GetChangeWithLeastNumberOfCoins(5.25m, new[] { 5.00m, 1.00m }).Result;

        //expecting a .50 and .25 piece
        Assert.AreEqual(2, result);
    }
    
    [TestMethod]
    public void ExampleCase2FromAssignment()
    {
        var service = new CashRegisterService(_loggerMock.Object, _registerSettings);

        var result = service.GetChangeWithLeastNumberOfCoins(5.25m, new[] { 5.00m, 0.10m , 0.10m, 0.10m, 0.10m, 0.10m, 0.10m, 0.10m, 0.10m, 0.10m, 0.10m}).Result;

        //expecting a .50 and .25 piece
        Assert.AreEqual(2, result);
    }
    
    [TestMethod]
    public void ExampleCase_UsingPennies()
    {
        var service = new CashRegisterService(_loggerMock.Object, _registerSettings);

        var result = service.GetChangeWithLeastNumberOfCoins(5.03m, new[] { 5.00m, 0.10m}).Result;

        //expecting a .05 and There are no .01 pieces
        Assert.AreEqual(1, result);
    }
}