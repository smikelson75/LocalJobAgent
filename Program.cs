using LocalJobAgent.Services;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

if (OperatingSystem.IsWindows())
{
  builder.Host.UseWindowsService(options =>
  {
    options.ServiceName = "LocalJobAgent";
  });
  LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
}
else if (OperatingSystem.IsLinux())
{
  // Only use Systemd if not running in a container (Docker uses Console lifetime)
  var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
  if (!isDocker)
  {
    builder.Host.UseSystemd();
  }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<JobService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
