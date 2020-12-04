using System;

namespace Benday.GitRepoSync.ConsoleUi
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine($"Missing args. The first arg is the command to run. Options: {Constants.CommandArgumentNameExportGitRepos}");
            }
            else
            {
                new ExportGitReposCommand(args).Run();
            }
        }

        
    }
}
