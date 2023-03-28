using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Benday.GitRepoSync.Api;


[Command(Name = Constants.CommandArgumentNameListRepos,
    IsAsync = false,
    Description = "Reads config file and lists the configured repositories.")]
public class ListReposCommand : GitRepoConfigurationCommand
{
    public ListReposCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        args.AddString(Constants.ArgumentNameFilter)
            .AsNotRequired()
            .WithDescription("Filter repos by string value");

        args.AddBoolean(Constants.ArgumentNameQuickSync)
            .AsNotRequired()
            .AllowEmptyValue()
            .WithDescription("Show repos that are marked as 'quick sync'");

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();
                
        var filterMode = Arguments.HasValue(Constants.ArgumentNameFilter);
        var filter = Arguments.GetStringValue(Constants.ArgumentNameFilter);

        bool isQuickSyncMode = Arguments.GetBooleanValue(Constants.ArgumentNameQuickSync);
        
        var repos = GetRepositories();

        if (isQuickSyncMode == true)
        {
            repos = repos.Where(r => r.IsQuickSync).ToList();
        }

        if (filterMode == true)
        {
            repos = repos.Where(r => r.MatchesFilter(filter)).ToList();
        }

        var count = repos.Count();

        WriteLine($"Matching repositories: {count}");

        var index = 0;

        foreach (var repo in repos)
        {
            index++;

            WriteLine($"*** REPOSITORY {index} ***");
            WriteLine($"Name       : {repo.RepositoryName}");
            WriteLine($"Category   : {repo.Category}");
            WriteLine($"Description: {repo.Description}");
            WriteLine($"Quick Sync : {repo.IsQuickSync}");
            WriteLine($"URL        : {repo.GitUrl}");
            WriteLine(string.Empty);
        }
    }     


    


    
}
