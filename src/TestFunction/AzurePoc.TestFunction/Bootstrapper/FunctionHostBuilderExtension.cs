using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Linq;

namespace AzurePoc.TestFunction.Bootstrapper
{
    public static class FunctionHostBuilderExtension
    {
        public static IFunctionsHostBuilder ConfigureAppSettings(this IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder.Services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;
            var appDirectory = executionContextOptions.AppDirectory;

            var tempConfig = new ConfigurationBuilder()
                .SetBasePath(appDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configurationBuilder = new ConfigurationBuilder();

            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfiguration configRoot)
            {
                configurationBuilder.AddConfiguration(configRoot);
            }

            var configuration = configurationBuilder
                .SetBasePath(appDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddAzureKeyVault(tempConfig["keyVaultUrl"])
                .Build();

            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

            return builder;
        }

        private static IConfigurationBuilder AddAzureKeyVault(this IConfigurationBuilder configurationBuilder, string keyVaultUrl)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            configurationBuilder.AddAzureKeyVault(keyVaultUrl, keyVaultClient, new DefaultKeyVaultSecretManager());

            return configurationBuilder;
        }
    }
}
