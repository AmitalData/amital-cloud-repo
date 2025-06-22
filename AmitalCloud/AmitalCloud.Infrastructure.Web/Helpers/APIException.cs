namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class APIException
    {
        public string ErrorType { get; set; } = string.Empty;
        public string ShortErrorMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
