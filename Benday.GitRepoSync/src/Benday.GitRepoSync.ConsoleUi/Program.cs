using Benday.CommandsFramework;
using Benday.GitRepoSync.Api;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Benday.GitRepoSync.ConsoleUi
{
    class Program
    {
        static void Main(string[] args)
        {
            var util = new CommandAttributeUtility();

            if (args.Length == 0)
            {
                DisplayUsage(util);
            }
            else
            {
                try
                {
                    var command = util.GetCommand(args, typeof(Constants).Assembly);

                    if (command == null)
                    {
                        DisplayUsage(util);
                    }
                    else
                    {
                        var attr = util.GetCommandAttributeForCommandName(typeof(Constants).Assembly,
                            command.ExecutionInfo.CommandName);

                        if (attr == null)
                        {
                            throw new InvalidOperationException(
                                $"Could not get command attribute for command name '{command.ExecutionInfo.CommandName}'.");
                        }
                        else
                        {
                            if (attr.IsAsync == false)
                            {
                                var runThis = command as ISynchronousCommand;

                                if (runThis == null)
                                {
                                    throw new InvalidOperationException($"Could not convert type to {typeof(ISynchronousCommand)}.");
                                }
                                else
                                {
                                    runThis.Execute();
                                }
                            }
                            else
                            {
                                var runThis = command as IAsyncCommand;

                                if (runThis == null)
                                {
                                    throw new InvalidOperationException($"Could not convert type to {typeof(IAsyncCommand)}.");
                                }
                                else
                                {
                                    var temp = runThis.ExecuteAsync().GetAwaiter();

                                    temp.GetResult();
                                }
                            }
                        }
                    }
                }
                catch (KnownException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void DisplayUsage(CommandAttributeUtility util)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($"Git Repository Sync Utility");
            Console.WriteLine($"https://www.benday.com");
            Console.WriteLine();
            Console.WriteLine($"v{versionInfo.FileVersion}");
            Console.WriteLine();
            Console.WriteLine($"Available commands:");

            var assembly = typeof(Constants).Assembly;

            var commands = util.GetAvailableCommandAttributes(assembly);

            var longestName = commands.Max(x => x.Name.Length);

            foreach (var command in commands.OrderBy(x => x.Name))
            {
                Console.Write(GetNameWithPadding(command.Name, longestName));
                Console.WriteLine($" - {command.Description}");
            }
        }

        private static string GetNameWithPadding(string name, int padToLength)
        {
            var builder = new StringBuilder();

            builder.Append(name);
            builder.Append(' ', padToLength - name.Length);

            return builder.ToString();
        }
    }
}
