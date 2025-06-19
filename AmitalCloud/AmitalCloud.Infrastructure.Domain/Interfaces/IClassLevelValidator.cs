using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IClassLevelValidator
    {
        bool IsValid(object value, object instance, string propertyName);
        string GetErrorMessage(object instance, string property);
        List<string> GetErrorsInObject(object instance);
    }
}
