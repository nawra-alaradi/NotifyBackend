using PostSignup.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace PostSignup.Utils
{
    public static class IdentityExtensions
    {

        public static string GetUserSub(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier)?? ((ClaimsIdentity)identity).FindFirst("sub");
            var value = (claim != null) ? claim.Value : string.Empty;
            return value;
        }


        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Name);
            var value = (claim != null) ? claim.Value : string.Empty;
            return value;
        }


        public static string GetUserEmail(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Email)?? ((ClaimsIdentity)identity).FindFirst("email");
            var value = (claim != null) ? claim.Value : string.Empty;
            return value;
        }




        public static string GetUserGender(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.Gender);
            var value = (claim != null) ? claim.Value : string.Empty;
            return value;
        }


        public static Users? GetNotifyUser(this IIdentity identity, DatabaseContext db)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier);
            var value = (claim != null) ? claim.Value : string.Empty;

            var user = db.Users.FirstOrDefault(a => a.CognitoSub.Equals(value));
            return user;
        }


        public static bool IsNotifyUser(this IIdentity identity, DatabaseContext db)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier);
            var value = (claim != null) ? claim.Value : string.Empty;

            var user = db.Users.FirstOrDefault(a => a.CognitoSub.Equals(value));
            return (user==null)? false: true;
        }


        public static int GetUserID(this IIdentity identity, DatabaseContext db)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier);
            var value = (claim != null) ? claim.Value : string.Empty;

            var user = db.Users.FirstOrDefault(a => a.CognitoSub.Equals(value));
            return (user == null) ? 0 : user.ID;
        }
    }
}
