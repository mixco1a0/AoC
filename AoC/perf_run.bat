dotnet build -c Release
bin\Release\net7.0\AoC.exe -skiplatest -runperf -compactperf -perfrecordcount 100 -perftimeout 3600000 -ignoreconfigfile
pause