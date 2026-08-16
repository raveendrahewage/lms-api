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
    [SignalROutput(HubName = "notificationhub", ConnectionStringSetting = "AzureSignalRConnectionString")]
    public async Task<SignalRMessageAction> Run(
        [ServiceBusTrigger("pdf-processing-queue", Connection = "ServiceBusConnectionString")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        // 1. Extract string body from ServiceBusReceivedMessage
        var messageBody = message.Body.ToString();
        var job = JsonSerializer.Deserialize<PdfJobMessage>(messageBody)!;

        logger.LogInformation("Processing PDF Job: {JobId} for User: {UserId}", job.JobId, job.UserId);

        // 2. Download file stream from Azure Blob Storage
        var containerClient = blobServiceClient.GetBlobContainerClient("pdf-uploads");
        var blobClient = containerClient.GetBlobClient(job.BlobName);

        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        // 3. Perform PDF Processing / Extract Data (Simulated work)
        await Task.Delay(3000);

        // 4. Complete the queue message explicitly (optional with ServiceBusMessageActions)
        await messageActions.CompleteMessageAsync(message);

        // 5. Return SignalR message output binding
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