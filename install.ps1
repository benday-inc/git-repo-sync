[CmdletBinding()]

param([Parameter(HelpMessage='Uninstall before installing')]
    [ValidateNotNullOrEmpty()]
    [switch]
    $reinstall)

if ($reinstall -eq $true)
{
    &.\uninstall.ps1
}

dotnet build .\Benday.GitRepoSync

dotnet tool install --global --add-source .\Benday.GitRepoSync\src\Benday.GitRepoSync.ConsoleUi\bin\Debug\ gitreposync
                                          
