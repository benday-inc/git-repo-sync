using Benday.CommandsFramework;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Benday.GitRepoSync.Api
{
    //[Command(Name = Constants.CommandArgumentNameExportCloneGitRepos,
    //IsAsync = false,
    //Description = "Reads existing Git repositories and outputs configuration information to console.")]
    public class ExportCloneGitReposCommand : SynchronousCommand
    {

        public ExportCloneGitReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
            base(info, outputProvider)
        {

        }


        public override ArgumentCollection GetArguments()
        {
            var args = new ArgumentCollection();

            args.AddString(Constants.ArgumentNameFromPath)
                .WithDescription("Starting path");

            return args;
        }

        protected override void OnExecute()
        {            
            string baseDir = Arguments.GetStringValue(Constants.ArgumentNameFromPath);

            StringBuilder builder = new();

            if (Directory.Exists(baseDir) == true)
            {
                var dirs = Directory.EnumerateDirectories(baseDir);

                builder.AppendLine("mkdir " + baseDir);
                builder.AppendLine("cd " + baseDir);

                foreach (var dir in dirs)
                {
                    var remote = GetGitRepoRemote(dir).Trim();

                    if (String.IsNullOrWhiteSpace(remote) == false)
                    {
                        builder.AppendLine(
                            "git clone " + remote);
                    }
                }

                string script = builder.ToString();

                WriteLine(script);
            }
            else
            {
                throw new KnownException("directory does not exist");
            }
        }

        private static string GetGitRepoRemote(string dir)
        {
            var temp = new ProcessStartInfo
            {
                WorkingDirectory = dir,

                FileName = "git",

                Arguments = "remotes",

                CreateNoWindow = true,

                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var process = Process.Start(temp) ?? throw new InvalidOperationException($"Null process start return value.");

            process.WaitForExit();

            var output = process.StandardOutput.ReadLine();

            if (output != null)
            {
                output = output.Replace("origin	", String.Empty).Replace(" (fetch)", String.Empty);

                if (output.Contains('\t') == true)
                {
                    var tokens = output.Split('\t');

                    output = tokens.Last();
                }

                return output;
            }
            else
            {
                return string.Empty;
            }
        }


    }
}
