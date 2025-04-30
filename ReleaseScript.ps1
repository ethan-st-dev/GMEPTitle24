Compress-Archive -Force -Path "bin", "Install.ps1", "gmep.ico" -DestinationPath "GMEPTitle24.zip"
Copy-Item "GMEPTitle24.zip" -Destination "Z:\GMEP Engineers\Users\GMEP Softwares"