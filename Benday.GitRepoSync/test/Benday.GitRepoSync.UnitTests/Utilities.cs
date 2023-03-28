
namespace Benday.GitRepoSync.UnitTests;

public class Utilities
{
    public static string[] GetStringArray(params string[] values)
    {
        return values;
    }

    public static string GetTempFolder()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks.ToString());

        if (Directory.Exists(tempPath) == false)
        {
            Directory.CreateDirectory(tempPath);
        }

        return tempPath;
    }

    public static GitRepoSyncConfigurationManager InitializeTestModeConfigurationManager()
    {
        var tempConfig = Path.Combine(GetTempFolder(), Constants.ConfigFileName);

        var manager = new GitRepoSyncConfigurationManager(tempConfig);

        GitRepoSyncConfigurationManager.Instance = manager;

        return manager;
    }

    public static void AssertFileExists(string path)
    {
        Assert.IsTrue(File.Exists(path), $"File does not exist '{path}'");
    }

    public static void AssertFileDoesNotExist(string path)
    {
        Assert.IsFalse(File.Exists(path), $"File should not exist '{path}'");
    }
}