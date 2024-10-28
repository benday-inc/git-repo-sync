using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Benday.GitRepoSync.Api;


[Command(
    Name = Constants.CommandArgumentNameRemoveRepo,
    IsAsync = false,
    Description = "Remove a repo from the list of configured repositories. " +
    "NOTE: Repository URL is the unique identifier")]
public class RemoveRepoConfigCommand : GitRepoConfigurationCommandBase
{
    public RemoveRepoConfigCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        args.AddString(Constants.ArgumentNameRepoUrl)
            .AsNotRequired()
            .WithDescription(
                "Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used");

        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();

        string repositoryUrl;

        if (Arguments.HasValue(Constants.ArgumentNameRepoUrl) == false)
        {
            repositoryUrl = GetGitRepoRemote(Environment.CurrentDirectory);

            if (string.IsNullOrEmpty(repositoryUrl))
            {
                throw new KnownException($"Current directory is not a git repository.");
            }
        }
        else
        {
            repositoryUrl =
                Arguments.GetStringValue(Constants.ArgumentNameRepoUrl);
        }

        if (string.IsNullOrEmpty(repositoryUrl))
        {
            throw new KnownException($"Repository URL cannot be empty.");
        }

        List<RepositoryInfo> repos = GetRepositories();

        RepositoryInfo? repo = repos.Where(
            r => string.Equals(r.GitUrl, repositoryUrl, StringComparison.CurrentCultureIgnoreCase))
            .FirstOrDefault();

        if (repo != null)
        {
            WriteLine("*** REMOVING REPOSITORY CONFIG ***");
            WriteRepositoryInfo(repo);

            repos.Remove(repo);

            WriteToDisk(repos);
        }
        else
        {
            WriteLine($"Repository '{repositoryUrl}' isn't configured.");
        }
    }

    private void WriteToDisk(List<RepositoryInfo> repos) { WriteToDisk(repos, GetConfigFilename()); }

    private void WriteToDisk(List<RepositoryInfo> repos, string filename)
    {
        if (File.Exists(filename) == false)
        {
            FileInfo info = new FileInfo(filename);

            var dir = info.Directory ?? throw new InvalidOperationException();

            if (dir.Exists == false)
            {
                dir.Create();
            }
        }

        StringBuilder builder = new();

        // header line
        builder.AppendLine("quicksync,category,description,parent folder,giturl");


        foreach (RepositoryInfo repo in repos)
        {
            builder.AppendLine(
                $"{repo.IsQuickSync}," +
                    $"{repo.Category.Replace(",", " ")}," +
                    $"{repo.RepositoryName.Replace(",", " ")}," +
                    $"{repo.ParentFolder.Replace('\\', '/')}," +
                    $"{repo.GitUrl}");
        }

        File.WriteAllText(filename, builder.ToString());
    }
}
