
pushd ./SmokeTest/

./Update-Dependencies.ps1

dotnet restore ./SmokeTest.csproj

dotnet run -p ./SmokeTest.csproj --framework netcoreapp2.1

popd