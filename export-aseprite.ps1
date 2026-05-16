$asepritePath = "C:\Program Files (x86)\Steam\steamapps\common\Aseprite\aseprite.exe"
$inputFolder = ".\assets"
$outputFolder = ".\assets"
$layerGroups = @("Base", "Overlay")

Get-ChildItem $inputFolder -Filter *.aseprite | ForEach-Object {
    $inputFile = $_.FullName
    foreach ($group in $layerGroups) {
        $outputFile = Join-Path $outputFolder (
            "{0}_{1}.png" -f $_.BaseName, $group.ToLower()
        )

        & $asepritePath -b $inputFile --layer $group --save-as $outputFile

        Write-Host "Exported [$group]: $outputFile"
    } 
}
