# The new version passed to the script by argument
$newVersion=$args[0]

if($null -eq $newVersion) 
{
    Throw "No version supplied - aborting"
}

Write-Host "Updating project and cache busting static assets using version string $newVersion"

# The repository root is always 2 levels below the tools directory
$repoRoot = Split-Path(Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent

# Create changes
# These can be commited using
# git commit -am "Build: Update version number to x.x.x"
$projectFile = Join-Path $repoRoot src/MudBlazor/MudBlazor.csproj
(Get-Content -Path $projectFile) -replace '<Version>.*</Version>', "<Version>$newVersion</Version>"
    | Set-Content -Path $projectFile -Encoding utf8BOM

$staticAssetsFind = '(js|css)\?v=.*?"'
$staticAssetsReplace = "`$1?v=$newVersion`""

$layoutFile = Join-Path $repoRoot src/MudBlazor.Docs.Server/Pages/_Layout.cshtml
(Get-Content -Path $layoutFile) -replace $staticAssetsFind, $staticAssetsReplace
    | Set-Content -Path $layoutFile -Encoding utf8BOM

$indexFile = Join-Path $repoRoot src/MudBlazor.Docs.Wasm/wwwroot/index.html
(Get-Content -Path $indexFile) -replace  $staticAssetsFind, $staticAssetsReplace
    | Set-Content -Path $indexFile -Encoding utf8BOM

$hostFile = Join-Path $repoRoot src/MudBlazor.Docs.WasmHost/Pages/_Host.cshtml
(Get-Content -Path $hostFile) -replace  $staticAssetsFind, $staticAssetsReplace
    | Set-Content -Path $hostFile -Encoding utf8BOM

Write-Host "Update complete"