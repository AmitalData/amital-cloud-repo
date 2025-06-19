namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IValidationClass
    {
        string GetErrorMessage(object value, object instance, string propertyName);
        bool IsValid(object value, object instance, string propertyName);
    }
}