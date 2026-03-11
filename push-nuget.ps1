$apiKey = Get-Content .\.nuget-api-key -Raw
$apiKey = $apiKey.Trim()

$packagePath = ".\IOTWave\nupkg\IOTWave.*.nupkg"
$package = Get-ChildItem $packagePath | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if ($package) {
    Write-Host "Pushing package: $($package.Name)"
    dotnet nuget push $package.FullName --api-key $apiKey --source https://api.nuget.org/v3/index.json
}
else {
    Write-Host "No package found. Run 'dotnet pack -c Release' in the LonCan10 directory first."
}
