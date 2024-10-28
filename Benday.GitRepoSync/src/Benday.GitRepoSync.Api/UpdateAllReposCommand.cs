using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = Constants.CommandArgumentNameUpdateAllRepos,
    IsAsync = false,
    Description = "Performs a 'git clone' or 'git pull' for each configured git repository.")]
public class UpdateAllReposCommand : GitRepoConfigurationCommandBase
{
    public UpdateAllReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        AddRepoFilters(args);

        args.AddBoolean(Constants.ArgumentNameParallel)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription(
                "EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess.");

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        bool runMultithreaded = Arguments.HasValue(Constants.ArgumentNameParallel);
        string codeFolderPath = GetCodeDir();

        WriteLine($"Configuration name: {GetConfigurationName()}");
        WriteLine($"Configuration file: {GetConfigFilename()}");
        WriteLine($"Code directory    : {codeFolderPath}");
        WriteLine(string.Empty);

        List<RepositoryInfo> repos = GetMatchingRepositories();

        int totalCount = repos.Count;

        int currentNumber = 0;

        if (runMultithreaded == true)
        {
            WriteLine("*** EXPERIMENTAL: RUNNING MULTITHREADED...MESSAGES ARE GOING TO BE WEIRD. ***");

            Parallel.ForEach(
                repos,
                repo =>
                {
                    UpdateRepo(repo, codeFolderPath, currentNumber, totalCount);
                    currentNumber++;
                });
        }
        else
        {
            foreach (RepositoryInfo repo in repos)
            {
                // DebugRepoInfo(codeFolderPath, repo);

                UpdateRepo(repo, codeFolderPath, currentNumber, totalCount);
                currentNumber++;
            }
        }
    }

    private void UpdateRepo(RepositoryInfo repo, string codeFolderPath, int currentNumber, int totalCount)
    {
        WriteLine($"Processing repo {currentNumber} of {totalCount}: {repo.RepositoryName}...");

        string parentFolder = ReplaceCodeVariable(repo.ParentFolder, codeFolderPath);

        string repoFolder = Path.Combine(parentFolder, GetGitRepoName(repo.GitUrl));

        if (Directory.Exists(repoFolder) == true)
        {
            SyncRepo(repo, repoFolder);
        }
        else
        {
            CloneRepo(repo, parentFolder);
        }
    }

    private void CloneRepo(RepositoryInfo repo, string parentFolder)
    {
        WriteLine($"Cloning {repo.RepositoryName} into {parentFolder}...");

        if (Directory.Exists(parentFolder) == false)
        {
            Directory.CreateDirectory(parentFolder);
        }

        ProcessStartInfo cloneCommand = new ProcessStartInfo("git", $"clone {repo.GitUrl}")
        {
            WorkingDirectory = parentFolder
        };

        var process = Process.Start(cloneCommand) ?? throw new InvalidOperationException();

        process.WaitForExit();
    }

    private void SyncRepo(RepositoryInfo repo, string repoFolder)
    {
        WriteLine($"Getting changes for {repo.RepositoryName}...");

        ProcessStartInfo pullCommand = new ProcessStartInfo("git", $"pull") { WorkingDirectory = repoFolder };

        var process = Process.Start(pullCommand) ?? throw new InvalidOperationException();

        process.WaitForExit();
    }


    private string ReplaceCodeVariable(string parentFolder, string codeFolderPath)
    { return parentFolder.Replace(Constants.CodeDirVariable, codeFolderPath); }
}
