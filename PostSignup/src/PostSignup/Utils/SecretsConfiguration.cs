using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PostSignup.Utils
{
    public class SecretsConfiguration
    {
        public string DbConnection { get; set; } = string.Empty;

        public Cognito Cognito { get; set; } = new();

        public static SecretsConfiguration FromEnvironment(IConfiguration? config = null) => new()
        {
            DbConnection = Environment.GetEnvironmentVariable("DBConnection") ?? config?["ConnectionStrings:DBConnection"] ??string.Empty,


            Cognito = JsonConvert.DeserializeObject<Cognito>(
                Environment.GetEnvironmentVariable("Cognito") ?? config?["Cognito"] ?? "{}"
            ) ?? new Cognito(),
        };
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