using LocalService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);

if (OperatingSystem.IsWindows())
{
  builder.Services.AddWindowsService(options =>
  {
    options.ServiceName = "LocalService";
  });

  LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
}
else if (OperatingSystem.IsLinux())
{
  builder.Services.AddSystemd();
}

builder.Services.AddHostedService<Worker>(); var host = builder.Build();
host.Run();
