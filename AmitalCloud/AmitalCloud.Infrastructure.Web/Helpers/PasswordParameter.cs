namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class PasswordParameter
    {
        public string Password { get; set; }
        public bool isHashPassword { get; set; }
        public bool IsOneTimePassword { get; set; }
    }
}