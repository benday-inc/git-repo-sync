using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;


[Command(Name = Constants.CommandArgumentNameListRepos,
    IsAsync = false,
    Description = "Reads config file and lists the configured repositories.")]
public class ListReposCommand : GitRepoConfigurationCommandBase
{
    public ListReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        AddRepoFilters(args);

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        List<RepositoryInfo> repos = GetMatchingRepositories();

        var count = repos.Count();

        WriteLine($"Matching repositories: {count}");

        var index = 0;

        foreach (var repo in repos)
        {
            index++;
            WriteLine($"*** REPOSITORY {index} ***");
            WriteRepositoryInfo(repo);
            WriteLine(string.Empty);
        }
    }
}