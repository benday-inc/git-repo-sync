using System;

namespace Benday.GitRepoSync.ConsoleUi
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayUsage();
            }
            else
            {
                try
                {
                    RunCommand(args);
                }
                catch (MissingArgumentException)
                {

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }                
            }
        }

        private static void RunCommand(string[] args)
        {
            var commandName = args[0];

            if (commandName == Constants.CommandArgumentNameExportCloneGitRepos)
            {
                new ExportCloneGitReposCommand(args).Run();
            }
            else
            {
                DisplayUsage();
            }
        }

        private static void DisplayUsage()
        {

            Console.Error.WriteLine($"Missing args. The first arg is the command to run. Options: {Constants.CommandArgumentNameExportCloneGitRepos}");
        }
    }
}
