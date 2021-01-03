namespace IdentityServer.API.Common
{
    public class AccountOptions
    {
        public static bool AllowRememberLogin = true;

        public static bool ShowLogoutPrompt = false;

        public static string InvalidCredentialsErrorMessage = "Invalid email or password";
    }
}