# LocalService

A C# Worker Service that writes a log entry to a file every 5 seconds. It is configured to run as a Windows Service on Windows and a Systemd Service on Linux.

## Prerequisites

- .NET 9.0 SDK or later

## Building

```powershell
dotnet build
```

## Running Locally

```powershell
dotnet run
```

The service will write logs to `bin\Debug\net9.0\service_log.txt` (or `service_log.txt` in the publish directory).

## Linux Deployment

For instructions on deploying to Linux, see [README_Linux.md](README_Linux.md).

## Publishing (Windows)

To publish the service as a single executable:

```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

## Installing as a Windows Service

1.  Open PowerShell as Administrator.
2.  Run the following command (adjust the path to your published executable):

```powershell
sc.exe create "LocalService" binPath= "C:\Path\To\LocalService.exe"
```

3.  Start the service:

```powershell
sc.exe start "LocalService"
```

4.  Stop the service:

```powershell
sc.exe stop "LocalService"
```

5.  Delete the service:

```powershell
sc.exe delete "LocalService"
```
