using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

public static class ExtensionMethods
{
    public static CommandExecutionInfo GetCloneOfArguments(
        this CommandExecutionInfo execInfo, string commandName)
    {
        if (execInfo is null || execInfo.Arguments is null)
        {
            throw new ArgumentNullException(nameof(execInfo));
        }

        var argsClone = execInfo.Arguments.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        var returnValue = new CommandExecutionInfo();
        returnValue.Arguments = argsClone;
        returnValue.CommandName = commandName;

        return returnValue;
    }
}