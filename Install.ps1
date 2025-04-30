$shortcut = (New-Object -ComObject Wscript.Shell).CreateShortcut("$([Environment]::GetFolderPath('Desktop'))\GMEPTitle24.lnk")
$currentDir = Get-Location
$shortcut.TargetPath = "$currentDir\bin\Debug\net8.0-windows7.0\GMEPTitle24.exe"
$shortcut.IconLocation = "$currentDir\gmep.ico"
$shortcut.Save()