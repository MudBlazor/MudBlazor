$nupkgDirectory=$args[0]
$assetName=$args[1]
Add-Type -assembly "system.io.compression.filesystem"
$nupkgFilePath = Get-ChildItem -Path $nupkgDirectory -Filter *.nupkg | Select-Object -First 1
$fileNames = [io.compression.zipfile]::OpenRead($nupkgFilePath).Entries.Name
if($fileNames.Contains($assetName))
{
    Write-Host -Message "Static asset check - $nupkgFilePath contains $assetName"
}
else
{
    Write-Error -Message "Static asset check - $nupkgFilePath does not contain $assetName"
    exit 1
}
