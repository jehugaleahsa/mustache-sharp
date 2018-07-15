&dotnet pack "..\MustacheSharp\MustacheSharp.csproj" --configuration Release --output $PWD

.\NuGet.exe push mustache-sharp.*.nupkg -Source https://www.nuget.org/api/v2/package

Remove-Item mustache-sharp.*.nupkg