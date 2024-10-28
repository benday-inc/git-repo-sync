using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Benday.CommandsFramework;

namespace Benday.GitRepoSync.Api;

[Command(Name = Constants.CommandArgumentNameListCategories,
    IsAsync = false,
    Description = "Lists the repository categories in the config file.")]
public class ListRepoCategoriesCommand : GitRepoConfigurationCommandBase
{
    public ListRepoCategoriesCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) :
           base(info, outputProvider)
    {

    }

    public override ArgumentCollection GetArguments()
    {
        var args = new ArgumentCollection();

        AddCommonArguments(args);

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        var repos = GetRepositories();

        int totalCount = repos.Count;

        var categories = (from temp in repos
                          select temp.Category)
                          .Distinct().OrderBy(x => x);

        WriteLine($"*** Category List ***");

        foreach (var item in categories)
        {
            WriteLine($"{item}");
        }
    }
}