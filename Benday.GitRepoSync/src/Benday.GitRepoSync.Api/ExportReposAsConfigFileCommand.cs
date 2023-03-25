using Benday.CommandsFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Benday.GitRepoSync.Api
{
    [Command(Name = Constants.CommandArgumentNameExportReposAsConfigFile,
        IsAsync = false,
        Description = "Reads existing Git repositories and outputs configuration information to config file.")]
    public class ExportReposAsConfigFileCommand : SynchronousCommand
    {
        public ExportReposAsConfigFileCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
               base(info, outputProvider)
        {

        }

        public override ArgumentCollection GetArguments()
        {
            var args = new ArgumentCollection();

            args.AddString(Constants.ArgumentNameFromPath)
                .WithDescription("Starting path for search");

            args.AddString(Constants.ArgumentNameCodeFolderPath)
                .WithDescription("Path for code directory. This becomes a variable in the config file.");

            args.AddString(Constants.ArgumentNameCategory)
                .WithDescription("Category name for this group of git repositories.");

            return args;
        }

        protected override void OnExecute()
        {
            string baseDir = Arguments.GetStringValue(Constants.ArgumentNameFromPath);

            var codeDirArgValue = Arguments.GetStringValue(Constants.ArgumentNameCodeFolderPath);
            var category = Arguments.GetStringValue(Constants.ArgumentNameCategory);

            string baseDirFormattedForConfigFile = baseDir;

            if (codeDirArgValue != null)
            {
                if (codeDirArgValue.StartsWith("~") == true)
                {
                    codeDirArgValue = codeDirArgValue.Replace("~", Environment.GetEnvironmentVariable("HOME"));
                }

                codeDirArgValue = Path.GetFullPath(codeDirArgValue);

                WriteLine($"INFO: Replacing code folder '{codeDirArgValue}' with variable '{Constants.CodeDirVariable}'.");

                if (Directory.Exists(codeDirArgValue) == false)
                {
                    throw new KnownException($"Directory for code folder '{codeDirArgValue}' does not exist.");
                }

                baseDirFormattedForConfigFile = baseDir.Replace(codeDirArgValue, Constants.CodeDirVariable);
            }
            else
            {
                Console.WriteLine("INFO: Not replacing code folder with variable.");
            }

            StringBuilder builder = new StringBuilder();

            // header line
            builder.AppendLine("quicksync,category,description,parent folder,giturl");

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
                            $"{false},{category},{repoName},{baseDirFormattedForConfigFile},{remote}");
                    }
                }

                builder.AppendLine();

                var script = builder.ToString();

                WriteLine(script);
            }
            else
            {
                throw new KnownException($"Base directory '{baseDir}' does not exist.");
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