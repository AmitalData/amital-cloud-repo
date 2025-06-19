using System.Collections.Generic;

namespace AmitalCloud.Invoice.Domain.Interfaces
{
    public interface IInvoiceValidationClass
    {
        bool IsHybrid { get; set; }

        string GetErrorMessage(object instance, string property);
        List<string> GetErrorsInObject(object instance);
        bool IsValid(object value, object instance, string propertyName);
    }
}