# gitreposync
Do you have a gigantic sprawling mess of git repositories that you care about? 
Do you need to manage that sprawl on multiple machines? Need to set up your 
list of repos on a new machine? 

This utility helps you to manage that mess. 

A collection of utilities for working with .NET Core Solutions and Projects.

Written by Benjamin Day  
Pluralsight Author | Microsoft MVP | Scrum.org Professional Scrum Trainer  
[https://www.benday.com](https://www.benday.com)

[https://www.slidespeaker.ai](https://www.slidespeaker.ai)

[info@benday.com](mailto:info@benday.com) 

YouTube: [https://www.youtube.com/@_benday](https://www.youtube.com/@_benday)

*Got ideas for utilities you'd like to see? Found a bug? Let us know by submitting an issue https://github.com/benday-inc/git-repo-sync/issues*. *Want to contribute? Submit a pull request.*

## Installing
The slnutil is distributed as a .NET Core Tool via NuGet. To install it go to the command prompt and type  
`dotnet tool install gitreposync -g`

### Prerequisites
- You'll need to install .NET Core 8 from https://dotnet.microsoft.com/

## Key features:
- Quickly sync all your git repos or just a subset of your repos with a single command. The tool decides whether to do a 'git clone' or a 'git pull'
- Use a config file to define all the git repos you care about
- Take your existing mess of git repos and script it all into a config file
- Define categories of git repos in the config file
- Mark git repos as a 'quick sync' repository
- Quickly sync all your 'quick sync' repos
- Quickly sync just a category of repos


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