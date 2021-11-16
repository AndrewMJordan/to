# To

# Installation
0. Create a GitHub personal access token ([here](https://github.com/settings/tokens/new)) with at least `read:packages` scope.

1. Add my package registry.
```bash
$ dotnet nuget add source --username <USERNAME> --password <PERSONAL_ACCESS_TOKEN> --store-password-in-clear-text --name github/AndrewMJordan "https://nuget.pkg.github.com/AndrewMJordan/index.json"
```

2. Install this dotnet tool.
```bash
$ dotnet tool install --global Andtech.To
```
