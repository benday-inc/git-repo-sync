# gitreposync
Do you have a gigantic sprawling mess of git repositories that you care about? 
Do you need to manage that sprawl on multiple machines? Need to set up your 
list of repos on a new machine? 

This utility helps you to manage that mess. 

### Key features:
- Quickly sync all your git repos or just a subset of your repos with a single command. The tool decides whether to do a 'git clone' or a 'git pull'
- Use a config file to define all the git repos you care about
- Take your existing mess of git repos and script it all into a config file
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

## Getting Started: Configuration
Everything starts with a configuration. After you've installed gitreposync, you'll need to 
run `gitreposync addconfig` to add a configuration. Each configuration has a list of git repositories and a source code directory. 

Configurations are named and you can have as many as you'd like.

### Suggestion: Put your repo config file into a cloud storage provider
I think you're going to want to use your repo config file (aka. your list of repositories) across multiple machines.
My suggestion is that you create that file in a directory that's synced to a cloud storage provider like OneDrive or Dropbox.

### Why does the tool ask you for a source code directory?
This tool is meant to be usable across OS platforms. The directory path where you store your source code is
going to be completely different if you're working on Windows, Mac, or Linux -- but the list of git repositories that you 
care about is probably the same.  

My guess is that you'll put your config file into a cloud storage provider like OneDrive or Dropbox.

When you add a git repository to the configuration, the source code directory value for that repo gets replaced with a 
variable ('%%CODE_DIR%%'). When you do a `gitreposync update`, the local version of the code directory is automatically set into that code dir variable so that you can share a repo config file across multiple machines and operating systems 
without having to change your config file.

### Set a Default Configuration
There's one default configuration named `(default)`. If you only care about one list of git repositories, then all you'll need to do is to run `gitreposync addconfig /codedir:c:\code /filename:c:\onedrive\gitreposync\gitreposync.csv` and that will set your default configuration. Done.

### Additional Named Configurations
If you want to add additional named configurations, you'll run `gitreposync addconfig /config:{name} ...`. 

### Running Commands
Once you've set a default configuration, you can run any gitreposync command without having to specify any additional URL or PAT info.  

If you want to run a command against a different list of git repo configs that is NOT your default, you'll need to supply the `/config:{name}`.

### Managing Configurations
To add new configurations or modify an existing configuration, use the `gitreposync addconfig` command. You can list your configurations using the `gitreposync listconfig` command. To delete a configuration, use the `gitreposync removeconfig` command.

## Getting Started: Scripting Your Current Git Repository Tree

Once you've run `gitreposync addconfig` and set up your config file path, you can start adding repositories to your config.
You can either do that one repo at a time using `gitreposync addrepo` or in bulk.

### Add one repository

This is the easiest way to get going. Open a terminal (aka. command prompt) window and `cd` to your git repo directory.
Let's say that your git repository is in `c:\code\my-git-repo`. Once you've done `cd \code\my-git-repo`, you can run
`gitreposync addrepo` and the tool will automatically add that repo to the config.

There are additional options such as quick sync and category that you also might want to configure.  The `addrepo` command
can be run multiple times and it will update the config for your repo.

To view the additional options, run `gitreposync addrepo --help`.

### Add multiple repositories

If you're scripting a lot of git repositories that are already on disk, you can run `gitreposync exportconfig`. This command 
will look at all the child directories under your current directory and export the repo config in comma-separated value (CSV)
format. The CSV data is printed to the console -- it does NOT actually update your repo config file.  Put another way, you'll
need to manually copy the CSV data into your repo config file.  

To open your repo config file run `gitreposync openconfig`. That command probably opens the repo config file in a text editor but you might find that it's helpful to open that config file in Excel.  

## Getting Started: Sync All Your Git Repositories

Once you have your repository config file set up, you can start bringing down your code. To do this, you'll run `gitreposync update`.
That command will perform either a git clone or a git pull to make sure that you have the latest code on your machine.

### Quick Sync
There are probably some repositories that you work with all the time and that have frequent changes. 
You can mark a configured git repository as a 'quick sync' repository using `gitreposync addrepo /quicksync`. 

To update just your quick sync repositories, run `gitreposync quicksync`

### Category
You can also organize your repositories by category. You can set the category for a repository using `gitreposync addrepo /category:{category-name}`. 

To update just your quick sync repositories, run `gitreposync update /category:{category-name}`

