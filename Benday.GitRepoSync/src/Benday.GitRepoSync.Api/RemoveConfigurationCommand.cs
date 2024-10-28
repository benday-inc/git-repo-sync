using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(Name = Constants.CommandArgumentNameRemoveConfig,
        Description = "Remove a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.",
        IsAsync = false)]
public class RemoveConfigurationCommand : SynchronousCommand
{
    public RemoveConfigurationCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString(Constants.ArgumentNameConfigurationName)
            .WithDescription("Name of the configuration")
            .AsRequired();

        return arguments;
    }

    protected override void OnExecute()
    {
        GitRepoSyncConfigurationManager.Instance.Remove(Arguments[Constants.ArgumentNameConfigurationName].Value);
    }
}