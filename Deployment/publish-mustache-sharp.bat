nuget pack ../mustache-sharp/mustache-sharp.csproj -Prop Configuration=Release -Build
nuget push *.nupkg
del *.nupkg