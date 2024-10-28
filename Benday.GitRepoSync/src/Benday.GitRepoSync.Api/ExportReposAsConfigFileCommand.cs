using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api
{
    [Command(Name = Constants.CommandArgumentNameExportReposAsConfigFile,
        IsAsync = false,
        Description = "Reads existing Git repositories and outputs configuration information to config file.")]
    public class ExportReposAsConfigFileCommand : GitRepoConfigurationCommandBase
    {
        public ExportReposAsConfigFileCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
               base(info, outputProvider)
        {

        }

        public override ArgumentCollection GetArguments()
        {
            var args = new ArgumentCollection();

            args.AddString(Constants.ArgumentNameFromPath)
                .WithDescription("Starting path for search. NOTE: this only checks immediate child directories.");

            args.AddString(Constants.ArgumentNameCodeFolderPath)
                .WithDescription("Path for code directory. This becomes a variable in the config file.");

            args.AddString(Constants.ArgumentNameCategory)
                .WithDescription("Category name for this group of git repositories.");

            args.AddString(Constants.ArgumentNameToFileName)
                .AsNotRequired()
                .WithDescription("Writes configuration to file name");

            return args;
        }

        protected override void OnExecute()
        {
            var baseDir = Arguments.GetStringValue(Constants.ArgumentNameFromPath);
            var codeDirArgValue = Arguments.GetStringValue(Constants.ArgumentNameCodeFolderPath);
            var category = Arguments.GetStringValue(Constants.ArgumentNameCategory);

            if (Path.IsPathFullyQualified(baseDir) == false)
            {
                baseDir = Path.Combine(Environment.CurrentDirectory, baseDir);
            }

            if (Path.IsPathFullyQualified(codeDirArgValue) == false)
            {
                codeDirArgValue = Path.Combine(Environment.CurrentDirectory, codeDirArgValue);
            }

            baseDir = new DirectoryInfo(baseDir).FullName;
            codeDirArgValue = new DirectoryInfo(codeDirArgValue).FullName;

            WriteLine($"Base directory: {baseDir}");
            WriteLine($"Code directory: {codeDirArgValue}");

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

            WriteLine("");

            StringBuilder builder = new();

            // header line
            builder.AppendLine("quicksync,category,description,parent folder,giturl");

            if (Directory.Exists(baseDir) == true)
            {
                var dirs = Directory.EnumerateDirectories(baseDir);

                var numberOfReposFound = 0;

                foreach (var dir in dirs)
                {
                    var remote = GetGitRepoRemote(dir).Trim();

                    if (String.IsNullOrWhiteSpace(remote) == false)
                    {
                        numberOfReposFound++;

                        var repoName = GetGitRepoName(remote)
                            .Replace("-", " ")
                            .Replace(",", " ");

                        builder.AppendLine(
                            $"{false},{category},{repoName},{baseDirFormattedForConfigFile},{remote}");
                    }
                }

                builder.AppendLine();

                if (numberOfReposFound == 0)
                {
                    WriteLine("");
                    WriteLine($"** Didn't find any git repos in '{baseDir}'. " +
                        $"Did you remember that this tool doesn't recurse directories? " +
                        "It only looks at immediate " +
                        "children of this directory.");
                }
                else
                {
                    var script = builder.ToString();

                    if (Arguments.HasValue(Constants.ArgumentNameToFileName) == false)
                    {
                        // output to console
                        WriteLine(script);
                    }
                    else
                    {
                        var toFileName = Arguments.GetStringValue(Constants.ArgumentNameToFileName);

                        if (Path.IsPathFullyQualified(toFileName) == false)
                        {
                            toFileName = Path.Combine(Environment.CurrentDirectory, toFileName);
                        }

                        toFileName = Path.GetFullPath(toFileName);

                        var directory = new DirectoryInfo(Path.GetDirectoryName(toFileName)!);

                        if (directory.Exists == false)
                        {
                            directory.Create();
                        }

                        File.WriteAllText(toFileName, script);
                        WriteLine($"Exported {numberOfReposFound} repos to config file '{toFileName}'.");
                    }
                }
            }
            else
            {
                throw new KnownException($"Base directory '{baseDir}' does not exist.");
            }
        }
    }
}