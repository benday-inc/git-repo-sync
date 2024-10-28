using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Benday.CommandsFramework;

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
        string parentDirectory;

        if (Arguments.HasValue(Constants.ArgumentNameRepoUrl) == false)
        {
            repositoryUrl = GetGitRepoRemote(Environment.CurrentDirectory);

            if (string.IsNullOrEmpty(repositoryUrl))
            {
                throw new KnownException($"Current directory is not a git repository.");
            }

            parentDirectory = GetGitRepoRootDirectory(Environment.CurrentDirectory);
        }
        else
        {
            if (Arguments.HasValue(Constants.ArgumentNameParentDir) == false)
            {
                throw new KnownException($"You must specify a value for /{Constants.ArgumentNameParentDir} " +
                    $"when you use the /{Constants.ArgumentNameRepoUrl} argument.");
            }
            else
            {
                parentDirectory =
                    Arguments.GetStringValue(Constants.ArgumentNameParentDir);
            }

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

            UpdateExistingRepositoryInfo(repo, repositoryUrl, parentDirectory);

            WriteLine("*** UPDATING REPOSITORY ***");
        }
        else
        {
            repo = new RepositoryInfo();

            PopulateNewRepositoryInfo(repo, repositoryUrl, parentDirectory);

            repos.Add(repo);

            WriteLine("*** ADDING REPOSITORY ***");
        }


        WriteRepositoryInfo(repo);

        WriteToDisk(repos);
    }

    private void PopulateNewRepositoryInfo(RepositoryInfo match,
        string repositoryUrl,
        string parentDirectory)
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

        match.IsQuickSync = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);

        match.ParentFolder = EscapeParentDirectory(parentDirectory);
    }

    private string EscapeParentDirectory(string parentDirectory)
    {
        var dir = new DirectoryInfo(parentDirectory);

        var parentDir = dir.Parent.FullName;
        var codeDir = GetCodeDir();

        var parentDirNameEscaped = parentDir.Replace(codeDir,
            Constants.CodeDirVariable,
            StringComparison.CurrentCultureIgnoreCase);

        return parentDirNameEscaped;
    }

    private void UpdateExistingRepositoryInfo(RepositoryInfo match,
        string repositoryUrl,
        string parentDirectory)
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

        if (Arguments.HasValue(Constants.ArgumentNameQuickSync) == true)
        {
            match.IsQuickSync = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);
        }

        match.ParentFolder = EscapeParentDirectory(parentDirectory);
    }

    private void WriteToDisk(List<RepositoryInfo> repos)
    {

        WriteToDisk(repos, GetConfigFilename());
    }

    private void WriteToDisk(List<RepositoryInfo> repos, string filename)
    {
        if (File.Exists(filename) == false)
        {
            var info = new FileInfo(filename);

            var dir = info.Directory;

            if (dir.Exists == false)
            {
                dir.Create();
            }
        }

        StringBuilder builder = new();

        // header line
        builder.AppendLine("quicksync,category,description,parent folder,giturl");


        foreach (var repo in repos)
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