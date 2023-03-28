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


[Command(Name = Constants.CommandArgumentNameAddRepo,
    IsAsync = false,
    Description = "Add or update a repo to the list of configured repositories. " +
    "NOTE: Repository URL is the unique identifier")]
public class AddUpdateRepoCommand : GitRepoConfigurationCommandBase
{
    public AddUpdateRepoCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        args.AddString(Constants.ArgumentNameRepoName)
            .AsNotRequired()
            .WithDescription("Name for the repository");

        args.AddString(Constants.ArgumentNameRepoDesc)
            .AsNotRequired()
            .WithDescription("Description for the repository");

        args.AddBoolean(Constants.ArgumentNameQuickSync)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription("Add repo to quick sync");

        args.AddBoolean(Constants.ArgumentNameOverwrite)
           .AsNotRequired()
           .AllowEmptyValue()
           .WithDescription("Overwrites an existing repo config");

        args.AddString(Constants.ArgumentNameCategory)
            .AsNotRequired()
            .WithDescription("Category for the repository");

        args.AddString(Constants.ArgumentNameRepoUrl)
            .AsNotRequired()
            .WithDescription("Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used");

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

        WriteLine($"Repository URL: {repositoryUrl}");

        List<RepositoryInfo> repos = GetRepositories();

        var repo = repos.Where(r => string.Equals(
            r.GitUrl, repositoryUrl, 
            StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        if (repo != null && 
            Arguments.GetBooleanValue(Constants.ArgumentNameOverwrite) == false)
        {
            WriteLine("*** REPOSITORY ALREADY CONFIGURED ***");
            WriteRepositoryInfo(repo);
            throw new KnownException($"Add the /{Constants.ArgumentNameOverwrite} argument to modify the configuration");
        }
        else if (repo != null &&
            Arguments.GetBooleanValue(Constants.ArgumentNameOverwrite) == true)
        {
            // modify existing

            UpdateExistingRepositoryInfo(repo, repositoryUrl);
        }
        else
        {
            repo = new RepositoryInfo();

            PopulateNewRepositoryInfo(repo, repositoryUrl);

            repos.Add(repo);
        }

        WriteLine("*** SAVE TO CONFIG FILE NOT IMPLEMENTED ***");
    }

    private void PopulateNewRepositoryInfo(RepositoryInfo match, string repositoryUrl)
    {
        match.GitUrl = repositoryUrl;

        if (Arguments.HasValue(Constants.ArgumentNameRepoName) == false)
        {
            match.RepositoryName = GetGitRepoName(match.GitUrl);
        }
        else
        {
            match.RepositoryName = Arguments.GetStringValue(Constants.ArgumentNameRepoName);
        }

        if (Arguments.HasValue(Constants.ArgumentNameCategory) == false)
        {
            match.Category = Constants.DefaultRepositoryCategory;
        }
        else
        {
            match.Category = Arguments.GetStringValue(Constants.ArgumentNameCategory);
        }

        if (Arguments.HasValue(Constants.ArgumentNameRepoDesc) == false)
        {
            match.Description = match.RepositoryName;
        }
        else
        {
            match.Description = Arguments.GetStringValue(Constants.ArgumentNameRepoDesc);
        }

        match.IsQuickSync = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);
    }

    private void UpdateExistingRepositoryInfo(RepositoryInfo match, string repositoryUrl)
    {
        match.GitUrl = repositoryUrl;

        if (Arguments.HasValue(Constants.ArgumentNameRepoName) == true)
        {
            match.RepositoryName = 
                Arguments.GetStringValue(Constants.ArgumentNameRepoName);
        }

        if (Arguments.HasValue(Constants.ArgumentNameCategory) == true)
        {
            match.Category = Arguments.GetStringValue(Constants.ArgumentNameCategory);
        }

        if (Arguments.HasValue(Constants.ArgumentNameRepoDesc) == true)
        {
            match.Description = Arguments.GetStringValue(Constants.ArgumentNameRepoDesc);
        }

        if (Arguments.HasValue(Constants.ArgumentNameQuickSync) == true)
        {
            match.IsQuickSync = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);
        }
    }
}
