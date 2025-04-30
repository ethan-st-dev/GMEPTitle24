$shortcut = (New-Object -ComObject Wscript.Shell).CreateShortcut("$([Environment]::GetFolderPath('Desktop'))\GMEPTitle24.lnk")
$currentDir = Get-Location
$shortcut.TargetPath = "$currentDir\bin\Debug\net9.0-windows\GMEPTitle24.exe"
$shortcut.IconLocation = "$currentDir\gmep.ico"
$shortcut.Save()