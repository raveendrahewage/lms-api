using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace LMS.Api.Helpers
{
    public static class ConfigurationManagerExtensions
    {
        public static void ConfigureKeyVault(this ConfigurationManager config, bool isDevelopment)
        {
            string keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

            if (string.IsNullOrWhiteSpace(keyVaultEndpoint))
                throw new InvalidOperationException("Store the Key Vault endpoint in a KEYVAULT_ENDPOINT environment variable.");

            var options = new DefaultAzureCredentialOptions
            {
                ExcludeManagedIdentityCredential = isDevelopment
            };

            SecretClient secretClient = new(new Uri(keyVaultEndpoint), new DefaultAzureCredential(options));
            config.AddAzureKeyVault(secretClient, new AzureKeyVaultConfigurationOptions());
        }
    }
}
