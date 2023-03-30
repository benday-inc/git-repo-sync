using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(Name = Constants.CommandArgumentNameListConfig,
        Description = "List a your git repo sync configurations. A git repo sync configuration is the list of repositories you care about plus your local code directory.",
        IsAsync = false)]
public class ListConfigurationCommand : SynchronousCommand
{
    public ListConfigurationCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString(Constants.ArgumentNameConfigurationName)
            .WithDescription("Name of the configuration")
            .AsNotRequired();

        return arguments;
    }

    protected override void OnExecute()
    {
        if (Arguments[Constants.ArgumentNameConfigurationName].HasValue)
        {
            var configName = Arguments[Constants.ArgumentNameConfigurationName].Value;

            var config = GitRepoSyncConfigurationManager.Instance.Get(configName);

            Print(config, configName);
        }
        else
        {
            var configs = GitRepoSyncConfigurationManager.Instance.GetAll();

            Print(configs);
        }
    }

    private void Print(GitRepoSyncConfiguration[] configs)
    {
        if (configs.Length == 0)
        {
            WriteLine("No configurations");
        }
        else
        {
            foreach (var config in configs)
            {
                Print(config, config.Name);
            }
        }
    }

    private void Print(GitRepoSyncConfiguration? config, string configName)
    {
        if (config == null)
        {
            WriteLine($"Configuration name '{configName}' not found");
        }
        else
        {
            WriteLine("***");
            WriteLine($"Name: {config.Name}");
            WriteLine($"Configuration File: {config.ConfigurationFilePath}");
            WriteLine($"Code Directory: {config.CodeDirectoryValue}");
        }
    }
}
