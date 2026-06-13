using Newtonsoft.Json;

namespace NotifyBackend.Utils
{
    public class SecretsConfiguration
    {
        public string DbConnection { get; set; } = string.Empty;
        public AwsSqs AwsSqs { get; set; } = new();
        public AwsS3 AwsS3 { get; set; } = new();

        public Cognito Cognito { get; set; } = new();

        public static SecretsConfiguration FromEnvironment(IConfiguration? config = null) => new()
        {
            DbConnection = Environment.GetEnvironmentVariable("DBConnection") ?? config?["ConnectionStrings:DBConnection"] ??string.Empty,

            AwsSqs = JsonConvert.DeserializeObject<AwsSqs>(
                Environment.GetEnvironmentVariable("AwsSqs") ?? config?["AwsSqs"] ?? "{}"
            ) ?? new AwsSqs(),

            AwsS3 = JsonConvert.DeserializeObject<AwsS3>(
                Environment.GetEnvironmentVariable("AwsS3") ?? config?["AwsS3"] ?? "{}"
            ) ?? new AwsS3(),

            Cognito = JsonConvert.DeserializeObject<Cognito>(
                Environment.GetEnvironmentVariable("Cognito") ?? config?["Cognito"] ?? "{}"
            ) ?? new Cognito(),
        };
    }

    public class AwsSqs
    {
        [JsonProperty("QueueUrl")]
        public string QueueUrl { get; set; } = string.Empty;

        [JsonProperty("Region")]
        public string Region { get; set; } = string.Empty;

        [JsonProperty("QueueName")]
        public string QueueName { get; set; } = string.Empty;

        [JsonProperty("QueueID")]
        public string QueueID { get; set; } = string.Empty;
    }

    public class AwsS3
    {
        [JsonProperty("BucketName")]
        public string BucketName { get; set; } = string.Empty;

        [JsonProperty("Region")]
        public string Region { get; set; } = string.Empty;
    }

    public class Cognito
    {
        [JsonProperty("UserPoolId")]
        public string UserPoolId { get; set; } = string.Empty;
        [JsonProperty("ClientId")]
        public string ClientId { get; set; } = string.Empty;
        [JsonProperty("Region")]
        public string Region { get; set; } = string.Empty;
        // Build the authority URL from the pool ID and region
        public string Authority => $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}";
    }
}