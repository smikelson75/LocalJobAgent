namespace LocalService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service_log.txt");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            try
            {
                await File.AppendAllTextAsync(logFilePath, $"Worker running at: {DateTimeOffset.Now}{Environment.NewLine}", stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to log file.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
