
using Benday.CommandsFramework;

using System.Collections.Generic;
using System.Diagnostics;

using System.IO;

namespace Benday.GitRepoSync.Api
{
    public abstract class GitRepoConfigurationCommandBase : SynchronousCommand
    {
        public GitRepoConfigurationCommandBase(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
            info,
            outputProvider)
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
                    string configName = GetConfigurationName();

                    GitRepoSyncConfiguration? temp =
                        GitRepoSyncConfigurationManager.Instance.Get(configName);

                    if (temp == null)
                    {
                        throw new KnownException(
                            $"Could not find a configuration named '{configName}'. Add a configuration and try again.");
                    }

                    _Configuration = temp;
                }

                return _Configuration;
            }

            set { _Configuration = value; }
        }

        protected string GetConfigurationName()
        {
            if (Arguments.ContainsKey(Constants.ArgumentNameConfigurationName) == true &&
                Arguments[Constants.ArgumentNameConfigurationName].HasValue)
            {
                string configName = Arguments[Constants.ArgumentNameConfigurationName].Value;

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
                    fromValue = fromValue.Replace("~", Environment.GetEnvironmentVariable("HOME"));
                }
                else
                {
                    fromValue = Path.Combine(Environment.CurrentDirectory, fromValue);
                }

                FileInfo info = new FileInfo(fromValue);

                return info.FullName;
            }
        }

        protected void ValidateConfiguration()
        {
            string configFilename = GetConfigFilename();
            string codeFolderPath = GetPath(Configuration.CodeDirectoryValue);

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
                string message = $"Config file does not exist - {configFilename}";

                throw new KnownException(message);
            }
        }

        protected string GetConfigFilename() { return GetPath(Configuration.ConfigurationFilePath); }

        protected string GetCodeDir()
        {
            string temp = GetPath(Configuration.CodeDirectoryValue);

            if (Directory.Exists(temp) == false)
            {
                return temp;
            }
            else
            {
                return new DirectoryInfo(temp).FullName;
            }
        }

        protected List<RepositoryInfo> GetRepositories()
        {
            string[] lines = File.ReadAllLines(GetConfigFilename());

            List<RepositoryInfo> returnValues = new List<RepositoryInfo>();

            bool isFirstLine = true;

            foreach (string line in lines)
            {
                if (isFirstLine == true)
                {
                    isFirstLine = false;
                    continue;
                }

                RepositoryInfo repo = GetRepository(line);

                if (repo != null)
                {
                    returnValues.Add(repo);
                }
            }

            return returnValues;
        }

        /// <summary>
        /// Gets a list of repositories from the config file optionally  filtered by command line filter options.
        /// /filter:string, /quicksync, and/or /category:string
        /// </summary>
        /// <returns></returns>
        protected List<RepositoryInfo> GetMatchingRepositories()
        {
            bool filterMode = Arguments.HasValue(Constants.ArgumentNameFilter);
            string filter = Arguments.GetStringValue(Constants.ArgumentNameFilter);
            bool categoryFilterMode = Arguments.HasValue(Constants.ArgumentNameCategory);
            string category = Arguments.GetStringValue(Constants.ArgumentNameCategory);

            bool isQuickSyncMode = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);

            List<RepositoryInfo> repos = GetRepositories();

            if (categoryFilterMode == true)
            {
                repos = repos.Where(r => string.Equals(r.Category, category, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
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
            string[] tokens = line.Split(',');

            if (tokens.Length != 5)
            {
                string message = $"Invalid line.  Not enough tokens.";
                Console.Error.WriteLine(message);
                Console.Error.WriteLine(line);

                throw new InvalidOperationException(message);
            }
            else
            {
                RepositoryInfo temp = new RepositoryInfo
                {
                    IsQuickSync = ToBoolean(tokens[0]),
                    Category = tokens[1],
                    RepositoryName = tokens[2],
                    ParentFolder = tokens[3],
                    GitUrl = tokens[4]
                };

                return temp;
            }
        }

        protected bool ToBoolean(string fromValue, bool defaultValue = false)
        {
            if (bool.TryParse(fromValue, out bool result) == true)
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
            Uri repoUri = new Uri(gitRepoUrl);

            string lastToken = repoUri.Segments.Last();

            if (string.IsNullOrWhiteSpace(lastToken) == true)
            {
                return string.Empty;
            }
            else if (lastToken.EndsWith(".git") == true)
            {
                return lastToken.Replace(".git", string.Empty);
            }
            else
            {
                return lastToken;
            }
        }

        public static void AddRepoFilters(ArgumentCollection args)
        {
            args.AddString(Constants.ArgumentNameFilter)
                .AsNotRequired()
                .WithDescription("Filter repos by partial string value");

            args.AddString(Constants.ArgumentNameCategory)
                .AsNotRequired()
                .WithDescription("Filter repos by category value. NOTE: this matches by full string");

            args.AddBoolean(Constants.ArgumentNameQuickSync)
                .AsNotRequired()
                .AllowEmptyValue()
                .WithDescription("Filter repos by 'quick sync' value");
        }

        protected string GetGitRepoRemote(string dir)
        {
            ProcessStartInfo temp = new ProcessStartInfo
            {
                WorkingDirectory = dir,

                FileName = "git",

                Arguments = "remote -v",
                CreateNoWindow = true,

                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(temp) ?? throw new InvalidOperationException();

            process.WaitForExit();

            string? output = process.StandardOutput.ReadLine();

            if (output != null)
            {
                output = output.Replace("origin	", string.Empty).Replace(" (fetch)", string.Empty);

                if (output.Contains('\t') == true)
                {
                    string[] tokens = output.Split('\t');

                    output = tokens.Last();
                }

                return output;
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetGitRepoRootDirectory(string dir)
        {
            ProcessStartInfo temp = new ProcessStartInfo
            {
                WorkingDirectory = dir,

                FileName = "git",

                Arguments = "rev-parse --show-toplevel",

                CreateNoWindow = true,

                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(temp) ?? throw new InvalidOperationException();

            process.WaitForExit();

            string? output = process.StandardOutput.ReadLine();

            if (output == null || output.Contains("not a git repository") == true)
            {
                throw new KnownException($"Directory '{dir}' is not a git repository");
            }
            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return output;
                }
                else
                {
                    output = output.Replace('/', '\\');
                    return output;
                }
            }
        }

        protected void WriteRepositoryInfo(RepositoryInfo repo)
        {
            WriteLine($"Name         : {repo.RepositoryName}");
            WriteLine($"Category     : {repo.Category}");
            WriteLine($"Quick Sync   : {repo.IsQuickSync}");
            WriteLine($"URL          : {repo.GitUrl}");
            WriteLine($"Parent Folder: {repo.ParentFolder}");
        }
    }
}
