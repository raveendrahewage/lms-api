using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DefaultAzureCredential = Azure.Identity.DefaultAzureCredential;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var keyVaultUrl = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

if (!string.IsNullOrEmpty(keyVaultUrl))
{
    // Production / Staging: Read secrets via Key Vault & Managed Identity
    var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    builder.Services.AddSingleton(secretClient);

    var blobConnectionString = secretClient.GetSecret("ConnectionStrings:BlobStorageConnection").Value.Value;
    builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));
}
else
{
    var blobConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:BlobStorageConnection")!;
    builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));
}

builder.Build().Run();
