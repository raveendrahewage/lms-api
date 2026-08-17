using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using LMS.Azure.Models;
using LMS.Data;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LMS.Azure;

public class ProcessPdfFunction(
    ApplicationDbContext dbContext,
    BlobServiceClient blobServiceClient,
    ILogger<ProcessPdfFunction> logger)
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly ILogger<ProcessPdfFunction> _logger = logger;

    [Function(nameof(ProcessPdfFunction))]
    [SignalROutput(HubName = "NotificationHub", ConnectionStringSetting = "SignalRConnection")]
    public async Task<SignalRMessageAction> Run(
        [ServiceBusTrigger("lms-pdf-processing-queue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message)
    {
        var messageBody = message.Body.ToString();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var job = JsonSerializer.Deserialize<PdfJobMessage>(messageBody, jsonOptions)!;

        logger.LogInformation("Processing PDF Job: {JobId} for User: {UserId}", job.JobId, job.UserId);

        var containerClient = _blobServiceClient.GetBlobContainerClient("pdf-uploads");
        var blobClient = containerClient.GetBlobClient(job.BlobName);

        using var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);

        await Task.Delay(3000);

        var file = _dbContext.Files.FirstOrDefault(f => f.Id == job.FileId);
        Notification insertedNotification = new ();
        if(file is not null)
        {
            file.FileStatus = FileStatus.Completed;
            _dbContext.Files.Update(file);

            var notification = new Notification
            {
                UserId = job.UserId,
                Message = $"Your file('{job.OriginalFileName}') has been processed successfully.",
                Title = "File Processed Successfully",
                Type = NotificationType.FileProcessed,
                TargetUrl = "/dashboard/file-upload"
            };
            var result = await _dbContext.Notifications.AddAsync(notification);
            insertedNotification = result.Entity;
        }
        await _dbContext.SaveChangesAsync();

        return new SignalRMessageAction("PdfCompleted")
        {
            UserId = job.UserId.ToString(),
            Arguments = [insertedNotification]
        };
    }
}