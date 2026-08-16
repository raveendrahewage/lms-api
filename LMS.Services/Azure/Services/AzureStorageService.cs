using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using LMS.Services.Azure.Interfaces;
using LMS.Services.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LMS.Service.Services;

public class AzureStorageService : IAzureStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IConfiguration _config;

    public AzureStorageService(
        BlobServiceClient blobServiceClient,
        ServiceBusClient serviceBusClient,
        IConfiguration config)
    {
        _blobServiceClient = blobServiceClient;
        _serviceBusClient = serviceBusClient;
        _config = config;
    }

    public string GenerateUploadSasUrl(string fileName, out string blobName)
    {
        var containerName = _config["Azure:BlobContainerName"] ?? "pdf-uploads";
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        blobName = $"{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-2),
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task EnqueuePdfJobAsync(PdfJobMessage jobMessage)
    {
        var queueName = _config["Azure:ServiceBusQueueName"] ?? "pdf-processing-queue";
        var sender = _serviceBusClient.CreateSender(queueName);

        var message = new ServiceBusMessage(JsonSerializer.Serialize(jobMessage))
        {
            ContentType = "application/json",
            MessageId = jobMessage.JobId
        };

        await sender.SendMessageAsync(message);
    }
}