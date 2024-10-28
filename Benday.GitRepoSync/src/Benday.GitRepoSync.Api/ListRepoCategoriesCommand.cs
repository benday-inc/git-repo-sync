using Benday.CommandsFramework;

using System.Collections.Generic;

namespace Benday.GitRepoSync.Api;

[Command(
    Name = Constants.CommandArgumentNameListCategories,
    IsAsync = false,
    Description = "Lists the repository categories in the config file.")]
public class ListRepoCategoriesCommand : GitRepoConfigurationCommandBase
{
    public ListRepoCategoriesCommand(CommandExecutionInfo info, ITextOutputProvider outputProvider) : base(
        info,
        outputProvider)
    {
    }

    public override ArgumentCollection GetArguments()
    {
        ArgumentCollection args = new ArgumentCollection();

        AddCommonArguments(args);

        return args;
    }


    protected override void OnExecute()
    {
        ValidateConfiguration();

        List<RepositoryInfo> repos = GetRepositories();

        int totalCount = repos.Count;

        IOrderedEnumerable<string> categories = (from temp in repos
            select temp.Category)
                          .Distinct()
            .OrderBy(x => x);

        WriteLine($"*** Category List ***");

        foreach (string item in categories)
        {
            WriteLine($"{item}");
        }
    }
}