FYI, the category filter matches by the exact category name, not the partial category name.

### Filter
Sometimes you want to update repositories by name. To do this, you can specify a filter.

To update your git repositories using a filter, run `gitreposync update /filter:{value}`. It will search the configured 
git repositories and clone/pull anything that contains the filter value in the name, category, or URL.

### Combining filters and options
Yes, you can combine these filters and options.  

### Experimental: Multithreaded Sync
Feeling impatient?  Don't care that the messages displayed in the tool might be kind of a mess?  Try running `gitreposync update /parallel` 
to run the update command in multi-threaded mode.  I guarantee that the displayed messages will be a complete mess...but since it runs 
using multiple cores/processors, it'll probably run a bit faster.  
## Commands
| Command Name | Description |
| --- | --- |
| [addconfig](#addconfig) | Add or update a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| [addrepo](#addrepo) | Add or update a repo to the list of configured repositories. NOTE: Repository URL is the unique identifier |
| [exportconfig](#exportconfig) | Reads existing Git repositories and outputs configuration information to config file. |
| [info](#info) | Gets the configuration info for the current repo |
| [listconfig](#listconfig) | List a your git repo sync configurations. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| [listcategories](#listcategories) | Lists the repository categories in the config file. |
| [listrepos](#listrepos) | Reads config file and lists the configured repositories. |
| [openconfig](#openconfig) | Open the repo configuration file in the default text editor |
| [quicksync](#quicksync) | Performs a update on all quick sync repos. EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess. |
| [removeconfig](#removeconfig) | Remove a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory. |
| [removerepo](#removerepo) | Remove a repo from the list of configured repositories. NOTE: Repository URL is the unique identifier |
| [update](#update) | Performs a 'git clone' or 'git pull' for each configured git repository. |
## <a name="addconfig"></a> addconfig
**Add or update a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Name of the configuration |
| filename | Required | String | Configuration file path |
| codedir | Required | String | Code directory value. Note: this is used as the code variable value  '%%CodeDir%%' in your config file. |
## <a name="addrepo"></a> addrepo
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
## <a name="exportconfig"></a> exportconfig
**Reads existing Git repositories and outputs configuration information to config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| frompath | Required | String | Starting path for search. NOTE: this only checks immediate child directories. |
| codefolderpath | Required | String | Path for code directory. This becomes a variable in the config file. |
| category | Required | String | Category name for this group of git repositories. |
| filename | Optional | String | Writes configuration to file name |
## <a name="info"></a> info
**Gets the configuration info for the current repo**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## <a name="listconfig"></a> listconfig
**List a your git repo sync configurations. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Name of the configuration |
## <a name="listcategories"></a> listcategories
**Lists the repository categories in the config file.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## <a name="listrepos"></a> listrepos
**Reads config file and lists the configured repositories.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| filter | Optional | String | Filter repos by partial string value |
| category | Optional | String | Filter repos by category value. NOTE: this matches by full string |
| quicksync | Optional | Boolean | Filter repos by 'quick sync' value |
## <a name="openconfig"></a> openconfig
**Open the repo configuration file in the default text editor**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
## <a name="quicksync"></a> quicksync
**Performs a update on all quick sync repos. EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| singlethread | Optional | Boolean | Runs the repo update operations single threaded. This turns off the experimental feature of running multithreaded. Running single threaded will fix the messed up message display. |
## <a name="removeconfig"></a> removeconfig
**Remove a git repo sync configuration. A git repo sync configuration is the list of repositories you care about plus your local code directory.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Required | String | Name of the configuration |
## <a name="removerepo"></a> removerepo
**Remove a repo from the list of configured repositories. NOTE: Repository URL is the unique identifier**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| url | Optional | String | Repository URL value. NOTE: If not supplied, the repo URL for the current directory is used |
## <a name="update"></a> update
**Performs a 'git clone' or 'git pull' for each configured git repository.**
### Arguments
| Argument | Is Optional | Data Type | Description |
| --- | --- | --- | --- |
| config | Optional | String | Configuration name to use |
| filter | Optional | String | Filter repos by partial string value |
| category | Optional | String | Filter repos by category value. NOTE: this matches by full string |
| quicksync | Optional | Boolean | Filter repos by 'quick sync' value |
| parallel | Optional | Boolean | EXPERIMENTAL: runs the repo synchronizations in parallel. It runs a lot faster but the messages written to the console WILL definitely be a mess. |
