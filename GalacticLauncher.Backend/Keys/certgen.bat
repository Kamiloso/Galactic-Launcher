@echo off
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$cn = Read-Host 'Podaj nazwe certyfikatu (CN)'; " ^
    "$path = Read-Host 'Podaj pelna sciezke zapisu (np. C:\temp\cert.pfx)'; " ^
    "$pass = Read-Host 'Podaj haslo do PFX' -AsSecureString; " ^
    "$cert = New-SelfSignedCertificate -CertStoreLocation 'Cert:\CurrentUser\My' -Subject \"CN=$cn\" -KeyAlgorithm ECDSA_nistP256 -HashAlgorithm SHA256; " ^
    "Export-PfxCertificate -Cert $cert -FilePath $path -Password $pass | Out-Null; " ^
    "Remove-Item -Path $cert.PSPath -Force; " ^
    "Write-Host 'Gotowe! Certyfikat zapisany i posprzatany z magazynu.' -ForegroundColor Green; " ^
    "Pause"