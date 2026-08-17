using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using LMS.Data;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

string blobConnectionString = builder.Configuration["BlobStorageConnection"]
    ?? throw new InvalidOperationException("BlobStorageConnection is missing from configuration.");

string dbConnectionString = builder.Configuration["LMSDbConnection"]
    ?? throw new InvalidOperationException("LMSDbConnection is missing from configuration.");

builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(dbConnectionString);
});

builder.Build().Run();