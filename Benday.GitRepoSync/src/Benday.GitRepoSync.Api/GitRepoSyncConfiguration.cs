using System.Text;

namespace Benday.GitRepoSync.Api;
public class GitRepoSyncConfiguration
{
    public string Name { get; set; } = Constants.DefaultConfigurationName;
    
    public string Token { get; set; } = string.Empty;

    public string GetTokenBase64Encoded()
    {
        var tokenBase64 = Convert.ToBase64String(
            ASCIIEncoding.ASCII.GetBytes(":" + Token));

        return tokenBase64;
    }
}
