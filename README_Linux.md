# LocalService on Linux

This guide explains how to deploy and run the LocalService application as a systemd service on Linux.

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

1.  Copy the contents of the `./publish-linux` directory to your Linux server. A common location is `/opt/localservice`.

    ```bash
    sudo mkdir -p /opt/localservice
    sudo cp -r ./publish-linux/* /opt/localservice/
    sudo chmod +x /opt/localservice/LocalService
    ```

## 3. Create Systemd Service

Create a systemd service file to manage the application.

1.  Create the file `/etc/systemd/system/localservice.service`:

    ```bash
    sudo nano /etc/systemd/system/localservice.service
    ```

2.  Paste the following configuration:

    ```ini
    [Unit]
    Description=Local Service Worker
    After=network.target

    [Service]
    # Type=notify is supported because we use Microsoft.Extensions.Hosting.Systemd
    Type=notify
    
    # Adjust paths if you installed elsewhere or are using a self-contained app
    ExecStart=/usr/bin/dotnet /opt/localservice/LocalService.dll
    # If self-contained: ExecStart=/opt/localservice/LocalService
    
    WorkingDirectory=/opt/localservice
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
    sudo systemctl enable localservice
    ```

3.  Start the service immediately:

    ```bash
    sudo systemctl start localservice
    ```

4.  Check the status:

    ```bash
    sudo systemctl status localservice
    ```

## 5. Viewing Logs

Since the application is integrated with systemd, standard output is captured by the journal.

To view logs:

```bash
# View logs in real-time
sudo journalctl -u localservice -f

# View all logs for the service
sudo journalctl -u localservice
```

Additionally, the service still writes to its own log file as configured in `Worker.cs`.
Check the `service_log.txt` in the working directory:

```bash
tail -f /opt/localservice/service_log.txt
```

## 6. Running with Docker

You can also run the service in a Docker container to test the application logic in a Linux environment.

1.  **Build the Docker Image**:

    ```bash
    docker build -t localservice .
    ```

2.  **Run the Container**:

    ```bash
    docker run -d --name localservice-test localservice
    ```

3.  **View Logs**:

    To see the logs written to the file inside the container:

    ```bash
    docker exec localservice-test cat /app/service_log.txt
    ```

    To follow the logs:

    ```bash
    docker exec -it localservice-test tail -f /app/service_log.txt
    ```

    **Note on Systemd/Journalctl**:
    Standard Docker containers do not run `systemd`, so `journalctl` is not available inside the container. However, Docker captures the standard output (which is what `journalctl` would capture on a server). To view these logs:

    ```bash
    docker logs localservice-test
    ```

4.  **Stop and Remove**:

    ```bash
    docker stop localservice-test
    docker rm localservice-test
    ```
