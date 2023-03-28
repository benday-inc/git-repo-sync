using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Benday.GitRepoSync.Api;


[Command(Name = Constants.CommandArgumentNameGetRepoInfo,
    IsAsync = false,
    Description = "Gets the configuration info for the current repo")]
public class GetRepoInfoCommand : GitRepoConfigurationCommandBase
{
    public GetRepoInfoCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();

        string repositoryUrl;

        repositoryUrl = GetGitRepoRemote(Environment.CurrentDirectory);

        if (string.IsNullOrEmpty(repositoryUrl))
        {
            throw new KnownException($"Current directory is not a git repository.");
        }

        if (string.IsNullOrEmpty(repositoryUrl))
        {
            throw new KnownException($"Repository URL cannot be empty.");
        }

        List<RepositoryInfo> repos = GetRepositories();

        var repo = repos.Where(r => string.Equals(
            r.GitUrl, repositoryUrl,
            StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        if (repo != null)
        {
            WriteLine("*** REPOSITORY CONFIGURATION INFO ***");
            WriteRepositoryInfo(repo);
        }
        else
        {
            WriteLine($"Repository '{repositoryUrl}' is not configured for {Constants.ExeName}.");
            WriteLine($"To add this repository run '{Constants.ExeName} {Constants.CommandArgumentNameAddRepo}'.");
        }
    }
}
