dotnet build -c Release
bin\Release\net7.0\AoC.exe -skiplatest -compactperf -runperf -perfrecordcount 100 -perftimeout 3600000 -ignoreconfigfile
pause