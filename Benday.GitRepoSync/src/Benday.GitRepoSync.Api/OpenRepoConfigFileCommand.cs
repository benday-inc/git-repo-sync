using Benday.CommandsFramework;

using System;
using System.Diagnostics;
using System.Linq;

namespace Benday.GitRepoSync.Api;


[Command(
    Name = Constants.CommandArgumentNameOpenRepoConfigFile,
    IsAsync = false,
    Description = "Open the repo configuration file in the default text editor")]
public class OpenRepoConfigFileCommand : GitRepoConfigurationCommandBase
{
    public OpenRepoConfigFileCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();

        string configFile = GetConfigFilename();

        WriteLine($"Opening config file...'{configFile}'");

        if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            Process.Start("open", configFile);
        }
        else
        {
            Process.Start("notepad.exe", configFile);
        }
    }
}
