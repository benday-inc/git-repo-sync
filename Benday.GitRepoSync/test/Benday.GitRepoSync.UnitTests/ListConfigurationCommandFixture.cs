using Benday.CommandsFramework;
using Benday.GitRepoSync.Api;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Benday.GitRepoSync.UnitTests;

[TestClass]
public class ListConfigurationCommandFixture
{
    [TestInitialize]
    public void OnTestInitialize()
    {
        _SystemUnderTest = null;
        _ConfigurationManager = null;
    }

    private ListConfigurationCommand? _SystemUnderTest;

    private ListConfigurationCommand SystemUnderTest
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
    public void ListNamedConfiguration_ThatExists()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config1",
            Token = "token1"
        });

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config2",
            Token = "token2"
        });

        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameListConfig,
            $"/{Constants.ArgumentNameConfigurationName}:config2");

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new ListConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Token: token2"), "didn't find token2 in output");
        Assert.IsTrue(output.Contains("Name: config2"), "didn't find config2 in output");
    }

    [TestMethod]
    public void ListAllConfigs()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config1",
            Token = "token1"
        });

        ConfigurationManager.Save(new GitRepoSyncConfiguration()
        {
            Name = "config2",
            Token = "token2"
        });

        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameListConfig);

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new ListConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        Utilities.AssertFileExists(ConfigurationManager.PathToConfigurationFile);
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("Token: token2"), "didn't find token2 in output");
        Assert.IsTrue(output.Contains("Name: config2"), "didn't find config2 in output");
    }

    [TestMethod]
    public void ListAllConfigs_NoConfigs()
    {
        // arrange
        Utilities.AssertFileDoesNotExist(ConfigurationManager.PathToConfigurationFile);

        Assert.AreEqual<int>(0, ConfigurationManager.GetAll().Length, "There should not be any configs at start of test.");

        var commandLineArgs = Utilities.GetStringArray(
            Constants.CommandArgumentNameListConfig);

        var executionInfo = new ArgumentCollectionFactory().Parse(commandLineArgs);

        _SystemUnderTest = new ListConfigurationCommand(executionInfo, OutputProvider);

        // act
        _SystemUnderTest.Execute();

        // assert        
        var output = OutputProvider.GetOutput();
        Console.WriteLine(output);

        Assert.IsTrue(output.Contains("No configurations"), "didn't find message in output");
    }
}
