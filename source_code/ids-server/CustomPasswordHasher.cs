using Microsoft.AspNetCore.Identity;

namespace idsserver
{
    public class StupidPasswordHasher : IPasswordHasher<UserAuth>
    {
        public string HashPassword(UserAuth user, string password)
        {
            return "#" + password;
        }

        public PasswordVerificationResult VerifyHashedPassword(UserAuth user, string hashedPassword, string providedPassword)
        {
            if ("#" + providedPassword == hashedPassword)
                return PasswordVerificationResult.Success;
            return PasswordVerificationResult.Failed;
        }
    }
}