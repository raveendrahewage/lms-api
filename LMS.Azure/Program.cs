using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

string blobConnectionString = builder.Configuration["BlobStorageConnection"]
    ?? throw new InvalidOperationException("BlobStorageConnection is missing from configuration.");

builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));

builder.Build().Run();