using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Benday.GitRepoSync.ConsoleUi
{
    public class ExportGitReposCommand : CommandBase
    {
        public ExportGitReposCommand(string[] args) : base(args)
        {
            
        }

        protected override string CommandArgumentName
        {
            get
            {
                return Constants.CommandArgumentNameExportGitRepos;
            }
        }

        protected override List<string> GetRequiredArguments()
        {
            var argumentNames = new List<string>();

            argumentNames.Add(Constants.ArgumentNameFromPath);

            return argumentNames;
        }

        public override void Run()
        {
            string baseDir = GetArgumentValue(Constants.ArgumentNameFromPath);

            StringBuilder builder = new StringBuilder();

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

                Console.WriteLine(script);
            }
            else
            {
                Console.Error.WriteLine("directory does not exist");
            }
        }

        private static string GetGitRepoRemote(string dir)
        {
            var temp = new ProcessStartInfo();

            temp.WorkingDirectory = dir;

            temp.FileName = "git";

            temp.Arguments = "remotes";

            temp.CreateNoWindow = true;

            temp.UseShellExecute = false;
            temp.RedirectStandardOutput = true;

            var process = Process.Start(temp);

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
