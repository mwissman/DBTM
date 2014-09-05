#$dbtmDir = (Get-ChildItem $env:ChocolateyInstall\lib\dbtm-ui* | Sort-Object $_.LastWriteTime -descending | select -first 1 )
#$dbtmExe = "$dbtmDir\tools\dbtm.ui.exe"

$dbtmBat = "$env:ChocolateyInstall\bin\dbtm.UI.bat" #(Get-BinRoot +"\dbtm.bat")

write-host (get-binroot)

write-host ("dbtm path: " + $dbtmBat)
Install-ChocolateyFileAssociation ".dbschema" $dbtmBat
