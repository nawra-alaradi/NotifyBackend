using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotifyBackend;
using NotifyBackend.Utils;

var builder = WebApplication.CreateBuilder(args);

// Load config from environment variables
var secretsConfiguration = SecretsConfiguration.FromEnvironment(builder.Configuration);
var cognitoConfig = secretsConfiguration.Cognito;
LambdaLogger.Log($"Cognito Authority: {cognitoConfig.Authority}");

Console.WriteLine($">>> Cognito Authority: {cognitoConfig.Authority}");
Console.WriteLine($">>> Cognito ClientId: {cognitoConfig.ClientId}");

var sqsconfig = secretsConfiguration.AwsSqs;
LambdaLogger.Log(sqsconfig.ToString());

var s3config = secretsConfiguration.AwsS3;
LambdaLogger.Log(s3config.ToString());


// ✅ ADD: Cognito JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = cognitoConfig.Authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = cognitoConfig.Authority,
            ValidateAudience = false,
            ValidAudience = cognitoConfig.ClientId,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();


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
// Program.cs
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error; // ← the real exception

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new ApiResponse
        {
            Success = false,
            Message = "An unexpected error occurred " + (app.Environment.IsDevelopment() ? exception?.ToString() : null),
            // In development, expose details; in production, hide them
            Data = null, DataCount=0
        });
    });
});
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();   //  ADD — must be BEFORE UseAuthorization
app.UseAuthorization();

//app.UseEndpoints(endpoints => endpoints.MapControllers());

// Replace UseEndpoints with top-level MapControllers
app.MapControllers();

app.Run();