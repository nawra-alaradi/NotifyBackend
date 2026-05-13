using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotifyBackend;
using NotifyBackend.Utils;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration; // configuration is an instance of ConfigurationManager

var secretJson = SecretsManager.Get(secretName: SecretsConfiguration.secretName).Result;
var secretsConfiguration = JsonConvert.DeserializeObject<SecretsConfiguration>(secretJson);
//deserialize AzureAD
var azureadconfig = secretsConfiguration.AzureAd;
LambdaLogger.Log(azureadconfig.ToString());
var sqsconfig = secretsConfiguration.AwsSqs;
LambdaLogger.Log(sqsconfig.ToString());

var s3config = secretsConfiguration.AwsS3;
LambdaLogger.Log(s3config.ToString());



//add dbcontext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection") ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.")));


LambdaLogger.Log("Added Azure AD");

//adding S3 and SQS configurations: 


//services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    var region = RegionEndpoint.GetBySystemName(s3config.Region);
    return new AmazonS3Client(region);
});
LambdaLogger.Log("Added IAmazon S3");

builder.Services.AddSingleton<S3Helper>(provider =>
{
    var s3Client = provider.GetRequiredService<IAmazonS3>();
    return new S3Helper(s3config.BucketName, s3Client);
});
LambdaLogger.Log("Added S3 Helper");


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
});
LambdaLogger.Log("Added CORS");


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
LambdaLogger.Log("Added lambda hosting");

var app = builder.Build();


app.UsePathBase("/api");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
