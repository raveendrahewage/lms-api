using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using LMS.Azure.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LMS.Azure;

public class ProcessPdfFunction(
    BlobServiceClient blobServiceClient,
    ILogger<ProcessPdfFunction> logger)
{
    [Function(nameof(ProcessPdfFunction))]
    [SignalROutput(HubName = "notifications", ConnectionStringSetting = "ConnectionStrings--SignalRConnection")]
    public async Task<SignalRMessageAction> Run(
        [ServiceBusTrigger("lms-pdf-processing-queue", Connection = "ConnectionStrings--ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var messageBody = message.Body.ToString();
        var job = JsonSerializer.Deserialize<PdfJobMessage>(messageBody)!;

        logger.LogInformation("Processing PDF Job: {JobId} for User: {UserId}", job.JobId, job.UserId);

        var containerClient = blobServiceClient.GetBlobContainerClient("pdf-uploads");
        var blobClient = containerClient.GetBlobClient(job.BlobName);

        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        await Task.Delay(3000);

        await messageActions.CompleteMessageAsync(message);

        return new SignalRMessageAction("PdfCompleted")
        {
            UserId = job.UserId,
            Arguments = [
                new
                {
                    jobId = job.JobId,
                    fileName = job.OriginalFileName,
                    status = "Completed",
                    timestamp = DateTime.UtcNow
                }
            ]
        };
    }
}