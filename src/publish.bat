cd ApiParser

dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true --self-contained true
dotnet publish -r osx-x64 -c Release -p:PublishSingleFile=true --self-contained true
dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=true --self-contained true