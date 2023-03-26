﻿using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(Name = Constants.CommandArgumentNameAddUpdateConfig,
        Description = "Add or update an Azure DevOps configuration. For example, which server or account plus auth information.",
        IsAsync = false)]
public class AddUpdateConfigurationCommand : SynchronousCommand
{
    public AddUpdateConfigurationCommand(
        CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        var arguments = new ArgumentCollection();

        arguments.AddString(Constants.ArgumentNameConfigurationName)
            .WithDescription("Name of the configuration")
            .AsNotRequired();
        arguments.AddString(Constants.ArgumentNameToken)
            .WithDescription("PAT for this collection")
            .AsRequired();
        
        return arguments;
    }

    protected override void OnExecute()
    {
        var configName = Constants.DefaultConfigurationName;

        if (Arguments[Constants.ArgumentNameConfigurationName].HasValue == true)
        {
            configName =
                Arguments[Constants.ArgumentNameConfigurationName].Value;
        }

        var config = new GitRepoSyncConfiguration()
        {
            Token = Arguments[Constants.ArgumentNameToken].Value,
            Name = configName
        };

        GitRepoSyncConfigurationManager.Instance.Save(config);
    }
}