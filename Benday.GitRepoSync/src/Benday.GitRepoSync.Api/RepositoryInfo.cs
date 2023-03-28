using System;
namespace Benday.GitRepoSync.Api
{
    public class RepositoryInfo
    {
        public bool IsQuickSync { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ParentFolder { get; set; }
        public string GitUrl { get; set; }
        public string RepositoryName { get; set; }

        private bool ContainsFilterCaseInsensitive(string value, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(filter), "Filter cannot be null or empty.");
            }

            var valueToLower = value.ToLower();
            var filterToLower = filter.ToLower();

            return valueToLower.Contains(filterToLower);
        }

        public bool MatchesFilter(string filter)
        {
            return ContainsFilterCaseInsensitive(Category, filter) || 
                ContainsFilterCaseInsensitive(Description, filter) || 
                ContainsFilterCaseInsensitive(GitUrl, filter) || 
                ContainsFilterCaseInsensitive(RepositoryName, filter);
        }
    }
}
