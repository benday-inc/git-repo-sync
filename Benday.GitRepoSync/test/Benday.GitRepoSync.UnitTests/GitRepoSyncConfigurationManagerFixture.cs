using Benday.GitRepoSync.Api;
namespace Benday.GitRepoSync.UnitTests;

[TestClass]
public class GitRepoSyncConfigurationManagerFixture
{
    [TestInitialize]
    public void OnTestInitialize()
    {
        _SystemUnderTest = null;
    }

    private GitRepoSyncConfigurationManager? _SystemUnderTest;

    private GitRepoSyncConfigurationManager SystemUnderTest
    {
        get
        {
            if (_SystemUnderTest == null)
            {
                _SystemUnderTest = new GitRepoSyncConfigurationManager();
            }

            return _SystemUnderTest;
        }
    }

    [TestMethod]
    public void GetConfigurationFilePath()
    {
        // arrange
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var expected = Path.Combine(userProfilePath, Constants.ExeName, Constants.ConfigFileName);

        // act
        var actual = SystemUnderTest.PathToConfigurationFile;

        // assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(actual), "Path should not be empty");

        Console.WriteLine($"{actual}");

        Assert.AreEqual<string>(expected, actual, "Path was wrong");
    }

    [TestMethod]
    public void AddConfig_Default()
    {
        // arrange
        _SystemUnderTest = Utilities.InitializeTestModeConfigurationManager();

        var expectedConfigurationName = Constants.DefaultConfigurationName;
        var expectedToken = "token-value";
        
        var config = new GitRepoSyncConfiguration()
        {
            ConfigurationFilePath = expectedToken,
            Name = expectedConfigurationName
        };


        Utilities.AssertFileDoesNotExist(SystemUnderTest.PathToConfigurationFile);

        // act
        SystemUnderTest.Save(config);

        // assert
        Utilities.AssertFileExists(SystemUnderTest.PathToConfigurationFile);

        var actual = SystemUnderTest.Get(expectedConfigurationName);

        Assert.IsNotNull(actual, $"Could not find configuration named '{expectedConfigurationName}'");
        Assert.AreEqual(expectedConfigurationName, actual.Name, "Config name was wrong");
        Assert.AreEqual(expectedToken, actual.ConfigurationFilePath, "Token was wrong");
    }

    [TestMethod]
    public void AddConfig_Named()
    {
        // arrange
        _SystemUnderTest = Utilities.InitializeTestModeConfigurationManager();

        var expectedConfigurationName = "config123";
        var expectedToken = "token-value";
        
        var config = new GitRepoSyncConfiguration()
        {
            ConfigurationFilePath = expectedToken,
            Name = expectedConfigurationName
        };

        Utilities.AssertFileDoesNotExist(SystemUnderTest.PathToConfigurationFile);

        // act
        SystemUnderTest.Save(config);

        // assert
        Utilities.AssertFileExists(SystemUnderTest.PathToConfigurationFile);

        var actual = SystemUnderTest.Get(expectedConfigurationName);

        Assert.IsNotNull(actual, $"Could not find configuration named '{expectedConfigurationName}'");
        Assert.AreEqual(expectedConfigurationName, actual.Name, "Config name was wrong");
        Assert.AreEqual(expectedToken, actual.ConfigurationFilePath, "Token was wrong");
    }

    [TestMethod]
    public void RemoveConfig()
    {
        // arrange
        _SystemUnderTest = Utilities.InitializeTestModeConfigurationManager();

        SystemUnderTest.Save(new GitRepoSyncConfiguration()
        {
            Name = "config1",
            ConfigurationFilePath = "token1"
        });

        SystemUnderTest.Save(new GitRepoSyncConfiguration()
        {
            Name = "config2",
            ConfigurationFilePath = "token2"
        });

        // act
        SystemUnderTest.Remove("config2");

        // assert
        var actual = SystemUnderTest.Get("config2");

        Assert.IsNull(actual, $"Should not find configuration named 'config2'");
    }

    [TestMethod]
    public void GetAll()
    {
        // arrange
        _SystemUnderTest = Utilities.InitializeTestModeConfigurationManager();

        SystemUnderTest.Save(new GitRepoSyncConfiguration()
        {
            Name = "config1",
            ConfigurationFilePath = "token1"
        });

        SystemUnderTest.Save(new GitRepoSyncConfiguration()
        {
            Name = "config2",
            ConfigurationFilePath = "token2"
        });

        // act
        var actual = SystemUnderTest.GetAll();

        // assert
        Assert.IsNotNull(actual);
        Assert.AreEqual<int>(2, actual.Length, "Config count was wrong.");
    }
}