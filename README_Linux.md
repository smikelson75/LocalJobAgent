# LocalJobAgent on Linux

This guide explains how to deploy and run the LocalJobAgent application as a systemd service on Linux.

## Prerequisites

- .NET 9.0 Runtime installed on the target Linux server.
  - See [Install .NET on Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux) for instructions specific to your distribution.

## 1. Publish the Application

Publish the application for Linux (assuming x64 architecture):

```bash
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish-linux
```

*Note: If you want a self-contained application (no .NET runtime required on the server), remove `--self-contained false` and add `-p:PublishSingleFile=true` if desired.*

## 2. Deploy to Server

1.  Copy the contents of the `./publish-linux` directory to your Linux server. A common location is `/opt/localjobagent`.

    ```bash
    sudo mkdir -p /opt/localjobagent
    sudo cp -r ./publish-linux/* /opt/localjobagent/
    sudo chmod +x /opt/localjobagent/LocalJobAgent
    ```

## 3. Create Systemd Service

Create a systemd service file to manage the application.

1.  Create the file `/etc/systemd/system/localjobagent.service`:

    ```bash
    sudo nano /etc/systemd/system/localjobagent.service
    ```

2.  Paste the following configuration:

    ```ini
    [Unit]
    Description=Local Job Agent Worker
    After=network.target

    [Service]
    # Type=notify is supported because we use Microsoft.Extensions.Hosting.Systemd
    Type=notify
    
    # Adjust paths if you installed elsewhere or are using a self-contained app
    ExecStart=/usr/bin/dotnet /opt/localjobagent/LocalJobAgent.dll
    # If self-contained: ExecStart=/opt/localjobagent/LocalJobAgent
    
    WorkingDirectory=/opt/localjobagent
    User=www-data
    Restart=always
    RestartSec=10
    
    # Environment variables can be set here
    # Environment=DOTNET_ENVIRONMENT=Production

    [Install]
    WantedBy=multi-user.target
    ```

3.  Save and exit (Ctrl+O, Enter, Ctrl+X).

## 4. Enable and Start the Service

1.  Reload systemd to recognize the new service:

    ```bash
    sudo systemctl daemon-reload
    ```

2.  Enable the service to start on boot:

    ```bash
    sudo systemctl enable localjobagent
    ```

3.  Start the service immediately:

    ```bash
    sudo systemctl start localjobagent
    ```

4.  Check the status:

    ```bash
    sudo systemctl status localjobagent
    ```

## 5. Viewing Logs

Since the application is integrated with systemd, standard output is captured by the journal.

To view logs:

```bash
sudo journalctl -u localjobagent -f
```

```bash
# View logs in real-time
sudo journalctl -u localservice -f

# View all logs for the service
sudo journalctl -u localservice
```

## 6. Running with Docker

You can also run the service in a Docker container to test the application logic in a Linux environment.

1.  **Build the Docker Image**:

    ```bash
    docker build -t localservice .
    ```

2.  **Run the Container**:

    ```bash
    docker run -d --name localservice-test -p 8080:8080 -p 8081:8081 localservice
    ```

3.  **Test the Service**:

    You can use `grpcurl` to test the service running in the container:

    ```bash
    grpcurl -plaintext -d '{"message": "Hello Docker"}' localhost:8080 echo.Echo/Send
    ```

4.  **View Logs**:

    ```bash
    docker logs localservice-test
    ```

5.  **Stop and Remove**:

    ```bash
    docker stop localservice-test
    docker rm localservice-test
    ```
