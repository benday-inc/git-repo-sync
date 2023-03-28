using System;

namespace Benday.GitRepoSync.Api
{
    public class KnownException : Exception
    {
        public KnownException(string message) : base(message) { }

    }
}
