$currentFolder = $PSScriptRoot

$certsFolder = Join-Path $currentFolder "certs"

If(!(Test-Path -Path $certsFolder))
{
    New-Item -ItemType Directory -Force -Path $certsFolder
    if(!(Test-Path -Path (Join-Path $certsFolder "localhost.pfx") -PathType Leaf)){
        Set-Location $certsFolder
        dotnet dev-certs https -v -ep localhost.pfx -p a00051f3-9bb9-49cf-bfea-1a77138db958 -t        
    }
}

Set-Location $currentFolder
docker-compose up -d
exit $LASTEXITCODE