using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzurePoc.TestFunction
{
    public class TestTopicFunction
    {
        private readonly ILogger<TestTopicFunction> _logger;
        private readonly IConfiguration _config;

        public TestTopicFunction(ILogger<TestTopicFunction> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [FunctionName("TestTopicFunction")]
        public async Task Run([ServiceBusTrigger("%TopicName%", "%SubscriptionName%", Connection = "ConnectionString")]Message message)
        {
            _logger.LogInformation("Received message with id {messageId}", message.MessageId);

            // Function app implementation
            var testConfigValue = _config["TestConfigSetting"];

            _logger.LogInformation("Processed message with id {messageId}, test config - {testConfigValue}", message.MessageId, testConfigValue);
        }
    }
}
