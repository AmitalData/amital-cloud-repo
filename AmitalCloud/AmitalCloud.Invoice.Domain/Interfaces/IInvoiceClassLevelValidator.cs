namespace AmitalCloud.Invoice.Domain.Interfaces
{
    public interface IInvoiceClassLevelValidator
    {
        string GetErrorMessage(object value, object instance, string propertyName);
        bool IsValid(object value, object instance, string propertyName);
    }
}