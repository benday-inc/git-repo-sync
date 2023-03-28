using System.Collections.Generic;
using System.IO;

using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api
{
    public abstract class GitRepoConfigurationCommand : SynchronousCommand
    {
        public GitRepoConfigurationCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
               base(info, outputProvider)
        {

        }

        protected void AddCommonArguments(ArgumentCollection args)
        {
            args.AddString(Constants.ArgumentNameConfigurationName)
                .AsNotRequired()
                .WithDescription("Configuration name to use");
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

        protected void ValidateConfiguration()
        {
            var configFilename = GetConfigFilename();
            var codeFolderPath = GetPath(Configuration.CodeDirectoryValue);

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

        private string GetConfigFilename()
        {
            return GetPath(Configuration.ConfigurationFilePath);
        }

        protected List<RepositoryInfo> GetRepositories()
        {
            var lines = File.ReadAllLines(GetConfigFilename());

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

        /// <summary>
        /// Gets a list of repositories from the config file optionally 
        /// filtered by command line filter options.
        /// /filter:string, /quicksync, and/or /category:string
        /// </summary>
        /// <returns></returns>
        protected List<RepositoryInfo> GetMatchingRepositories()
        {
            var filterMode = Arguments.HasValue(Constants.ArgumentNameFilter);
            var filter = Arguments.GetStringValue(Constants.ArgumentNameFilter);
            var categoryFilterMode = Arguments.HasValue(Constants.ArgumentNameCategory);
            var category = Arguments.GetStringValue(Constants.ArgumentNameCategory);

            bool isQuickSyncMode = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);

            var repos = GetRepositories();

            if (categoryFilterMode == true)
            {
                repos = repos.Where(r => string.Equals(r.Category, category,
                    StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            if (filterMode == true)
            {
                repos = repos.Where(r => r.MatchesFilter(filter)).ToList();
            }

            if (isQuickSyncMode == true)
            {
                repos = repos.Where(r => r.IsQuickSync).ToList();
            }

            return repos;
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

        protected bool ToBoolean(string fromValue, bool defaultValue = false)
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

        protected string GetGitRepoName(string gitRepoUrl)
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
