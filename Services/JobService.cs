using Grpc.Core;
using LocalJobAgent;

namespace LocalJobAgent.Services;

/// <summary>
/// Service implementation for handling job trigger requests.
/// Inherits from the generated Job.JobBase gRPC class.
/// </summary>
public class JobService : Job.JobBase
{
  private readonly ILogger<JobService> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="JobService"/> class.
  /// </summary>
  /// <param name="logger">The logger instance.</param>
  public JobService(ILogger<JobService> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// Triggers a job based on the provided request.
  /// </summary>
  /// <param name="request">The request containing job name and arguments.</param>
  /// <param name="context">The server call context.</param>
  /// <returns>A reply containing the job ID and status.</returns>
  public override Task<TriggerJobReply> TriggerJob(TriggerJobRequest request, ServerCallContext context)
  {
    _logger.LogInformation("Received request to process job: {JobName}", request.JobName);

    var jobId = Guid.NewGuid().ToString();
    _logger.LogInformation("Job ID {JobId} assigned to {JobName} with args: {Args}", jobId, request.JobName, request.Args);

    // TODO: Actually start the background process here.

    return Task.FromResult(new TriggerJobReply
    {
      JobId = jobId,
      Status = "Queued",
      Message = $"Job '{request.JobName}' has been queued successfully."
    });
  }
}
