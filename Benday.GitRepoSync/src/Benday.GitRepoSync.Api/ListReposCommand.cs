using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Benday.GitRepoSync.Api;


[Command(
    Name = Constants.CommandArgumentNameListRepos,
    IsAsync = false,
    Description = "Reads config file and lists the configured repositories.")]
public class ListReposCommand : GitRepoConfigurationCommandBase
{
    public ListReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(info, outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        AddRepoFilters(args);

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        List<RepositoryInfo> repos = GetMatchingRepositories();

        int count = repos.Count();

        WriteLine($"Matching repositories: {count}");

        int index = 0;

        foreach (RepositoryInfo repo in repos)
        {
            index++;
            WriteLine($"*** REPOSITORY {index} ***");
            WriteRepositoryInfo(repo);
            WriteLine(string.Empty);
        }
    }
}
