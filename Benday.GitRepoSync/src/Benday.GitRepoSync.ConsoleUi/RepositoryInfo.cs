using System;
namespace Benday.GitRepoSync.ConsoleUi
{
    public class RepositoryInfo
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string ParentFolder { get; set; }
        public string GitUrl { get; set; }
        public string RepositoryName { get; set; }
    }
}
