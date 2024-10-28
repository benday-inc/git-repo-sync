using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;


[Command(Name = Constants.CommandArgumentNameOpenRepoConfigFile,
    IsAsync = false,
    Description = "Open the repo configuration file in the default text editor")]
public class OpenRepoConfigFileCommand : GitRepoConfigurationCommandBase
{
    public OpenRepoConfigFileCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        return args;
    }

    protected override void OnExecute()
    {
        ValidateConfiguration();

        var configFile = GetConfigFilename();

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