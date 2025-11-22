using Grpc.Core;
using LocalJobAgent;

namespace LocalJobAgent.Services;

public class JobService : Job.JobBase
{
  private readonly ILogger<JobService> _logger;

  public JobService(ILogger<JobService> logger)
  {
    _logger = logger;
  }

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
