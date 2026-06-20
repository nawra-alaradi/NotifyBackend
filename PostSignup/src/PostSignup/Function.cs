using Amazon.Lambda.Annotations;
using Amazon.Lambda.CognitoEvents;
using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using PostSignup.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace PostSignup
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input">The event for the Lambda function handler to process.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns></returns>
        private readonly DatabaseContext _db;
        public Function(DatabaseContext db)
        {

            LambdaLogger.Log($"PostConfirmation adding DBcontext");

            _db = db;

            // _db =db;
        }


        [LambdaFunction]
        public async Task<CognitoPostConfirmationEvent> FunctionHandler(
           CognitoPostConfirmationEvent cognitoEvent,  // receive Cognito event
           ILambdaContext context)
        {
            try
            {
                //  Extract user info from Cognito event
                var attributes = cognitoEvent.Request.UserAttributes;

                string cognitoSub = attributes.GetValueOrDefault("sub", string.Empty);
                string email = attributes.GetValueOrDefault("email", string.Empty);
                string name = attributes.GetValueOrDefault("name", string.Empty);

                LambdaLogger.Log($"PostConfirmation triggered for sub: {cognitoSub}, email: {email}");

                if (string.IsNullOrEmpty(cognitoSub))
                {
                    LambdaLogger.Log("Warning: cognitoSub is empty, skipping DB insert");
                    return cognitoEvent; // always return the event back to Cognito
                }

                //  Check if user already exists (idempotency)
                var existingUser =  _db.Users
                    .FirstOrDefault(u => u.Email.Equals (email));

                if (existingUser == null)
                {
                    var newUser = new Users
                    {
                        CognitoSub = cognitoSub,
                        Email = email,
                        Name = name,
                        CreatedAt = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow
                    };

                    _db.Users.Add(newUser);
                     _db.SaveChanges();

                    LambdaLogger.Log($"User added to DB: {cognitoSub}");
                }
                else
                {
                    LambdaLogger.Log($"User already exists, skipping: {cognitoSub}");
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Error in PostConfirmation: {ex.Message}");
                throw; //  rethrow so Cognito knows something went wrong
            }

            //  Must return the event back — Cognito requires this
            return cognitoEvent;
        }
    }
}
