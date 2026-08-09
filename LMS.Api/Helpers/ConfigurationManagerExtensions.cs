using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace LMS.Api.Helpers
{
    public static class ConfigurationManagerExtensions
    {
        public static void ConfigureKeyVault(this ConfigurationManager config)
        {
            string keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

            if (string.IsNullOrWhiteSpace(keyVaultEndpoint))
                throw new InvalidOperationException("Store the Key Vault endpoint in a KEYVAULT_ENDPOINT environment variable.");

            SecretClient secretClient = new(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
            config.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions());
        }
    }
}
