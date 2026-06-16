using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostSignup.Utils;

namespace PostSignup
{
    [LambdaStartup]
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Build configuration here 
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var secretsConfiguration = SecretsConfiguration.FromEnvironment(configuration);

            LambdaLogger.Log("Secret in startup.cs: " + secretsConfiguration.DbConnection);

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(secretsConfiguration.DbConnection));
        }
    }
}