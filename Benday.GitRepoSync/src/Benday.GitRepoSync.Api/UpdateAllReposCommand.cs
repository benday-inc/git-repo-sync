using Benday.CommandsFramework;
using Benday.Common;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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


    private int _TotalRepoCount = 0;
    private readonly ConcurrentBag<string> _FailedOperations = new ConcurrentBag<string>();

    protected override void OnExecute()
    {
        ValidateConfiguration();

        bool runMultithreaded = Arguments.GetBooleanValue(Constants.ArgumentNameParallel);
        string codeFolderPath = GetCodeDir();

        WriteLine($"Configuration name: {GetConfigurationName()}");
        WriteLine($"Configuration file: {GetConfigFilename()}");
        WriteLine($"Code directory    : {codeFolderPath}");
        WriteLine(string.Empty);

        List<RepositoryInfo> repos = GetMatchingRepositories();

        _TotalRepoCount = repos.Count;
        _FailedOperations.Clear();

        int currentNumber = 0;

        if (runMultithreaded == true)
        {
            WriteLine("*** EXPERIMENTAL: RUNNING MULTITHREADED...MESSAGES ARE GOING TO BE WEIRD. ***");

            Parallel.ForEach(
                repos,
                repo =>
                {
                    var repoNumber = Interlocked.Increment(ref currentNumber);
                    UpdateRepo(repo, codeFolderPath, repoNumber);                    
                });
        }
        else
        {
            foreach (RepositoryInfo repo in repos)
            {
                UpdateRepo(repo, codeFolderPath, currentNumber);
                currentNumber++;
            }
        }

        WriteLine(string.Empty);
        if (_FailedOperations.Count > 0)
        {
            WriteLine("=== FAILED OPERATIONS ===");
            foreach (var failedOp in _FailedOperations.OrderBy(x => x))
            {
                WriteLine(failedOp);
            }
        }
    }

    private void UpdateRepo(RepositoryInfo repo, string codeFolderPath, int currentNumber)
    {
        WriteLine($"Processing repo {currentNumber} of {_TotalRepoCount}: {repo.RepositoryName}...");

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
        if (Directory.Exists(parentFolder) == false)
        {
            Directory.CreateDirectory(parentFolder);
        }

        var cloneCommand = new ProcessStartInfo("git", $"clone {repo.GitUrl}")
        {
            WorkingDirectory = parentFolder
        };

        RunGitCommand(repo, cloneCommand, "clone");
    }

    private void SyncRepo(RepositoryInfo repo, string repoFolder)
    {
        var pullCommand = new ProcessStartInfo("git", $"pull") { WorkingDirectory = repoFolder };

        RunGitCommand(repo, pullCommand, "pull");
    }

    private void RunGitCommand(
        RepositoryInfo repo, 
        ProcessStartInfo pullCommand,
        string gitCommandName)
    {
        const int maxRetries = 2;
        const int defaultTimeoutMs = 10000;
        const int retryTimeoutMs = 30000;

        IProcessRunnerResult? result = null;
        int currentTimeout = defaultTimeoutMs;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var runner = new ProcessRunner(pullCommand);
                
                // Try to set a longer timeout on retry
                if (attempt > 0)
                {
                    try
                    {
                        var timeoutProperty = runner.GetType().GetProperty("TimeoutMs");
                        if (timeoutProperty != null && timeoutProperty.CanWrite)
                        {
                            timeoutProperty.SetValue(runner, retryTimeoutMs);
                            currentTimeout = retryTimeoutMs;
                        }
                    }
                    catch
                    {
                        // If property doesn't exist, continue with default
                    }
                }

                result = runner.Run();

                if (result.IsSuccess || result.IsError)
                {
                    // Success or normal error - don't retry
                    break;
                }
                else if (result.IsTimeout && attempt < maxRetries - 1)
                {
                    // Timeout occurred - retry with longer timeout
                    WriteLine($"Timeout on attempt {attempt + 1} calling '{gitCommandName}' for {repo.RepositoryName}. Retrying with {retryTimeoutMs}ms timeout...");
                    Thread.Sleep(1000); // Brief pause before retry
                    continue;
                }
            }
            catch (TimeoutException ex)
            {
                if (attempt < maxRetries - 1)
                {
                    WriteLine($"TimeoutException on attempt {attempt + 1} calling '{gitCommandName}' for {repo.RepositoryName}. Retrying with longer timeout...");
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    _FailedOperations.Add($"[{gitCommandName}] {repo.RepositoryName} - TimeoutException after {maxRetries} attempts: {ex.Message}");
                    WriteLine($"Failed calling '{gitCommandName}' for {repo.RepositoryName}: TimeoutException after {maxRetries} attempts: {ex.Message}");
                    return;
                }
            }
            catch (Exception ex)
            {
                _FailedOperations.Add($"[{gitCommandName}] {repo.RepositoryName} - {ex.GetType().Name}: {ex.Message}");
                WriteLine($"Exception calling '{gitCommandName}' for {repo.RepositoryName}: {ex.GetType().Name}: {ex.Message}");
                return;
            }
        }

        // Handle final result
        if (result != null)
        {
            if (result.IsSuccess)
            {
                // WriteLine($"Successfully called '{gitCommandName}' {repo.RepositoryName}.");
            }
            else if (result.IsError)
            {
                _FailedOperations.Add($"[{gitCommandName}] {repo.RepositoryName} - Error: {result.ErrorText}");
                WriteLine($"Error calling '{gitCommandName}' for {repo.RepositoryName}: {result.ErrorText}");
            }
            else if (result.IsTimeout)
            {
                _FailedOperations.Add($"[{gitCommandName}] {repo.RepositoryName} - Process timed out after {currentTimeout}ms");
                WriteLine($"Timeout calling '{gitCommandName}' for {repo.RepositoryName}.");
            }
            else
            {
                _FailedOperations.Add($"[{gitCommandName}] {repo.RepositoryName} - Unknown result");
                WriteLine($"Unknown result calling '{gitCommandName}' for {repo.RepositoryName}.");
            }
        }
    }

    private string ReplaceCodeVariable(string parentFolder, string codeFolderPath)
    { return parentFolder.Replace(Constants.CodeDirVariable, codeFolderPath); }
}
