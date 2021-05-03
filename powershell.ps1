$libPath = ".\ClassLibrary_5_uzd\ClassLibrary_5_uzd\bin\Debug\netcoreapp3.1\ClassLibrary_5_uzd.dll"
[System.Reflection.Assembly]::LoadFrom($libPath)
[ClassLibrary_5_uzd.Machine] $reader = New-Object 'ClassLibrary_5_uzd.Machine'
$desktopPath = Join-Path ([environment]::GetFolderPath('Desktop')) "output.txt"
$reader.SystemInfo() | Out-File -FilePath $desktopPath
$reader.GetEvents("ESENT") | Out-File -FilePath $desktopPath -Append
Write-Host "Press any key to exit.. (Check output.txt on desktop!)"
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")