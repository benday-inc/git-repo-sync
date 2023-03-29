#!/bin/bash

dotnet test ./Benday.GitRepoSync

cp ./Benday.GitRepoSync/generated-readme-files/README-for-nuget.md .
cp ./Benday.GitRepoSync/generated-readme-files/README.md .

dotnet build ./Benday.GitRepoSync
