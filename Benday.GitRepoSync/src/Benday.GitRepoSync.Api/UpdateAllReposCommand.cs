using Benday.CommandsFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Benday.GitRepoSync.Api
{
    [Command(Name = Constants.CommandArgumentNameUpdateAllRepos,
        IsAsync = false,
        Description = "Reads existing Git repositories and outputs configuration information to config file.")]
    public class UpdateAllReposCommand : SynchronousCommand
    {
        public UpdateAllReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
               base(info, outputProvider)
        {

        }

        public override ArgumentCollection GetArguments()
        {
            var args = new ArgumentCollection();

            args.AddString(Constants.ArgumentNameConfigurationName)
                .AsNotRequired()
                .WithDescription("Configuration name to use");

            args.AddString(Constants.ArgumentNameCategory)
    .AsNotRequired()
    .WithDescription("Category of repos to sync");

            /*
            args.AddString(Constants.ArgumentNameConfigFile)
                .WithDescription("Path to configuration file");

            args.AddString(Constants.ArgumentNameCodeFolderPath)
                .WithDescription("Path for code directory variable in the config file.");
            */

            args.AddBoolean(Constants.ArgumentNameListCategories)
                .AsNotRequired()
                .AllowEmptyValue()
                .WithDescription("Lists the categories in the config file");

            args.AddBoolean(Constants.ArgumentNameQuickSync)
                .AsNotRequired()
                .AllowEmptyValue()
                .WithDescription("Sync only repos that are marked as 'quick sync'");

            args.AddBoolean(Constants.ArgumentNameParallel)
                .AsNotRequired()
                .AllowEmptyValue()
                .WithDescription("EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess.");

            return args;
        }

        private GitRepoSyncConfiguration? _Configuration;

        protected GitRepoSyncConfiguration Configuration
        {
            get
            {
                if (_Configuration == null)
                {
                    var configName = GetConfigurationName();

                    var temp =
                        GitRepoSyncConfigurationManager.Instance.Get(configName);

                    if (temp == null)
                    {
                        throw new KnownException($"Could not find a configuration named '{configName}'. Add a configuration and try again.");
                    }

                    _Configuration = temp;
                }

                return _Configuration;
            }

            set => _Configuration = value;
        }

        protected string GetConfigurationName()
        {
            if (Arguments.ContainsKey(Constants.ArgumentNameConfigurationName) == true &&
                Arguments[Constants.ArgumentNameConfigurationName].HasValue)
            {
                var configName = Arguments[Constants.ArgumentNameConfigurationName].Value;

                return configName;
            }
            else
            {
                return Constants.DefaultConfigurationName;
            }
        }


        protected override void OnExecute()
        {
            ValidateArguments();

            //var configFilename = GetPath(Arguments.GetStringValue(Constants.ArgumentNameConfigFile));
            //var codeFolderPath = GetPath(Arguments.GetStringValue(Constants.ArgumentNameCodeFolderPath));

            var configFilename = GetPath(Arguments.GetStringValue(Configuration.ConfigurationFilePath));
            var codeFolderPath = GetPath(Arguments.GetStringValue(Configuration.CodeDirectoryValue));

            var listCategoriesMode = Arguments.HasValue(Constants.ArgumentNameListCategories);
            var hasCategoryFilter = Arguments.HasValue(Constants.ArgumentNameCategory);
            var runMultithreaded = Arguments.HasValue(Constants.ArgumentNameParallel);

            bool isQuickSyncMode = false;

            if (hasCategoryFilter == false && Arguments.HasValue(Constants.ArgumentNameQuickSync) == true)
            {
                isQuickSyncMode = true;
            }

            List<RepositoryInfo> repos = GetRepositories(configFilename);

            int totalCount = repos.Count;

            if (listCategoriesMode == true)
            {
                var categories = (from temp in repos
                                  select temp.Category)
                                  .Distinct().OrderBy(x => x);

                WriteLine($"*** Category List ***");

                foreach (var item in categories)
                {
                    WriteLine($"{item}");
                }
            }
            else
            {
                if (hasCategoryFilter == true)
                {
                    var categoryFilter = Arguments.GetStringValue(Constants.ArgumentNameCategory);

                    repos = repos.Where(x => x.Category == categoryFilter).ToList();

                    totalCount = repos.Count;
                }

                int currentNumber = 0;

                if (runMultithreaded == true)
                {
                    Parallel.ForEach(repos, repo =>
                    {
                        UpdateRepo(isQuickSyncMode, repo, codeFolderPath, currentNumber, totalCount);
                        currentNumber++;
                    });
                }
                else
                {
                    foreach (var repo in repos)
                    {
                        // DebugRepoInfo(codeFolderPath, repo);

                        UpdateRepo(isQuickSyncMode, repo, codeFolderPath, currentNumber, totalCount);
                        currentNumber++;
                    }
                }
            }
        }

        protected string GetPath(string fromValue)
        {
            if (Path.IsPathFullyQualified(fromValue) == true)
            {
                return fromValue;
            }
            else
            {
                if (fromValue.StartsWith("~") == true)
                {
                    fromValue = fromValue.Replace("~",
                        Environment.GetEnvironmentVariable("HOME"));
                }
                else
                {
                    fromValue = Path.Combine(Environment.CurrentDirectory, fromValue);
                }

                var info = new FileInfo(fromValue);
                
                return info.FullName;
            }
        }

        protected void ValidateArguments()
        {
            var configFilename = GetPath(Arguments.GetStringValue(Constants.ArgumentNameConfigFile));
            var codeFolderPath = GetPath(Arguments.GetStringValue(Constants.ArgumentNameCodeFolderPath));

            if (Directory.Exists(codeFolderPath) == false)
            {
                // var message = $"Code folder directory does not exist - {codeFolderPath}";

                WriteLine($"Code folder directory does not exist - {codeFolderPath}. Creating...");
                // Console.Error.WriteLine(message);

                // throw new DirectoryNotFoundException(message);
                Directory.CreateDirectory(codeFolderPath);
            }

            if (File.Exists(configFilename) == false)
            {
                var message = $"Config file does not exist - {configFilename}";

                throw new KnownException(message);
            }
        }        

        private void UpdateRepo(bool isQuickSyncMode,
            RepositoryInfo repo, string codeFolderPath,
            int currentNumber, int totalCount)
        {
            if (isQuickSyncMode == true && repo.IsQuickSync == false)
            {
                WriteLine($"Quick sync is skipping repo {currentNumber} of {totalCount}: {repo.Description}...");
            }
            else
            {
                WriteLine($"Processing repo {currentNumber} of {totalCount}: {repo.Description}...");

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
        }

        private void CloneRepo(RepositoryInfo repo, string parentFolder)
        {
            WriteLine($"Cloning {repo.Description} into {parentFolder}...");

            if (Directory.Exists(parentFolder) == false)
            {
                Directory.CreateDirectory(parentFolder);
            }

            var cloneCommand = new ProcessStartInfo("git",
                $"clone {repo.GitUrl}")
            {
                WorkingDirectory = parentFolder
            };

            Process.Start(cloneCommand).WaitForExit();
        }

        private void SyncRepo(RepositoryInfo repo, string repoFolder)
        {
            WriteLine($"Getting changes for {repo.Description}...");

            var pullCommand = new ProcessStartInfo("git",
                $"pull")
            {
                WorkingDirectory = repoFolder
            };

            Process.Start(pullCommand).WaitForExit(); ;
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

            if (tokens.Length != 5)
            {
                var message = $"Invalid line.  Not enough tokens.";
                Console.Error.WriteLine(message);
                Console.Error.WriteLine(line);

                throw new InvalidOperationException(message);
            }
            else
            {
                var temp = new RepositoryInfo
                {
                    IsQuickSync = ToBoolean(tokens[0]),
                    Category = tokens[1],
                    Description = tokens[2],
                    ParentFolder = tokens[3],
                    GitUrl = tokens[4]
                };
                temp.RepositoryName = GetGitRepoName(temp.GitUrl);

                return temp;
            }
        }

        private bool ToBoolean(string fromValue, bool defaultValue = false)
        {
            if (bool.TryParse(fromValue, out var result) == true)
            {
                return result;
            }
            else
            {
                return defaultValue;
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
    }
}
