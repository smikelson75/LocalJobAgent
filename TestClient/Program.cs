using Grpc.Net.Client;
using LocalJobAgent;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Job.JobClient(channel);

Console.WriteLine("Triggering job 'DataSync'...");
try
{
  var reply = await client.TriggerJobAsync(new TriggerJobRequest
  {
    JobName = "DataSync",
    Args = "--force"
  });

  Console.WriteLine($"Job ID: {reply.JobId}");
  Console.WriteLine($"Status: {reply.Status}");
  Console.WriteLine($"Message: {reply.Message}");
}
catch (Exception ex)
{
  Console.WriteLine($"Error communicating with gRPC service: {ex.Message}");
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
