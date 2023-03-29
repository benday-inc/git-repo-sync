dotnet test .\Benday.GitRepoSync

Copy-Item -Path .\Benday.GitRepoSync\generated-readme-files\README-for-nuget.md -Destination .
Copy-Item -Path .\Benday.GitRepoSync\generated-readme-files\README.md -Destination .

dotnet build .\Benday.GitRepoSync