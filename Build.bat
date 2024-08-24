dotnet publish --os win --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
dotnet publish --os linux --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
dotnet publish --os osx --self-contained -p:PublishSingleFile=True -p:PublishTrimmed=True -p:TrimMode=CopyUsed -p:PublishReadyToRun=True
pause
start "" "%~dp0bin\Release\net8.0"