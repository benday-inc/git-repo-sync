using Benday.CommandsFramework;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = Constants.CommandArgumentNameCheckRepoStatus,
    IsAsync = false,
    Description = "Checks all configured repositories for pending changes and unpushed commits.")]
public class CheckRepoStatusCommand : GitRepoConfigurationCommandBase
{
    public CheckRepoStatusCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        AddRepoFilters(args);

        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();

        string codeFolderPath = GetCodeDir();

        WriteLine($"Configuration name: {GetConfigurationName()}");
        WriteLine($"Configuration file: {GetConfigFilename()}");
        WriteLine($"Code directory    : {codeFolderPath}");
        WriteLine(string.Empty);

        List<RepositoryInfo> repos = GetMatchingRepositories();

        int totalCount = repos.Count;
        int currentNumber = 0;
        int reposWithPendingChanges = 0;
        int reposWithUnpushedCommits = 0;
        int missingRepos = 0;

        List<(RepositoryInfo repo, bool hasPendingChanges, bool hasUnpushedCommits, string repoPath)> reposWithIssues = new();

        foreach (RepositoryInfo repo in repos)
        {
            currentNumber++;

            string parentFolder = ReplaceCodeVariable(repo.ParentFolder, codeFolderPath);
            string repoFolder = Path.Combine(parentFolder, GetGitRepoName(repo.GitUrl));

            if (Directory.Exists(repoFolder) == false)
            {
                missingRepos++;
                continue;
            }

            bool hasPendingChanges = HasPendingChanges(repoFolder);
            bool hasUnpushedCommits = HasUnpushedCommits(repoFolder);

            if (hasPendingChanges)
            {
                reposWithPendingChanges++;
            }

            if (hasUnpushedCommits)
            {
                reposWithUnpushedCommits++;
            }

            if (hasPendingChanges || hasUnpushedCommits)
            {
                reposWithIssues.Add((repo, hasPendingChanges, hasUnpushedCommits, repoFolder));
            }
        }

        WriteLine($"Total repositories checked: {totalCount}");
        WriteLine($"Missing repositories (not cloned): {missingRepos}");
        WriteLine($"Repositories with pending changes: {reposWithPendingChanges}");
        WriteLine($"Repositories with unpushed commits: {reposWithUnpushedCommits}");
        WriteLine(string.Empty);

        if (reposWithIssues.Count > 0)
        {
            WriteLine("=== REPOSITORIES REQUIRING ATTENTION ===");
            WriteLine(string.Empty);

            foreach (var (repo, hasPendingChanges, hasUnpushedCommits, repoPath) in reposWithIssues)
            {
                WriteLine($"*** {repo.RepositoryName} ***");
                WriteLine($"Path: {repoPath}");

                if (hasPendingChanges)
                {
                    WriteLine("  - Has pending changes (uncommitted files)");
                }

                if (hasUnpushedCommits)
                {
                    WriteLine("  - Has unpushed commits");
                }

                WriteLine(string.Empty);
            }
        }
        else
        {
            WriteLine("All repositories are clean with no pending changes or unpushed commits.");
        }
    }

    private bool HasPendingChanges(string repoFolder)
    {
        ProcessStartInfo statusCommand = new ProcessStartInfo
        {
            WorkingDirectory = repoFolder,
            FileName = "git",
            Arguments = "status --porcelain",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        var process = Process.Start(statusCommand) ?? throw new InvalidOperationException();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return !string.IsNullOrWhiteSpace(output);
    }

    private bool HasUnpushedCommits(string repoFolder)
    {
        // First check if there's an upstream tracking branch
        ProcessStartInfo checkUpstream = new ProcessStartInfo
        {
            WorkingDirectory = repoFolder,
            FileName = "git",
            Arguments = "rev-parse --abbrev-ref @{u}",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var upstreamProcess = Process.Start(checkUpstream) ?? throw new InvalidOperationException();
        upstreamProcess.StandardOutput.ReadToEnd();
        string errorOutput = upstreamProcess.StandardError.ReadToEnd();
        upstreamProcess.WaitForExit();

        // If no upstream is configured, we can't determine unpushed commits
        if (upstreamProcess.ExitCode != 0 || !string.IsNullOrWhiteSpace(errorOutput))
        {
            return false;
        }

        // Check for commits ahead of upstream
        ProcessStartInfo logCommand = new ProcessStartInfo
        {
            WorkingDirectory = repoFolder,
            FileName = "git",
            Arguments = "rev-list --count @{u}..HEAD",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        var process = Process.Start(logCommand) ?? throw new InvalidOperationException();
        string output = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();

        if (int.TryParse(output, out int count))
        {
            return count > 0;
        }

        return false;
    }

    private string ReplaceCodeVariable(string parentFolder, string codeFolderPath)
    {
        return parentFolder.Replace(Constants.CodeDirVariable, codeFolderPath);
    }
}
