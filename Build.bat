dotnet publish --os win --self-contained -p:PublishSingleFile=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
dotnet publish --os linux --self-contained -p:PublishSingleFile=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
start "" "%~dp0bin\Release\net8.0"