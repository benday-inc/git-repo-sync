namespace Benday.GitRepoSync.Api
{
    public static class Constants
    {
        public const string ExeName = "gitreposync";

        public const string CommandArgumentNameExportCloneGitRepos = "export";
        public const string CommandArgumentNameExportReposAsConfigFile = "exportconfig";
        public const string CommandArgumentNameUpdateAllRepos = "update";
        public const string CommandArgumentNameListRepos = "listrepos";
        public const string CommandArgumentNameListCategories = "listcategories";

        public const string ConfigFileName = "gitreposync-config.json";
        public const string DefaultConfigurationName = "(default)";

        public const string CommandArgumentNameShowConfig = "showconfig";
        public const string CommandArgumentNameAddUpdateConfig = "addconfig";
        public const string CommandArgumentNameRemoveConfig = "removeconfig";
        public const string CommandArgumentNameListConfig = "listconfig";

        public const string ArgumentNameListCategories = "listcategories";
        public const string ArgumentNameConfigFile = "config";
        public const string ArgumentNameFromPath = "frompath";
        public const string ArgumentNameCodeFolderPath = "codefolderpath";
        public const string ArgumentNameQuickSync = "quicksync";
        public const string ArgumentNameCategory = "category";
        public const string ArgumentNameFilter = "filter";
        public const string CodeDirVariable = "%%CodeDir%%";
        public const string ArgumentNameParallel = "parallel";
        public const string ArgumentNameToFileName = "filename";

        public const string ArgumentNameConfigurationName = "config";
        public const string ArgumentNameConfigurationFile = "filename";        
        public const string ArgumentNameCodeDirectory = "codedir";        
    }
}
