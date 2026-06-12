using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = "quick",
    IsAsync = false,
    Description = "Add or update the current repo as a quick sync repository. " +
                  "NOTE: Repository URL is the unique identifier")]
public class AddUpdateQuickSyncRepoCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider)
    : AddUpdateRepoCommand(info,
        outputProvider)
{
    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        /*
        args.AddString(Constants.ArgumentNameRepoName)
            .AsNotRequired()
            .WithDescription("Human readable name for the repository");

        args.AddString(Constants.ArgumentNameParentDir)
            .AsNotRequired()
            .WithDescription("Parent directory for this repository. Essentially, where do you want this on disk?");

        args.AddBoolean(Constants.ArgumentNameQuickSync)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription("Add repo to quick sync");

        args.AddBoolean(Constants.ArgumentNameOverwrite)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription("Overwrites an existing repo config");

        args.AddString(Constants.ArgumentNameCategory).AsNotRequired().WithDescription("Category for the repository");

        args.AddString(Constants.ArgumentNameRepoUrl)
            .AsNotRequired()
            .WithDescription(
                "Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used");
*/
        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();
        
        var repositoryUrl = GetGitRepoRemote(Environment.CurrentDirectory);

        if (string.IsNullOrEmpty(repositoryUrl))
        {
            throw new KnownException($"Current directory is not a git repository.");
        }

        var parentDirectory = GetGitRepoRootDirectory(Environment.CurrentDirectory);

        if (string.IsNullOrEmpty(repositoryUrl))
        {
            throw new KnownException($"Repository URL cannot be empty.");
        }

        WriteLine($"Repository URL: {repositoryUrl}");

        var repos = GetRepositories();

        var repo = repos
            .FirstOrDefault(r => string.Equals(
                r.GitUrl, repositoryUrl, StringComparison.CurrentCultureIgnoreCase));

        if (repo != null)
        {
            // modify existing
            UpdateExistingRepositoryInfo(repo, repositoryUrl, parentDirectory, true);
            
            WriteLine("*** UPDATING REPOSITORY ***");
        }
        else
        {
            repo = new RepositoryInfo();

            PopulateNewRepositoryInfo(repo, repositoryUrl, parentDirectory, true);

            repos.Add(repo);

            WriteLine("*** ADDING REPOSITORY ***");
        }
        
        WriteRepositoryInfo(repo);

        WriteToDisk(repos);
    }

}