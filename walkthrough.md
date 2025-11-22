# LocalJobAgent: gRPC Starter Project

## 1. Project Overview
**LocalJobAgent** is a sample .NET 9.0 application designed to demonstrate the basics of building and interacting with a gRPC service. It serves as a foundational template for developers looking to understand how gRPC communication works in a .NET environment.

This project demonstrates:
1.  **Defining a Service**: Using Protocol Buffers (`.proto`) to define the API contract.
2.  **Implementing the Server**: Creating a service that handles gRPC requests.
3.  **Creating a Client**: Building a console application to send requests to the server.
4.  **Hosting**: Running the service as a console app, Windows Service, or Systemd Daemon.

## 2. Architecture & Flow
The system consists of two main parts: the **Server** (`LocalJobAgent`) and the **Client** (`TestClient`).

### The Communication Flow
1.  **Client** sends a `TriggerJobRequest` (containing a name and arguments).
2.  **gRPC Framework** serializes this message using Protocol Buffers.
3.  **Server** receives the request in `JobService.cs`.
4.  **Server** processes the request (currently logs it and generates an ID).
5.  **Server** returns a `TriggerJobReply` (containing the ID and status).

## 3. Project Structure
```
LocalJobAgent/
├── Protos/
│   └── job.proto          # The Contract: Defines the messages and service methods.
├── Services/
│   └── JobService.cs      # The Server Logic: Implements the behavior defined in the proto.
├── TestClient/            # The Client: A console app to test the connection.
│   └── Program.cs         # Client code that sends the gRPC request.
├── Program.cs             # Entry Point: Configures the server and hosting.
└── LocalJobAgent.csproj   # Project File: References the proto file.
```

## 4. Code Deep Dive

### A. The Contract (`Protos/job.proto`)
This is the most important file. It is the shared language between the client and server.
```protobuf
service Job {
  rpc TriggerJob (TriggerJobRequest) returns (TriggerJobReply);
}
```
- **`rpc TriggerJob`**: Defines a method that takes a request and returns a reply.
- **`message`**: Defines the data structure for inputs and outputs.

### B. The Server Implementation (`Services/JobService.cs`)
This class inherits from `Job.JobBase` (which is auto-generated from the `.proto` file).
- It overrides `TriggerJob` to handle the incoming request.
- Currently, it simulates work by logging the request and returning a success message immediately.

### C. The Client (`TestClient/Program.cs`)
This shows how to consume the service.
1.  **Channel**: Establishes a connection (`GrpcChannel.ForAddress`).
2.  **Client**: Creates a strongly-typed client (`Job.JobClient`).
3.  **Call**: Invokes `TriggerJobAsync` just like a normal C# method.

## 5. How to Run

### Prerequisites
- .NET 9.0 SDK

### Step 1: Start the Server
The server must be running to accept requests.
```powershell
dotnet run --project LocalJobAgent.csproj
```
*Output will indicate the server is listening (usually on http://localhost:5000).*

### Step 2: Run the Client
Open a **new terminal window** and run the client to send a message.
```powershell
dotnet run --project TestClient/TestClient.csproj
```

### Running with Docker (Linux Support)
To verify the service works in a Linux container:

1.  **Build and Run**:
    ```powershell
    docker compose up --build
    ```
    This maps the container's port 8080 to your machine's port 5000.

2.  **Run the Client**:
    Since the port is mapped to 5000, you can run the same client command:
    ```powershell
    dotnet run --project TestClient/TestClient.csproj
    ```

### Expected Output
**Client Terminal:**
```text
Triggering job 'DataSync'...
Job ID: <guid>
Status: Queued
Message: Job 'DataSync' has been queued successfully.
```

**Server Terminal:**
```text
info: LocalJobAgent.Services.JobService[0]
      Triggering job: DataSync with args: --force. Job ID: <guid>
```

## 6. Key Takeaways
- **Code Generation**: Notice how `JobBase` and `JobClient` didn't need to be written manually. They were generated from `job.proto`.
- **Strong Typing**: Both client and server use the same strongly-typed classes (`TriggerJobRequest`), preventing structure mismatch errors.
