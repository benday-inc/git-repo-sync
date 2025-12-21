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


## Commands
| Command Name | Description |
| --- | --- |
| addconfig | Add or update a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| addrepo | Add or update a repo to the list of configured repositories. NOTE: Repository URL is the unique identifier |
| status | Checks all configured repositories for pending changes and unpushed commits. |
| exportconfig | Reads existing Git repositories and outputs configuration information to config file. |
| info | Gets the configuration info for the current repo |
| listconfig | List a your git repo sync configurations. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| listcategories | Lists the repository categories in the config file. |
| listrepos | Reads config file and lists the configured repositories. |
| openconfig | Open the repo configuration file in the default text editor |
| quicksync | Performs a update on all quick sync repos. EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess. |
| removeconfig | Remove a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| removerepo | Remove a repo from the list of configured repositories. NOTE: Repository URL is the unique identifier |
| update | Performs a 'git clone' or 'git pull' for each configured git repository. |
| get-configuration | Display all configuration values or a specific configuration value |
| remove-configuration | Remove a configuration value |
| set-configuration | Set a configuration value |
| get-configuration | Display all configuration values or a specific configuration value |
| remove-configuration | Remove a configuration value |
| set-configuration | Set a configuration value |
| get-configuration | Display all configuration values or a specific configuration value |
| remove-configuration | Remove a configuration value |
| set-configuration | Set a configuration value |
## addconfig
**Add or update a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Name of the configuration |
| filename | Required | String | Configuration file path |
| codedir | Required | String | Code directory value. Note: this is used as the code variable value  '%%CodeDir%%' in your config file. |
## addrepo
**Add or update a repo to the list of configured repositories. NOTE: Repository URL is the unique identifier**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| name | Optional | String | Human readable name for the repository |
| parentdir | Optional | String | Parent directory for this repository. Essentially, where do you want this on disk? |
| quicksync | Optional | Boolean | Add repo to quick sync |
| overwrite | Optional | Boolean | Overwrites an existing repo config |
| category | Optional | String | Category for the repository |
| url | Optional | String | Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used |
## status
**Checks all configured repositories for pending changes and unpushed commits.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| filter | Optional | String | Filter repos by partial string value |
| category | Optional | String | Filter repos by category value. NOTE: this matches by full string |
| quicksync | Optional | Boolean | Filter repos by 'quick sync' value |
## exportconfig
**Reads existing Git repositories and outputs configuration information to config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| frompath | Required | String | Starting path for search. NOTE: this only checks immediate child directories. |
| codefolderpath | Required | String | Path for code directory. This becomes a variable in the config file. |
| category | Required | String | Category name for this group of git repositories. |
| filename | Optional | String | Writes configuration to file name |
## info
**Gets the configuration info for the current repo**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## listconfig
**List a your git repo sync configurations. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Name of the configuration |
## listcategories
**Lists the repository categories in the config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## listrepos
**Reads config file and lists the configured repositories.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| filter | Optional | String | Filter repos by partial string value |
| category | Optional | String | Filter repos by category value. NOTE: this matches by full string |
| quicksync | Optional | Boolean | Filter repos by 'quick sync' value |
## openconfig
**Open the repo configuration file in the default text editor**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## quicksync
**Performs a update on all quick sync repos. EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| singlethread | Optional | Boolean | Runs the repo update operations single threaded. This turns off the experimental feature of running multithreaded. Running single threaded will fix the messed up message display. |
## removeconfig
**Remove a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Required | String | Name of the configuration |
## removerepo
**Remove a repo from the list of configured repositories. NOTE: Repository URL is the unique identifier**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| url | Optional | String | Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used |
## update
**Performs a 'git clone' or 'git pull' for each configured git repository.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| filter | Optional | String | Filter repos by partial string value |
| category | Optional | String | Filter repos by category value. NOTE: this matches by full string |
| quicksync | Optional | Boolean | Filter repos by 'quick sync' value |
| parallel | Optional | Boolean | EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess. |
## get-configuration
**Display all configuration values or a specific configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Optional | String | Name of the configuration name to display |
## remove-configuration
**Remove a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration name to display |
## set-configuration
**Set a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration value to set |
| value | Required | String | Value of the configuration |
## get-configuration
**Display all configuration values or a specific configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Optional | String | Name of the configuration name to display |
## remove-configuration
**Remove a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration name to display |
## set-configuration
**Set a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration value to set |
| value | Required | String | Value of the configuration |
## get-configuration
**Display all configuration values or a specific configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Optional | String | Name of the configuration name to display |
## remove-configuration
**Remove a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration name to display |
## set-configuration
**Set a configuration value**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| name | Required | String | Name of the configuration value to set |
| value | Required | String | Value of the configuration |
