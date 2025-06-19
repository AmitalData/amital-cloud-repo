namespace AmitalCloud.Shipment.Domain.Interfaces
{
    public interface IShipmentValidationClass
    {
        string GetErrorMessage(object value, object instance, string propertyName);
        bool IsValid(object value, object instance, string propertyName);
    }
}