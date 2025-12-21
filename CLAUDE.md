# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Build the solution
dotnet build Benday.GitRepoSync/Benday.GitRepoSync.sln

# Run all tests
dotnet test Benday.GitRepoSync/Benday.GitRepoSync.sln

# Run a single test
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Install locally as a dotnet tool
dotnet pack Benday.GitRepoSync/src/Benday.GitRepoSync.ConsoleUi/Benday.GitRepoSync.ConsoleUi.csproj
dotnet tool install --global --add-source ./Benday.GitRepoSync/src/Benday.GitRepoSync.ConsoleUi/bin/Debug gitreposync
```

## Architecture

This is a .NET CLI tool (`gitreposync`) for managing multiple git repositories across machines. It uses the `Benday.CommandsFramework` NuGet package for command-line parsing and command discovery.

### Project Structure

- **Benday.GitRepoSync.Api** - Core library containing all commands and business logic
- **Benday.GitRepoSync.ConsoleUi** - Entry point that uses `DefaultProgram` from the framework to auto-discover commands via reflection
- **Benday.GitRepoSync.UnitTests** - MSTest-based unit tests

### Command Pattern

All commands inherit from `GitRepoConfigurationCommandBase` (which extends `SynchronousCommand`). Commands are auto-discovered via the `[Command]` attribute:

```csharp
[Command(Name = Constants.CommandArgumentNameXyz, IsAsync = false, Description = "...")]
public class MyCommand : GitRepoConfigurationCommandBase
{
    public override ArgumentCollection GetArguments() { /* define args */ }
    protected override void OnExecute() { /* implementation */ }
}
```

Command names are defined in `Constants.cs`. When adding a new command:
1. Add a constant in `Constants.cs`
2. Create a new class inheriting from `GitRepoConfigurationCommandBase`
3. Apply the `[Command]` attribute with the constant

### Configuration System

Two-level configuration:
1. **Global config** (`~/.gitreposync/gitreposync-config.json`) - JSON file managed by `GitRepoSyncConfigurationManager` (singleton). Contains named configurations, each pointing to a CSV file and code directory.
2. **Repository list** (CSV files) - Format: `quicksync,category,description,parent folder,giturl`

The `%%CodeDir%%` variable in CSV files gets replaced at runtime with the machine-specific code directory, enabling cross-platform config sharing.

### Git Operations

Git commands are executed via `System.Diagnostics.Process` (no LibGit2Sharp). The base class provides helpers:
- `GetGitRepoRemote(dir)` - runs `git remote -v`
- `GetGitRepoRootDirectory(dir)` - runs `git rev-parse --show-toplevel`
- `GetGitRepoName(url)` - extracts repo name from URL

### Testing Pattern

Tests use MSTest with `StringBuilderTextOutputProvider` for capturing output. Use `Utilities.InitializeTestModeConfigurationManager()` to set up an isolated configuration manager pointing to a temp directory.
