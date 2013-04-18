msbuild ../mustache-sharp.sln /p:Configuration=Release
nuget pack ../mustache-sharp/mustache-sharp.csproj -Properties Configuration=Release
nuget push *.nupkg
del *.nupkg