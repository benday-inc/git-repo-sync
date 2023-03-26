# gitreposync
Do you have a gigantic sprawling mess of git repositories that you care about? 
Do you need to manage that sprawl on multiple machines? Need to set up your 
list of repos on new machine? 
This utility helps you 
to manage that mess. 

### Key features:
- Quickly sync all your git repos or just a subset of your repos with a single command. The tool decides whether to do a 
'git clone' or a 'git pull'
- Use a config file to define all the git repos you care about
- Take your existing mess of git repos and script all into a config file
- Define categories of git repos in the config file
- Mark git repos as a 'quick sync' repository
- Quickly sync all your 'quick sync' repos
- Quickly sync just a category of repos

### About

Written by Benjamin Day  
Pluralsight Author | Microsoft MVP | Scrum.org Professional Scrum Trainer  
https://www.benday.com  
info@benday.com 

*Got ideas for git repo sync features you'd like to see? Found a bug? 
Let us know by submitting an issue https://github.com/benday-inc/git-repo-sync/issues*. *Want to contribute? Submit a pull request.*

## Installing
The gitreposync is distributed as a .NET Core Tool via NuGet. To install it go to the command prompt and type  
`dotnet tool install gitreposync -g`

### Prerequisites
- You'll need to install .NET Core 7 from https://dotnet.microsoft.com/

## Getting Started
Everything starts with a configuration. After you've installed gitreposync, you'll need to 
run `gitreposync addconfig` to add a configuration. A configuration is how you store the URL for your Azure DevOps instance and the personal access token (PAT) for authenticating to that instance.  

Configurations are named and you can have as many as you'd like.

### Set a Default Configuration
There's one default configuration named `(default)`. If you only work with one Azure DevOps instance, then all you'll need to do is to is run `gitreposync addconfig /url:{url} /pat:{pat}` and that will set your default configuration. 

### Additional Named Configurations
If you want to add additional named configurations, you'll run `gitreposync addconfig /config:{name} /url:{url} /pat:{pat}`. 

### Running Commands
Once you've set a default configuration, you can run any gitreposync command without having to specify any additional URL or PAT info.  

If you want to run a command against an Azure DevOps instance that is NOT your default, you'll need to supply the `/config:{name}`.

### Managing Configurations
To add new configuration or modify an existing configuration, use the `gitreposync addconfig` command. You can list your configurations using the `gitreposync listconfig` command. To delete a configuration, use the `gitreposync removeconfig` command.

## Commands
| Command Name | Description |
| --- | --- |
| [export](#export) | Reads existing Git repositories and outputs configuration information to console. |
| [exportconfig](#exportconfig) | Reads existing Git repositories and outputs configuration information to config file. |
| [update](#update) | Reads existing Git repositories and outputs configuration information to config file. |
## <a name="export"></a> export
**Reads existing Git repositories and outputs configuration information to console.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| frompath | Required | String | Starting path |
## <a name="exportconfig"></a> exportconfig
**Reads existing Git repositories and outputs configuration information to config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| frompath | Required | String | Starting path for search |
| codefolderpath | Required | String | Path for code directory. This becomes a variable in the config file. |
| category | Required | String | Category name for this group of git repositories. |
## <a name="update"></a> update
**Reads existing Git repositories and outputs configuration information to config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Required | String | Path to configuration file |
| codefolderpath | Required | String | Path for code directory variable in the config file. |
