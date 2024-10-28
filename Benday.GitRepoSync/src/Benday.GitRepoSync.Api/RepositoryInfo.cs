using System;

namespace Benday.GitRepoSync.Api
{
    public class RepositoryInfo
    {
        public bool IsQuickSync { get; set; }

        public string Category { get; set; } = string.Empty;

        public string ParentFolder { get; set; } = string.Empty;

        public string GitUrl { get; set; } = string.Empty;

        public string RepositoryName { get; set; } = string.Empty;

        private bool ContainsFilterCaseInsensitive(string value, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                throw new ArgumentOutOfRangeException(nameof(filter), "Filter cannot be null or empty.");
            }

            string valueToLower = value.ToLower();
            string filterToLower = filter.ToLower();

            return valueToLower.Contains(filterToLower);
        }

        public bool MatchesFilter(string filter)
        {
            return ContainsFilterCaseInsensitive(Category, filter) ||
                ContainsFilterCaseInsensitive(GitUrl, filter) ||
                ContainsFilterCaseInsensitive(RepositoryName, filter);
        }
    }
}
