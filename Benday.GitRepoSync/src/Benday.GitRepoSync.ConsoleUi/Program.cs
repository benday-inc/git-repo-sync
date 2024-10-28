using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using Benday.CommandsFramework;
using Benday.GitRepoSync.Api;

namespace Benday.GitRepoSync.ConsoleUi;
class Program
{
    static void Main(string[] args)
    {
        var assembly = typeof(Constants).Assembly;

        var versionInfo =
            FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location);

        var options = new DefaultProgramOptions();

        options.Version = $"v{versionInfo.FileVersion}";
        options.ApplicationName = "Git Repository Sync Utility";
        options.Website = "https://www.benday.com";
        options.UsesConfiguration = false;

        var program = new DefaultProgram(options, assembly);

        program.Run(args);

    }
}
