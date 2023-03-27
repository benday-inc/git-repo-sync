using System.Text;

namespace Benday.GitRepoSync.Api;
public class GitRepoSyncConfiguration
{
    public string Name { get; set; } = Constants.DefaultConfigurationName;

    public string ConfigurationFilePath { get; set; } = string.Empty;
    public string CodeDirectoryValue { get; set; } = string.Empty;

}
