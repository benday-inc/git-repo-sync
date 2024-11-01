﻿using Benday.CommandsFramework;
using Benday.GitRepoSync.Api;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Benday.GitRepoSync.UnitTests;

[TestClass]
public class AddUpdateConfigurationCommandFixture
{
    [TestInitialize]
    public void OnTestInitialize()
    {
        _SystemUnderTest = null;
        _ConfigurationManager = null;
    }

    private AddUpdateConfigurationCommand? _SystemUnderTest;

    private AddUpdateConfigurationCommand SystemUnderTest
    {
        get
        {
            Assert.IsNotNull(_SystemUnderTest, "Not initialized");

            return _SystemUnderTest;
        }
    }

    private GitRepoSyncConfigurationManager? _ConfigurationManager;

    private GitRepoSyncConfigurationManager ConfigurationManager
    {
        get
        {
            if (_ConfigurationManager == null)
            {
                _ConfigurationManager =
                    Utilities.InitializeTestModeConfigurationManager();
            }

            return _ConfigurationManager;
        }
    }

    private StringBuilderTextOutputProvider? _OutputProvider;

    private StringBuilderTextOutputProvider OutputProvider
    {
        get
        {
            if (_OutputProvider == null)
            {
                _OutputProvider = new StringBuilderTextOutputProvider();
            }

            return _OutputProvider;
        }
    }

    [TestMethod]
    public void AddNamedConfiguration()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        var expectedConfigurationName = "config123";
        var expectedToken = Environment.CurrentDirectory;
        var expectedCodeDir = "c:\\code";

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameAddUpdateConfig,
            $"/{Constants.ArgumentNameConfigurationName}:{expectedConfigurationName}",
            $"/{Constants.ArgumentNameConfigurationFile}:{expectedToken}",
            $"/{Constants.ArgumentNameCodeDirectory}:{expectedCodeDir}");

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new AddUpdateConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        var actual = ConfigurationManager.Get(expectedConfigurationName);

        Assert.IsNotNull(actual, $"Could not find configuration named '{expectedConfigurationName}'");
        Assert.AreEqual(expectedConfigurationName, actual.Name, "Config name was wrong");
        Assert.AreEqual(expectedToken, actual.ConfigurationFilePath, "Token was wrong");
    }

    [TestMethod]
    public void AddDefaultConfiguration()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        var expectedConfigurationName = Constants.DefaultConfigurationName;
        var expectedToken = Environment.CurrentDirectory;
        var expectedCodeDir = "c:\\code";

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameAddUpdateConfig,
            $"/{Constants.ArgumentNameConfigurationFile}:{expectedToken}",
            $"/{Constants.ArgumentNameCodeDirectory}:{expectedCodeDir}");

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new AddUpdateConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        var actual = ConfigurationManager.Get(expectedConfigurationName);

        Assert.IsNotNull(actual, $"Could not find configuration named '{expectedConfigurationName}'");
        Assert.AreEqual(expectedConfigurationName, actual.Name, "Config name was wrong");
        Assert.AreEqual(expectedToken, actual.ConfigurationFilePath, "Token was wrong");
    }
}