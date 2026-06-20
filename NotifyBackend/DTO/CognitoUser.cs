namespace NotifyBackend.DTO
{
    public class CognitoUser
    {

        public string Sub { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
        public string Username { get; set; } = string.Empty;
        public string IdentityProvider { get; set; } = "Cognito"; // default = native
        public bool IsExternalProvider => IdentityProvider != "Cognito";
    }
}
