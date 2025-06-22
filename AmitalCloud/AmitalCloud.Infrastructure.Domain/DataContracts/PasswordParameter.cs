namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class PasswordParameter
    {
        public string Password { get; set; } = string.Empty;
        public bool isHashPassword { get; set; }
        public bool IsOneTimePassword { get; set; }
    }
}