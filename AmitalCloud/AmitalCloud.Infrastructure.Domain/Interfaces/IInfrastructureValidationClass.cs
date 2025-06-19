namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IInfrastructureValidationClass
    {
        bool IsValid(object value, object objectInstance, string propertyName);
        string GetErrorMessage(object value, object instance, string propertyName);
    }
}
