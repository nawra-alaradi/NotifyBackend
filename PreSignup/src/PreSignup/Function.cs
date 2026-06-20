using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PreSignup
{
    public class Function
    {
        public async Task<CognitoPreSignupEvent> FunctionHandler(  // ✅ fixed type name
            CognitoPreSignupEvent cognitoEvent,
            ILambdaContext context)
        {
            LambdaLogger.Log("PreSignUp triggered for user: " + cognitoEvent.UserName);
            LambdaLogger.Log("PreSignUp triggered source: " + cognitoEvent.TriggerSource);

            var triggerSource = cognitoEvent.TriggerSource;

            if (triggerSource == "PreSignUp_ExternalProvider")
            {
                LambdaLogger.Log("Inside if");
                var email = cognitoEvent.Request.UserAttributes["email"];
                LambdaLogger.Log("email: " + email);

                var userPoolId = cognitoEvent.UserPoolId;

                LambdaLogger.Log("pool ID: " + userPoolId);

                var cognitoClient = new AmazonCognitoIdentityProviderClient();

                try
                {
                    var existingUsers = await cognitoClient.ListUsersAsync(new ListUsersRequest
                    {
                        UserPoolId = userPoolId,
                        Filter = $"email = \"{email}\""
                    });


                    var nativeUser = existingUsers.Users
                        .FirstOrDefault(u => u.UserStatus != "EXTERNAL_PROVIDER");
                    LambdaLogger.Log("Native users found: " + existingUsers.Users.Count);

                    if (nativeUser != null)
                    {
                        //  Safer provider name/ID extraction
                        var firstUnderscore = cognitoEvent.UserName.IndexOf('_');
                        var providerName = cognitoEvent.UserName[..firstUnderscore];
                        var providerUserId = cognitoEvent.UserName[(firstUnderscore + 1)..];

                        await cognitoClient.AdminLinkProviderForUserAsync(
                            new AdminLinkProviderForUserRequest
                            {
                                UserPoolId = userPoolId,
                                DestinationUser = new ProviderUserIdentifierType
                                {
                                    ProviderName = "Cognito",
                                    ProviderAttributeValue = nativeUser.Username
                                },
                                SourceUser = new ProviderUserIdentifierType
                                {
                                    ProviderName = providerName,
                                    ProviderAttributeValue = providerUserId,
                                    ProviderAttributeName = "Cognito_Subject"
                                }
                            });

                        LambdaLogger.Log($"Linked {providerName} to existing account: {email}");
                    }
                    else
                    {
                        // ✅ No action needed — Cognito creates the user after we return
                        LambdaLogger.Log($"New Google user, Cognito will create account for: {email}");
                    }
                }
                catch (Exception ex)
                {
                    LambdaLogger.Log($"Error in PreSignUp: {ex.Message}");
                    throw;
                }

                //  Only auto-confirm federated users, moved inside if block
                cognitoEvent.Response.AutoConfirmUser = true;
                cognitoEvent.Response.AutoVerifyEmail = true;
            }
            LambdaLogger.Log("PreSignUp completed for user: " + cognitoEvent.ToString()); 
            return cognitoEvent;
        }
    }
}