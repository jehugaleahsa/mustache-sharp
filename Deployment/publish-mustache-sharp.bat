"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" ../mustache-sharp.sln /p:Configuration=Release
nuget pack ../mustache-sharp/mustache-sharp.csproj -Properties Configuration=Release
nuget push *.nupkg -Source https://www.nuget.org/api/v2/package
del *.nupkg