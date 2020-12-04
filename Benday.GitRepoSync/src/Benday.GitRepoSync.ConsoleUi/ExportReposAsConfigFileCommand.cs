using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Benday.GitRepoSync.ConsoleUi
{
    public class ExportReposAsConfigFileCommand : CommandBase
    {
        public ExportReposAsConfigFileCommand(string[] args) : base(args)
        {
            
        }

        protected override string CommandArgumentName
        {
            get
            {
                return Constants.CommandArgumentNameExportCloneGitRepos;
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

            // header line
            builder.AppendLine("description,parent folder,giturl");

            if (Directory.Exists(baseDir) == true)
            {
                var dirs = Directory.EnumerateDirectories(baseDir);

                foreach (var dir in dirs)
                {
                    var remote = GetGitRepoRemote(dir).Trim();
                    
                    if (String.IsNullOrWhiteSpace(remote) == false)
                    {
                        var repoName = GetGitRepoName(remote).Replace("-", " ");

                        builder.AppendLine(
                            $"{repoName},{baseDir},{remote}");
                    }
                }

                builder.AppendLine();

                string script = builder.ToString();

                Console.WriteLine(script);
            }
            else
            {
                Console.Error.WriteLine("directory does not exist");
            }
        }

        private string GetGitRepoName(string gitRepoUrl)
        {
            var repoUri = new Uri(gitRepoUrl);
            
            var lastToken = repoUri.Segments.Last();

            if (String.IsNullOrWhiteSpace(lastToken) == true)
            {
                return String.Empty;
            }
            else if (lastToken.EndsWith(".git") == true)
            {
                return lastToken.Replace(".git", String.Empty);
            }
            else
            {
                return lastToken;
            }
        }

            private string GetGitRepoRemote(string dir)
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
