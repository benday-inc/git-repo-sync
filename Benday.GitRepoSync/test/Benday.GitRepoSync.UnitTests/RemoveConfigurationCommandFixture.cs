﻿using Benday.CommandsFramework;
using Benday.GitRepoSync.Api;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Benday.GitRepoSync.UnitTests;

[TestClass]
public class RemoveConfigurationCommandFixture
{
    [TestInitialize]
    public void OnTestInitialize()
    {
        _SystemUnderTest = null;
        _ConfigurationManager = null;
    }

    private RemoveConfigurationCommand? _SystemUnderTest;

    private RemoveConfigurationCommand SystemUnderTest
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
    public void RemoveNamedConfiguration_ThatExists()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config1",
            ConfigurationFilePath = "token1"
        });

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config2",
            ConfigurationFilePath = "token2"
        });

        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameRemoveConfig,
            $"/{Constants.ArgumentNameConfigurationName}:config1");

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new RemoveConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        var actual1 = ConfigurationManager.Get("config1");
        var actual2 = ConfigurationManager.Get("config2");

        Assert.IsNull(actual1, $"Should not find configuration named 'config1'");
        Assert.IsNotNull(actual2, $"Could not find configuration named 'config2'");
    }
}