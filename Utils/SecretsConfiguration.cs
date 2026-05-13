using Newtonsoft.Json;

namespace NotifyBackend.Utils
{
    public class SecretsConfiguration
    {
        public static string secretName = "notify-secret";

        [JsonProperty("db-connection")]
        public string DbConnection { get; set; }

        [JsonProperty("smtp-server")]
        public string SmtpServer { get; set; }

        [JsonProperty("sqs-queue-id")]
        public string SqsQueueId { get; set; }

        [JsonProperty("from-address")]
        public string FromAddress { get; set; }

        [JsonProperty("sqs-queue-name")]
        public string SqsQueueName { get; set; }

        [JsonProperty("sqs-queue-url")]
        public string SqsQueueUrl { get; set; }

        // Fixed: Changed from string to Object types
        [JsonProperty("AzureAd")]
        public AzureAd AzureAd { get; set; }

        [JsonProperty("AwsSqs")]
        public AwsSqs AwsSqs { get; set; }

        [JsonProperty("AwsS3")]
        public AwsS3 AwsS3 { get; set; }
    }

    public class AzureAd
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string CallbackPath { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SignedOutCallbackPath { get; set; }
        public string Audience { get; set; }
    }

    public class AwsSqs
    {
        public string QueueUrl { get; set; }
        public string Region { get; set; }
    }

    public class AwsS3
    {
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}