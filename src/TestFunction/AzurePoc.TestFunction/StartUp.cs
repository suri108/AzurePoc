using AzurePoc.TestFunction;
using AzurePoc.TestFunction.Bootstrapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzurePoc.TestFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureAppSettings();
        }
    }
}
