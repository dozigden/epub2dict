Set-Location ./output

Remove-Item ./compressed/*.*

#Compress html files
$files = Get-ChildItem *.html

foreach ($file in $files) {
    $file
    &"C:\Program Files\7-Zip\7z.exe" a -tgzip compressed\$($file.Name) $($file.Name)
}

#Call wsl to run marisa-build, must have been installed under wsl with "sudo apt install marisa"
&wsl marisa-build -owords index.txt
Copy-Item words ./compressed

Set-Location ./compressed
&"C:\Program Files\7-Zip\7z.exe" a -tzip ../../dicthtml-de-en.zip *.html words
Set-Location ../..