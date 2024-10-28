using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = Constants.CommandArgumentNameQuickSync,
    IsAsync = false,
    Description = "Performs a update on all quick sync repos. " + "EXPERIMENTAL: runs the repo synchronizations in parallel. It runs " + "a lot faster but the messages written to the console WILL " +
    "definitely be a mess.")]
public class QuickSyncCommand : GitRepoConfigurationCommandBase
{
    public QuickSyncCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        args.AddBoolean(Constants.ArgumentNameSingleThreaded)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription(
                "Runs the repo update operations single threaded. " +
                    "This turns off the experimental feature of running multithreaded. " +
                    "Running single threaded will fix the messed up message display.");

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        bool runMultithreaded = !Arguments.HasValue(Constants.ArgumentNameSingleThreaded);

        CommandExecutionInfo newArgs = ExecutionInfo.GetCloneOfArguments(Constants.CommandArgumentNameUpdateAllRepos);

        newArgs.AddArgumentValue(Constants.ArgumentNameConfigurationName, GetConfigurationName());

        newArgs.AddArgumentValue(Constants.ArgumentNameParallel, runMultithreaded.ToString());

        newArgs.AddArgumentValue(Constants.ArgumentNameQuickSync, true.ToString());

        UpdateAllReposCommand command = new UpdateAllReposCommand(newArgs, _OutputProvider);

        command.Execute();
    }
}
