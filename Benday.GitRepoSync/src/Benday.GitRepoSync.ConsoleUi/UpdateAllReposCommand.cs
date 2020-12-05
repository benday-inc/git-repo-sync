using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Benday.GitRepoSync.ConsoleUi
{
    public class UpdateAllReposCommand : CommandBase
    {
        public UpdateAllReposCommand(string[] args) : base(args)
        {
            
        }

        protected override string CommandArgumentName
        {
            get
            {
                return Constants.CommandArgumentNameUpdateAllRepos;
            }
        }

        protected override void DisplayUsage(StringBuilder builder)
        {
            Console.WriteLine($"Command name: {CommandArgumentName}");
            Console.WriteLine($"Arguments:");
            Console.WriteLine($"\t{Constants.ArgumentNameConfigFile}");
            Console.WriteLine($"\t{Constants.ArgumentNameCodeFolderPath}");
        }

        protected override List<string> GetRequiredArguments()
        {
            var argumentNames = new List<string>();

            argumentNames.Add(Constants.ArgumentNameConfigFile);
            argumentNames.Add(Constants.ArgumentNameCodeFolderPath);

            return argumentNames;
        }

        protected string GetPath(string fromValue)
        {
            if (fromValue.StartsWith("~") == true)
            {
                fromValue = fromValue.Replace("~",
                    Environment.GetEnvironmentVariable("HOME"));
            }

            fromValue = Path.GetFullPath(fromValue);

            return fromValue;
        }

        protected override void AfterValidateArguments()
        {
            var configFilename = GetPath(GetArgumentValue(Constants.ArgumentNameConfigFile));
            var codeFolderPath = GetPath(GetArgumentValue(Constants.ArgumentNameCodeFolderPath));

            if (Directory.Exists(codeFolderPath) == false)
            {
                var message = $"Code folder directory does not exist - {codeFolderPath}";

                Console.Error.WriteLine(message);

                throw new DirectoryNotFoundException(message);
            }

            if (File.Exists(configFilename) == false)
            {
                var message = $"Config file does not exist - {configFilename}";

                Console.Error.WriteLine(message);

                throw new DirectoryNotFoundException(message);
            }
        }

        public override void Run()
        {
            var configFilename = GetPath(GetArgumentValue(Constants.ArgumentNameConfigFile));
            var codeFolderPath = GetPath(GetArgumentValue(Constants.ArgumentNameCodeFolderPath));

            List<RepositoryInfo> repos = GetRepositories(configFilename);

            int totalCount = repos.Count;
            int currentNumber = 0;

            foreach (var repo in repos)
            {
                // DebugRepoInfo(codeFolderPath, repo);

                UpdateRepo(repo, codeFolderPath, currentNumber, totalCount);
                currentNumber++;
            }
        }

        private void UpdateRepo(RepositoryInfo repo, string codeFolderPath,
            int currentNumber, int totalCount)
        {
            Console.WriteLine($"Processing repo {currentNumber} of {totalCount}: {repo.Description}...");

            var parentFolder = ReplaceCodeVariable(repo.ParentFolder, codeFolderPath);

            var repoFolder = Path.Combine(parentFolder, repo.RepositoryName);

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
            Console.WriteLine($"Cloning {repo.Description} into {parentFolder}...");

            if (Directory.Exists(parentFolder) == false)
            {
                Directory.CreateDirectory(parentFolder);
            }

            var cloneCommand = new ProcessStartInfo("git",
                $"clone {repo.GitUrl}");
            cloneCommand.WorkingDirectory = parentFolder;

            Process.Start(cloneCommand).WaitForExit();
        }

        private void SyncRepo(RepositoryInfo repo, string repoFolder)
        {
            Console.WriteLine($"Getting changes for {repo.Description}...");

            var fetchCommand = new ProcessStartInfo("git",
                $"fetch");
            fetchCommand.WorkingDirectory = repoFolder;

            var pullCommand = new ProcessStartInfo("git",
                $"pull");
            pullCommand.WorkingDirectory = repoFolder;

            Process.Start(fetchCommand).WaitForExit(); ;
            Process.Start(pullCommand).WaitForExit(); ;
        }

        private void DebugRepoInfo(string codeFolderPath, RepositoryInfo repo)
        {
            Console.WriteLine($"**********");
            Console.WriteLine($"Category     : {repo.Category}");
            Console.WriteLine($"Name         : {repo.RepositoryName}");
            Console.WriteLine($"Desc         : {repo.Description}");
            Console.WriteLine($"Parent folder: {repo.ParentFolder}");
            Console.WriteLine($"To folder    : {ReplaceCodeVariable(repo.ParentFolder, codeFolderPath)}");
            Console.WriteLine($"Git Url      : {repo.GitUrl}");
        }

        private string ReplaceCodeVariable(string parentFolder, string codeFolderPath)
        {
            return parentFolder.Replace(Constants.CodeDirVariable, codeFolderPath);
        }

        private List<RepositoryInfo> GetRepositories(string configFilename)
        {
            var lines = File.ReadAllLines(configFilename);

            var returnValues = new List<RepositoryInfo>();

            bool isFirstLine = true;

            foreach (var line in lines)
            {
                if (isFirstLine == true)
                {
                    isFirstLine = false;
                    continue;
                }

                var repo = GetRepository(line);

                if (repo != null)
                {
                    returnValues.Add(repo);
                }
            }

            return returnValues;
        }

        private RepositoryInfo GetRepository(string line)
        {
            var tokens = line.Split(',');

            if (tokens.Length != 4)
            {
                var message = $"Invalid line.  Not enough tokens.";
                Console.Error.WriteLine(message);
                Console.Error.WriteLine(line);

                throw new InvalidOperationException(message);
            }
            else
            {
                var temp = new RepositoryInfo();

                temp.Category = tokens[0];
                temp.Description = tokens[1];
                temp.ParentFolder = tokens[2];
                temp.GitUrl = tokens[3];
                temp.RepositoryName = GetGitRepoName(temp.GitUrl);

                return temp;
            }
        }

        private string GetGitRepoName(string gitRepoUrl)
        {
            var repoUri = new Uri(gitRepoUrl);
            
            var lastToken = repoUri.Segments.Last();

            if (String.IsNullOrWhiteSpace(lastToken) == true)
            {
                return String.Empty;
            }
            else if (lastToken.EndsWith(".git") == true)
            {
                return lastToken.Replace(".git", String.Empty);
            }
            else
            {
                return lastToken;
            }
        }

            private string GetGitRepoRemote(string dir)
        {
            var temp = new ProcessStartInfo();

            temp.WorkingDirectory = dir;

            temp.FileName = "git";

            temp.Arguments = "remotes";

            temp.CreateNoWindow = true;

            temp.UseShellExecute = false;
            temp.RedirectStandardOutput = true;

            var process = Process.Start(temp);

            process.WaitForExit();

            var output = process.StandardOutput.ReadLine();

            if (output != null)
            {
                output = output.Replace("origin	", String.Empty).Replace(" (fetch)", String.Empty);

                if (output.Contains('\t') == true)
                {
                    var tokens = output.Split('\t');

                    output = tokens.Last();
                }

                return output;
            }
            else
            {
                return string.Empty;
            }
        }

        
    }
}
