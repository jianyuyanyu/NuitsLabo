# docker run -v ${pwd}\script:c:\script -it --isolation=hyperv mcr.microsoft.com/windows/servercore:ltsc2019 powershell
docker run -v ${pwd}\script:c:\script -it --isolation=hyperv nuitsjp/server-core-dotnet:6.0 powershell