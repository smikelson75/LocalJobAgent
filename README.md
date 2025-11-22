# LocalJobAgent

A .NET 9.0 gRPC Service designed to act as a "Local Agent" for triggering background jobs. It supports running as a Windows Service, a Systemd Daemon (Linux), or in a Docker container.

## Features

- **gRPC Interface**: Standardized communication using Protocol Buffers.
- **Cross-Platform Hosting**:
  - **Windows**: Runs as a native Windows Service.
  - **Linux**: Runs as a Systemd Daemon.
  - **Docker**: Ready-to-use `docker-compose` setup.
- **Test Client**: Includes a dedicated console client for easy testing.

## Service Definition

The service implements the following gRPC definition (`Protos/job.proto`):

```protobuf
service Job {
  rpc TriggerJob (TriggerJobRequest) returns (TriggerJobReply);
}

message TriggerJobRequest {
  string job_name = 1;
  string args = 2;
}

message TriggerJobReply {
  string job_id = 1;
  string status = 2;
  string message = 3;
}
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK

### Building

```powershell
dotnet build
```

### Running Locally

Start the service:

```powershell
dotnet run --project LocalJobAgent.csproj
```

The service listens on `http://localhost:5000` (HTTP/2).

### Testing with the Client

Open a new terminal and run the included test client:

```powershell
dotnet run --project TestClient/TestClient.csproj
```

You should see output confirming the job was triggered:
```text
Triggering job 'DataSync'...
Job ID: <guid>
Status: Queued
```

## Running with Docker

To run the service in a Linux container:

```powershell
docker compose up --build
```

Then run the client as usual (it connects to `localhost:5000` which is mapped to the container).

## Documentation

For a detailed code walkthrough and architecture explanation, see [walkthrough.md](walkthrough.md).

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
