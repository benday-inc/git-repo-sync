# gitreposync
A utility for managing and synchronizing multiple git repositories across machines.

[![NuGet](https://img.shields.io/nuget/v/gitreposync.svg)](https://www.nuget.org/packages/gitreposync/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/gitreposync.svg)](https://www.nuget.org/packages/gitreposync/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Written by Benjamin Day
Pluralsight Author | Microsoft MVP | Scrum.org Professional Scrum Trainer
https://www.benday.com
https://www.slidespeaker.ai
info@benday.com
YouTube: https://www.youtube.com/@_benday

## Key features

* Sync all your git repos with a single command - automatically decides between `git clone` or `git pull`
* Share your repository list across Windows, Mac, and Linux using a portable config file
* Store your config in cloud storage (OneDrive, Dropbox) to keep machines in sync
* Organize repositories by category
* Mark repositories as "quick sync" for frequently-used repos
* Filter updates by category, name, or partial match
* Check repository status for pending changes and unpushed commits
* Export your existing repository structure to a config file
* Experimental multi-threaded sync for faster updates

## Suggestions, Problems, or Bugs?

*Got ideas for utilities you'd like to see? Found a bug? Let us know by submitting an issue https://github.com/benday-inc/git-repo-sync/issues*. *Want to contribute? Submit a pull request.*

## Installing
The gitreposync utility is distributed as a .NET Tool via NuGet. To install it go to the command prompt and type
`dotnet tool install gitreposync -g`

### Prerequisites
- .NET 8 or later from https://dotnet.microsoft.com/

