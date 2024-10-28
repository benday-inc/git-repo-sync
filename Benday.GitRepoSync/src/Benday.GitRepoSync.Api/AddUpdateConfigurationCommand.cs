using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = Constants.CommandArgumentNameAddUpdateConfig,
    Description = "Add or update a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.",
    IsAsync = false)]
public class AddUpdateConfigurationCommand : GitRepoConfigurationCommandBase
{
    public AddUpdateConfigurationCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection arguments = new ArgumentCollection();

        arguments.AddString(Constants.ArgumentNameConfigurationName)
            .WithDescription("Name of the configuration")
            .AsNotRequired();
        arguments.AddString(Constants.ArgumentNameConfigurationFile)
            .WithDescription("Configuration file path")
            .AsRequired();
        arguments.AddString(Constants.ArgumentNameCodeDirectory)
            .WithDescription(
                $"Code directory value. Note: this is used as the code variable value " +
                    $" '{Constants.CodeDirVariable}' in your config file.")
            .AsRequired();

        return arguments;
    }

    protected override void OnExecute()
    {
        string configName = Constants.DefaultConfigurationName;

        if (Arguments[Constants.ArgumentNameConfigurationName].HasValue == true)
        {
            configName =
                Arguments[Constants.ArgumentNameConfigurationName].Value;
        }

        GitRepoSyncConfiguration config = new GitRepoSyncConfiguration()
        {
            ConfigurationFilePath = GetPath(Arguments.GetStringValue(Constants.ArgumentNameConfigurationFile)),
            CodeDirectoryValue = GetPath(Arguments.GetStringValue(Constants.ArgumentNameCodeDirectory)),
            Name = configName
        };

        GitRepoSyncConfigurationManager.Instance.Save(config);
    }
}
