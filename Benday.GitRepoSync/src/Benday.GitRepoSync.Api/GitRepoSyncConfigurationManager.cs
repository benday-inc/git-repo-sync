
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Benday.GitRepoSync.Api;

public class GitRepoSyncConfigurationManager
{
    public static GitRepoSyncConfigurationManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GitRepoSyncConfigurationManager();
            }

            return _Instance;
        }
        set { _Instance = value; }
    }

    public GitRepoSyncConfigurationManager()
    {
    }

    public GitRepoSyncConfigurationManager(string pathToConfigurationFile)
    { _PathToConfigurationFile = pathToConfigurationFile; }

    private string? _PathToConfigurationFile;
    private static GitRepoSyncConfigurationManager? _Instance;

    // Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    public string PathToConfigurationFile
    {
        get
        {
            if (_PathToConfigurationFile == null)
            {
                string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                _PathToConfigurationFile = Path.Combine(userProfilePath, Constants.ExeName, Constants.ConfigFileName);
            }

            return _PathToConfigurationFile;
        }

        private set { _PathToConfigurationFile = value; }
    }

    public GitRepoSyncConfiguration? Get(string name = Constants.DefaultConfigurationName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        if (File.Exists(PathToConfigurationFile) == false)
        {
            return null;
        }
        else
        {
            string json = File.ReadAllText(PathToConfigurationFile);

            GitRepoSyncConfiguration[]? configs = JsonSerializer.Deserialize<GitRepoSyncConfiguration[]>(json);

            if (configs == null || configs.Length == 0)
            {
                return null;
            }
            else
            {
                GitRepoSyncConfiguration? match = configs.Where(x => x.Name == name).FirstOrDefault();

                return match;
            }
        }
    }

    public GitRepoSyncConfiguration[] GetAll()
    {
        if (File.Exists(PathToConfigurationFile) == false)
        {
            return new GitRepoSyncConfiguration[] { };
        }
        else
        {
            string json = File.ReadAllText(PathToConfigurationFile);

            GitRepoSyncConfiguration[]? configs = JsonSerializer.Deserialize<GitRepoSyncConfiguration[]>(json);

            if (configs == null || configs.Length == 0)
            {
                return new GitRepoSyncConfiguration[] { };
            }
            else
            {
                return configs;
            }
        }
    }

    public void Save(GitRepoSyncConfiguration config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }
        else if (string.IsNullOrEmpty(config.Name) == true)
        {
            throw new ArgumentException(nameof(config), "Name value not set");
        }

        List<GitRepoSyncConfiguration> configurations;

        if (File.Exists(PathToConfigurationFile) == true)
        {
            string json = File.ReadAllText(PathToConfigurationFile);

            GitRepoSyncConfiguration[]? configs = JsonSerializer.Deserialize<GitRepoSyncConfiguration[]>(json);

            if (configs == null || configs.Length == 0)
            {
                configurations = new List<GitRepoSyncConfiguration>();
            }
            else
            {
                configurations = configs.ToList();
            }
        }
        else
        {
            configurations = new List<GitRepoSyncConfiguration>();
        }

        GitRepoSyncConfiguration? match = configurations.Where(x => x.Name == config.Name).FirstOrDefault();

        if (match != null)
        {
            configurations.Remove(match);
            configurations.Add(config);
        }
        else
        {
            configurations.Add(config);
        }

        Save(configurations);
    }

    public void Remove(string configName)
    {
        if (string.IsNullOrEmpty(configName))
        {
            throw new ArgumentException($"'{nameof(configName)}' cannot be null or empty.", nameof(configName));
        }

        List<GitRepoSyncConfiguration> configurations;

        if (File.Exists(PathToConfigurationFile) == true)
        {
            string json = File.ReadAllText(PathToConfigurationFile);

            GitRepoSyncConfiguration[]? configs = JsonSerializer.Deserialize<GitRepoSyncConfiguration[]>(json);

            if (configs == null || configs.Length == 0)
            {
                configurations = new List<GitRepoSyncConfiguration>();
            }
            else
            {
                configurations = configs.ToList();
            }
        }
        else
        {
            configurations = new List<GitRepoSyncConfiguration>();
        }

        GitRepoSyncConfiguration? match = configurations.Where(x => x.Name == configName).FirstOrDefault();

        if (match != null)
        {
            configurations.Remove(match);
        }

        Save(configurations);
    }

    private void Save(List<GitRepoSyncConfiguration> configurations)
    {
        string? dirName = Path.GetDirectoryName(PathToConfigurationFile);

        if (dirName == null)
        {
            throw new InvalidOperationException($"Could not establish directory.");
        }

        if (Directory.Exists(dirName) == false)
        {
            Directory.CreateDirectory(dirName);
        }

        string json = JsonSerializer.Serialize<GitRepoSyncConfiguration[]>(configurations.ToArray());

        File.WriteAllText(PathToConfigurationFile, json);
    }
}
