using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using NotifyBackend;
using NotifyBackend.Utils;

var builder = WebApplication.CreateBuilder(args);

// Load config from environment variables
var secretsConfiguration = SecretsConfiguration.FromEnvironment(builder.Configuration);
var cognitoConfig = secretsConfiguration.Cognito;

var sqsconfig = secretsConfiguration.AwsSqs;
LambdaLogger.Log(sqsconfig.ToString());

var s3config = secretsConfiguration.AwsS3;
LambdaLogger.Log(s3config.ToString());

// Add DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(secretsConfiguration.DbConnection));

LambdaLogger.Log("Added DbContext");

// Add S3
builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    var region = RegionEndpoint.GetBySystemName(s3config.Region);
    return new AmazonS3Client(region);
});
LambdaLogger.Log("Added IAmazonS3");

builder.Services.AddSingleton<S3Helper>(provider =>
{
    var s3Client = provider.GetRequiredService<IAmazonS3>();
    return new S3Helper(s3config.BucketName, s3Client);
});
LambdaLogger.Log("Added S3Helper");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
LambdaLogger.Log("Added CORS");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
LambdaLogger.Log("Added Lambda hosting");

var app = builder.Build();

app.UsePathBase("/api");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();